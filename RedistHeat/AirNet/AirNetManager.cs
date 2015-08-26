using System.Collections.Generic;
using System.Runtime.InteropServices;
using RimWorld;
using Verse;

namespace RedistHeat
{
    public static class AirNetManager
    {
        //Lists
        private static List< AirNet >[] allNets;
        private static List< CompAir >[] newComps;
        private static List< CompAir >[] oldComps;

        public static void Reinit()
        {
            var layerCount = Common.NetLayerCount();
            allNets = new List< AirNet >[layerCount];
            newComps = new List< CompAir >[layerCount];
            oldComps = new List< CompAir >[layerCount];

            for ( var i = 0; i < layerCount; i++ )
            {
                allNets[i] = new List< AirNet >();
                allNets[i].Clear();
                newComps[i] = new List< CompAir >();
                newComps[i].Clear();
                oldComps[i] = new List< CompAir >();
                oldComps[i].Clear();
            }
        }

        public static void NotifyCompSpawn( CompAir compAir )
        {
            if ( compAir.IsLayerOf( NetLayer.Lower ) )
                newComps[(int) NetLayer.Lower].Add( compAir );
            if ( compAir.IsLayerOf( NetLayer.Upper ) )
                newComps[(int) NetLayer.Upper].Add( compAir );

            NotifyDrawersForGridUpdate( compAir );
        }

        public static void NotifyCompDespawn( CompAir compAir )
        {
            if ( compAir.IsLayerOf(NetLayer.Lower) )
                oldComps[(int)NetLayer.Lower].Add(compAir);
            if ( compAir.IsLayerOf(NetLayer.Upper) )
                oldComps[(int)NetLayer.Upper].Add(compAir);

            NotifyDrawersForGridUpdate( compAir );
        }

        public static void NotifyCompLayerChange( CompAir compAir, NetLayer oldLayer )
        {
            if ( oldLayer == compAir.currentLayer )
            {
                Log.Error( "LT-RH: Tried to change " + compAir + "\'s layer to " + compAir.currentLayer + ", which is not different!" );
                return;
            }

            oldComps[(int) oldLayer].Add( compAir );
            newComps[(int) compAir.currentLayer].Add( compAir );

            NotifyDrawersForGridUpdate(compAir);
        }

        public static void RegisterAirNet( AirNet newNet )
        {
            allNets[newNet.LayerInt].Add( newNet );
            AirNetGrid.NotifyNetCreated( newNet );
        }

        public static void DeregisterAirNet( AirNet oldNet )
        {
            allNets[oldNet.LayerInt].Remove( oldNet );
            AirNetGrid.NotifyNetDeregistered( oldNet );
        }

        public static void AirNetsTick()
        {
            foreach ( var layer in allNets )
            {
                foreach ( var net in layer )
                {
                    foreach ( var current in allNets )
                        net.AirNetTick();
                }
            }
        }

        public static void AirNetsUpdate()
        {
            for ( var layerInt = 0; layerInt < Common.NetLayerCount(); layerInt++ )
            {
                float? beforeMergeTemperature = null;
                //Deregister the whole net that should be merged
                foreach ( var current in newComps[layerInt] )
                {
                    //Check for adjacent cells
                    foreach ( var adjPos in GenAdj.CellsAdjacentCardinal( current.parent ) )
                    {
                        if ( !adjPos.InBounds() )
                        {
                            continue;
                        }

                        var oldNode = AirNetGrid.NetAt( adjPos, (NetLayer) layerInt );
                        if ( beforeMergeTemperature == null )
                        {
                            beforeMergeTemperature = oldNode.NetTemperature;
                        }

                        if ( oldNode != null )
                        {
                            DeregisterAirNet( oldNode );
                        }
                    }
                }

                float? beforeSplitTemperature = null;
                //Deregister comps marked as old
                foreach ( var current in oldComps[layerInt] )
                {
                    var oldNode = AirNetGrid.NetAt( current.parent.Position, (NetLayer) layerInt );

                    if ( oldNode != null )
                    {
                        if ( beforeSplitTemperature == null )
                        {
                            beforeSplitTemperature = oldNode.NetTemperature;
                        }
                        DeregisterAirNet( oldNode );
                    }
                }

                //Making a new, merged net
                foreach ( var current in newComps[layerInt] )
                {
                    if ( AirNetGrid.NetAt( current.parent.Position, (NetLayer) layerInt ) == null )
                    {
                        RegisterAirNet( AirNetMaker.NewAirNetStartingFrom( (Building) current.parent, (NetLayer) layerInt, beforeMergeTemperature ) );
                    }
                }

                //Split nets
                foreach ( var current in oldComps[layerInt] )
                {
                    foreach ( var adjPos in GenAdj.CellsAdjacentCardinal( current.parent ) )
                    {
                        if ( !adjPos.InBounds() )
                            continue;

                        var airNode = GetAirNodeAt( adjPos, (NetLayer) layerInt );
                        if ( airNode != null )
                        {
                            RegisterAirNet( AirNetMaker.NewAirNetStartingFrom( airNode, (NetLayer) layerInt, beforeSplitTemperature ) );
                        }
                    }
                }

                foreach ( var current in oldComps[layerInt] )
                {
                    current.ResetAirVars();
                }

                newComps[layerInt].Clear();
                oldComps[layerInt].Clear();
            }
        }

        private static Building GetAirNodeAt(IntVec3 loc, NetLayer layer)
        {
            var things = Find.ThingGrid.ThingsListAt( loc );
            foreach ( var current in things )
            {
                var compAir = current.TryGetComp< CompAir >();
                if ( compAir == null )
                    continue;

                if ( compAir.IsLayerOf( layer ) )
                    return (Building) current;
            }
            return null;
        }
        
        //Overlay drawer update
        public static void NotifyDrawersForGridUpdate( ThingComp compAir )
        {
            Find.MapDrawer.MapMeshDirty( compAir.parent.Position, MapMeshFlag.PowerGrid, true, false );
        }
    }
}