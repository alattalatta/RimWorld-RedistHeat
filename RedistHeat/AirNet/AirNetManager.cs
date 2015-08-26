using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RedistHeat
{
    public static class AirNetManager
    {
        //Pending comps
        private static List< AirNet >[] allNets;
        private static List< CompAir >[] newComps;
        private static List< CompAir >[] oldComps;

        //Pending graphic updates
        private static List< IntVec3 > updatees;

        static AirNetManager()
        {
            var layerCount = Common.NetLayerCount();
            allNets = new List<AirNet>[layerCount];
            newComps = new List<CompAir>[layerCount];
            oldComps = new List<CompAir>[layerCount];

            updatees = new List<IntVec3>();

            for ( var i = 0; i < layerCount; i++ )
            {
                allNets[i] = new List<AirNet>();
                newComps[i] = new List<CompAir>();
                oldComps[i] = new List<CompAir>();
            }

            if(Prefs.LogVerbose)
                Log.Message( "LT-RH: Initialized NetManager." );
        }

        /*public static void Reinit()
        {
            var layerCount = Common.NetLayerCount();
            allNets = new List< AirNet >[layerCount];
            newComps = new List< CompAir >[layerCount];
            oldComps = new List< CompAir >[layerCount];

            updatees = new List< IntVec3 >();

            for ( var i = 0; i < layerCount; i++ )
            {
                allNets[i] = new List< AirNet >();
                newComps[i] = new List< CompAir >();
                oldComps[i] = new List< CompAir >();
            }
        }*/

        public static void Reload()
        {
            foreach ( var current in Find.Map.listerBuildings.allBuildingsColonist )
            {
                var compAir = current.TryGetComp< CompAir >();
                if ( compAir != null )
                {
                    newComps[(int) compAir.currentLayer].Add( compAir );
                    updatees.Add( current.Position );
                }
            }
        }

        public static void NotifyCompSpawn( CompAir compAir )
        {
            if ( compAir.IsLayerOf( NetLayer.Lower ) )
            {
#if DEBUG
                //Log.Message(compAir + " spawned at " + compAir.parent.Position + ", layer " + NetLayer.Lower);
#endif
                newComps[(int) NetLayer.Lower].Add( compAir );
            }

            else if ( compAir.IsLayerOf( NetLayer.Upper ) )
            {
#if DEBUG
                //Log.Message( compAir + " spawned at " + compAir.parent.Position + ", layer " + NetLayer.Upper );
#endif
                newComps[(int) NetLayer.Upper].Add( compAir );
            }

            AddToGraphicUpdateList( compAir );
        }

        public static void NotifyCompDespawn( CompAir compAir )
        {
            if ( compAir.IsLayerOf( NetLayer.Lower ) )
            {
#if DEBUG
                //Log.Message( compAir + " despawned at " + compAir.parent.Position + ", layer " + NetLayer.Lower );
#endif
                oldComps[(int) NetLayer.Lower].Add( compAir );
            }

            if ( compAir.IsLayerOf( NetLayer.Upper ) )
            {
#if DEBUG
                //Log.Message( compAir + " despawned at " + compAir.parent.Position + ", layer " + NetLayer.Upper );
#endif
                oldComps[(int) NetLayer.Upper].Add( compAir );
            }

            AddToGraphicUpdateList( compAir );
        }

        public static void NotifyCompLayerChange( CompAir compAir, NetLayer oldLayer )
        {
            if ( oldLayer == compAir.currentLayer )
            {
                Log.Error( "LT-RH: Tried to change " + compAir + "\'s layer to " + compAir.currentLayer +
                           ", which is not different!" );
                return;
            }

            oldComps[(int) oldLayer].Add( compAir );
            newComps[(int) compAir.currentLayer].Add( compAir );

#if DEBUG
            //Log.Message( compAir + " changed layer to " + compAir.currentLayer );
#endif
            AddToGraphicUpdateList( compAir );
        }

        public static void RegisterAirNet( AirNet newNet )
        {
#if DEBUG
            Log.Message( "Created net " + newNet );
#endif

            allNets[newNet.LayerInt].Add( newNet );
            AirNetGrid.NotifyNetCreated( newNet );
        }

        public static void DeregisterAirNet( AirNet oldNet )
        {
#if DEBUG
            Log.Message( "Deleted net " + oldNet );
#endif

            allNets[oldNet.LayerInt].Remove( oldNet );
            AirNetGrid.NotifyNetDeregistered( oldNet );
        }

        public static void AirNetsTick()
        {
            foreach ( var layer in allNets )
            {
                foreach ( var net in layer )
                {
                    net.AirNetTick();
                }
            }
        }

        public static void AirNetsUpdate()
        {
            for ( var layerInt = 0; layerInt < Common.NetLayerCount(); layerInt++ )
            {
                if ( !newComps.Any() && !oldComps.Any() )
                    continue;

                //I'm not sure if this works or not
                float? beforeMergeTemperature = null;
                //Deregister the whole net that should be merged (deregister adjacent AirNet)
                foreach ( var current in newComps[layerInt] )
                {
                    //Check for adjacent cells
                    foreach ( var adjPos in GenAdj.CellsAdjacentCardinal( current.parent ) )
                    {
                        if ( !adjPos.InBounds() )
                        {
                            continue;
                        }

                        var oldNet = AirNetGrid.NetAt( adjPos, (NetLayer) layerInt );

                        if ( oldNet != null && beforeMergeTemperature == null )
                        {
                            beforeMergeTemperature = oldNet.NetTemperature;
                            DeregisterAirNet( oldNet );
                        }
                    }
                }


                //I'm not sure if this works or not
                float? beforeSplitTemperature = null;
                //Deregister comps marked as old
                foreach ( var current in oldComps[layerInt] )
                {
                    var oldNet = AirNetGrid.NetAt( current.parent.Position, (NetLayer) layerInt );

                    if ( oldNet != null )
                    {
                        if ( beforeSplitTemperature == null )
                        {
                            beforeSplitTemperature = oldNet.NetTemperature;
                        }
                        DeregisterAirNet( oldNet );
                    }
                }


                //Making a new, merged net
                foreach ( var current in newComps[layerInt] )
                {
                    if ( AirNetGrid.NetAt( current.parent.Position, (NetLayer) layerInt ) == null )
                    {
                        RegisterAirNet( AirNetMaker.NewAirNetStartingFrom( (Building) current.parent,
                                                                           (NetLayer) layerInt,
                                                                           beforeMergeTemperature ) );
                    }
                }

                //Split nets
                foreach ( var current in oldComps[layerInt] )
                {
                    foreach ( var adjPos in GenAdj.CellsAdjacentCardinal( current.parent ) )
                    {
                        if ( !adjPos.InBounds() )
                        {
                            continue;
                        }

                        var airNode = GetAirNodeAt( adjPos, (NetLayer) layerInt );
                        if ( airNode != null )
                        {
                            RegisterAirNet( AirNetMaker.NewAirNetStartingFrom( airNode, (NetLayer) layerInt,
                                                                               beforeSplitTemperature ) );
                        }
                    }
                }

                newComps[layerInt].Clear();
                oldComps[layerInt].Clear();
            }
        }

        private static Building GetAirNodeAt( IntVec3 loc, NetLayer layer )
        {
            var things = Find.ThingGrid.ThingsListAt( loc );
            foreach ( var current in things )
            {
                var compAir = current.TryGetComp< CompAir >();
                if ( compAir == null )
                {
                    continue;
                }

                if ( compAir.IsLayerOf( layer ) )
                {
                    return (Building) current;
                }
            }
            return null;
        }

        //Overlay drawer update
        private static void AddToGraphicUpdateList( ThingComp compAir )
        {
            foreach ( var current in compAir.parent.OccupiedRect() )
                updatees.Add( current );
        }

        public static void UpdateMapDrawer()
        {
            if ( updatees.Count == 0 )
            {
                return;
            }

#if DEBUG
            //Log.Message( "Updated drawer." );
#endif
            foreach ( var current in updatees )
            {
                Find.MapDrawer.MapMeshDirty( current, MapMeshFlag.Things, true, false );
                Find.MapDrawer.MapMeshDirty( current, MapMeshFlag.PowerGrid, true, false );
            }

            Find.MapDrawer.MapMeshDrawerUpdate_First();
            updatees.Clear();
        }
    }
}