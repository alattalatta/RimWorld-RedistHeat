using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RedistHeat
{
    public static class AirNetMaker
    {
        private static HashSet< Building > closedSet  = new HashSet< Building >();
        private static HashSet< Building > openSet    = new HashSet< Building >();
        private static HashSet< Building > currentSet = new HashSet< Building >();
        
        private static IEnumerable< CompAir > ContiguousAirBuildings( Building root, NetLayer layer )
        {
            closedSet.Clear();
            currentSet.Clear();
            openSet.Add( root );

            do
            {
                //Move all opened to closed
                foreach ( var current in openSet )
                {
                    closedSet.Add( current );
                }

                var tempSet = currentSet;
                currentSet = openSet;
                openSet = tempSet; //Why should do swapping?
                openSet.Clear();

                foreach ( var things in currentSet.SelectMany( openBuilding =>
                                                                   GenAdj.CellsAdjacentCardinal( openBuilding )
                                                                         .Select( openCells => openCells.GetThingList() ) ) )
                {
                    //All adjacent things
                    foreach ( var current in things )
                    {
                        var building = current as Building;
                        var compAir = building?.TryGetComp< CompAir >();

                        //No adjacent CompAir
                        if ( compAir == null )
                        {
                            continue;
                        }
                        //CompAir is not on the same layer
                        if ( !compAir.IsLayerOf( layer ) )
                        {
                            continue;
                        }
                        //Already swept through
                        if ( openSet.Contains( building ) ||
                             currentSet.Contains( building ) ||
                             closedSet.Contains( building ) )
                        {
                            continue;
                        }

                        openSet.Add( building );
                        break;
                    }
                }
            } while ( openSet.Count > 0 );

            return from b in closedSet
                   select b.TryGetComp< CompAir >();
        }

        public static AirNet NewAirNetStartingFrom( Building root, NetLayer layer, float? temperature )
        {
            var temp = temperature ?? GenTemperature.OutdoorTemp;

            return new AirNet( ContiguousAirBuildings( root, layer ), layer, temp, root.TryGetComp<CompAir>() );
        }
    }
}
