using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RedistHeat
{
    public static class AirNetManager
    {
        public static List< AirNet >[] allNets;

        //Pending comps
        private static List< CompAir >[] newComps;
        private static List< CompAir >[] oldComps;

        //Pending graphic updates
        private static List< IntVec3 > updatees;

        private static bool isReady;

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
        }

        public static void Reload()
        {
            for ( var i = 0; i < Common.NetLayerCount(); i++ )
            {
                allNets[i] = new List<AirNet>();
                newComps[i] = new List<CompAir>();
                oldComps[i] = new List<CompAir>();
            }

            foreach ( var current in Find.Map.listerBuildings.allBuildingsColonist )
            {
                var compAir = current.TryGetComp< CompAir >();
                if ( compAir != null )
                {
                    newComps[(int) compAir.currentLayer].Add( compAir );
                    updatees.Add( current.Position );
                }
            }
#if DEBUG
            Log.Message("LT-RH: Initialized AirNetManager.");
#endif
            isReady = true;
        }

        public static void NotifyCompSpawn( CompAir compAir )
        {
            if ( !isReady )
                return;

            if ( compAir.IsLayerOf( NetLayer.Lower ) )
            {
                newComps[(int) NetLayer.Lower].Add( compAir );
            }

            else if ( compAir.IsLayerOf( NetLayer.Upper ) )
            {
                newComps[(int) NetLayer.Upper].Add( compAir );
            }
#if DEBUG
            Log.Message("LT-RH: Spawning " + compAir.parent);
#endif

            AddToGraphicUpdateList( compAir );
        }

        public static void NotifyCompDespawn( CompAir compAir )
        {
            if ( !isReady )
                return;

            if ( compAir.IsLayerOf( NetLayer.Lower ) )
            {
                oldComps[(int) NetLayer.Lower].Add( compAir );
            }

            if ( compAir.IsLayerOf( NetLayer.Upper ) )
            {
                oldComps[(int) NetLayer.Upper].Add( compAir );
            }

#if DEBUG
            Log.Message("LT-RH: Despawning " + compAir.parent);
#endif
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
            
            AddToGraphicUpdateList( compAir );
        }

        public static void RegisterAirNet( AirNet newNet )
        {
#if DEBUG
            Log.Message("LT-RH: Registering net " + newNet);
#endif
            allNets[newNet.LayerInt].Add( newNet );
            AirNetGrid.NotifyNetCreated( newNet );
        }

        public static void DeregisterAirNet( AirNet oldNet )
        {
#if DEBUG
            Log.Message("LT-RH: Deregistering net " + oldNet);
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
                if ( !newComps[layerInt].Any() && !oldComps[layerInt].Any() )
                    continue;
                
                //Deregister the whole net that should be merged (deregister adjacent AirNet)
                foreach ( var current in newComps[layerInt] )
                {
                    Log.Message("Cleaning.");
                    //Check for adjacent cells
                    foreach ( var adjPos in GenAdj.CellsAdjacentCardinal( current.parent ) )
                    {
                        if ( !adjPos.InBounds() )
                        {
                            continue;
                        }

                        var oldNet = AirNetGrid.NetAt( adjPos, (NetLayer) layerInt );

                        if ( oldNet != null)
                        {
                            DeregisterAirNet( oldNet );
                        }
                    }
                }

                //Deregister comps marked as old
                foreach ( var current in oldComps[layerInt] )
                {
                    Log.Message("Deleting.");
                    var oldNet = AirNetGrid.NetAt( current.parent.Position, (NetLayer) layerInt );

                    if ( oldNet != null )
                    {
                        DeregisterAirNet( oldNet );
                    }
                }


                //Make a new, merged net
                foreach ( var current in newComps[layerInt] )
                {
                    Log.Message("Merging.");
                    if ( AirNetGrid.NetAt( current.parent.Position, (NetLayer) layerInt ) == null )
                    {
                        RegisterAirNet( AirNetMaker.NewAirNetStartingFrom( (Building) current.parent,
                                                                           (NetLayer) layerInt,
                                                                           GenTemperature.OutdoorTemp ) );
                    }
                }

                //Split nets
                foreach ( var current in oldComps[layerInt] )
                {
                    Log.Message("Splitting.");
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
                                                                               GenTemperature.OutdoorTemp) );
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