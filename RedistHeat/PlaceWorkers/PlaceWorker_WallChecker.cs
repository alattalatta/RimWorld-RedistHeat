using System.Linq;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_WallChecker : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot, Thing thingToIgnore = null)
        {
            //var things = Find.Maps.Cast<Map>().SingleOrDefault(i => i == base.Map).thingGrid.ThingsListAt( center );
            var things = base.Map.thingGrid.ThingsListAt(center);
            if (things.Exists( s => s is IWallAttachable ))
            {
                return ResourceBank.WallAlreadyOccupied;
            }
            return true;
        }
    }
}