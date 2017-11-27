using System.Linq;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_DuctCooler : PlaceWorker_DuctBase
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            base.DrawGhost( def, center, rot );
            var map = Find.VisibleMap;
            var room = center.GetRoom(map);
            if (room == null || room.UsesOutdoorTemperature)
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), GenTemperature.ColorRoomHot );
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            return center.InBounds(map);
        }
    }
}