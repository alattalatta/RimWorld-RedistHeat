using Verse;

namespace RedistHeat
{
    public static class AirNetGrid
    {
        private static AirNet[][] netGrid;

        static AirNetGrid()
        {
            var layerCount = Common.NetLayerCount();
            netGrid = new AirNet[layerCount][];

            for ( var i = 0; i < layerCount; i++ )
            {
                netGrid[i] = new AirNet[CellIndices.NumGridCells];
            }

            if(Prefs.LogVerbose)
                Log.Message( "LT-RH: Initialized NetGrid." );
        }

        /*
        public static void Reinit()
        {
            var layerCount = Common.NetLayerCount();
            netGrid = new AirNet[layerCount][];

            for ( var i = 0; i < layerCount; i++ )
            {
                netGrid[i] = new AirNet[CellIndices.NumGridCells];
            }
        }*/

        public static AirNet NetAt( IntVec3 pos, NetLayer layer )
        {
            return netGrid[(int)layer][CellIndices.CellToIndex( pos )];
        }

        public static Building GetAirTransmitter( this IntVec3 loc, NetLayer layer )
        {
            foreach ( var current in Find.ThingGrid.ThingsListAt( loc ) )
            {
                var compAir = current.TryGetComp< CompAir >();
                if ( compAir == null )
                    continue;

                if ( compAir.IsLayerOf( layer ) )
                    return (Building) current;
            }
            return null;
        }

        public static void NotifyNetCreated( AirNet newNet )
        {
            foreach ( var node in newNet.nodes )
            {
                //For every cell occupied by a node
                var occupiedRect = node.parent.OccupiedRect();
                foreach ( var current in occupiedRect )
                {
                    //Register the cell as the new net
                    netGrid[newNet.LayerInt][CellIndices.CellToIndex( current )] = newNet;
                }
            }
        }

        public static void NotifyNetDeregistered( AirNet oldNet )
        {
            foreach ( var node in oldNet.nodes )
            {
                //For every cell occupied by a node
                var occupiedRect = node.parent.OccupiedRect();
                foreach ( var current in occupiedRect )
                {
                    //Delete the cell's registered net
                    netGrid[oldNet.LayerInt][CellIndices.CellToIndex( current )] = null;
                }
            }
        }
    }
}