using Verse;

namespace RedistHeat
{
    public class PlaceWorker_WallChecker : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot )
        {
            var things = Find.Map.thingGrid.ThingsListAt( center );
            if ( things.Exists( s => s is Building_DuctComp || s is Building_Vent ) )
            {
                return ResourceBank.WallAlreadyOccupied;
            }
            return true;
        }
    }
}