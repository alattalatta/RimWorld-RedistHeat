using System.Linq;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_DuctCooler : PlaceWorker_DuctBase
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            base.DrawGhost( def, center, rot );

            var room = center.GetRoom();
            if (room == null || room.UsesOutdoorTemperature)
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), GenTemperature.ColorRoomHot );
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot )
        {
            return center.InBounds();
        }
    }
}