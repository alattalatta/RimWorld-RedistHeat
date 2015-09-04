using System.Linq;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_DuctCooler : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            var room = center.GetRoom();
            if ( room == null || room.UsesOutdoorTemperature )
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