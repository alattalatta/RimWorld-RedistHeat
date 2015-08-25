using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
    /// <summary>
    /// Manages map-wide air node grid.
    /// </summary>
    public static class AirNetGrid
    {
        private static readonly CompAirBase[] NetGrid;

        static AirNetGrid()
        {
            NetGrid = new CompAirBase[CellIndices.NumGridCells];
        }

        #region Finder

        public static CompAirBase AirNodeAt( IntVec3 pos )
        {
            if ( pos.InBounds() )
            {
                return NetGrid[CellIndices.CellToIndex( pos )];
            }

            Log.Error( "LT-RH: Tried to get AirNode out of bounds: " + pos );
            return null;
        }

        #endregion

        #region Node management

        //Register Base
        public static void Register( CompAirBase comp )
        {
            if ( !comp.Position.InBounds() )
            {
                Log.Warning( comp + " tried to register out of bounds at " + comp.Position + ". Destroying." );
                comp.parent.Destroy();
                return;
            }

            var index = CellIndices.CellToIndex( comp.Position );
            NetGrid[index] = comp;
            AirNetManager.NotifyDrawerForGridUpdate( comp.Position );
        }

        //Deregister Base
        public static void Deregister( CompAirBase comp )
        {
            if ( !comp.Position.InBounds() )
            {
                Log.Error( comp + " tried to de-register out of bounds at " + comp.Position );
            }
            else
            {
                var index = CellIndices.CellToIndex( comp.Position );
                if ( NetGrid[index] == comp )
                {
                    NetGrid[index] = null;
                }
            }
            AirNetManager.NotifyDrawerForGridUpdate( comp.Position );
        }

        #endregion //#region Node management
    }
}