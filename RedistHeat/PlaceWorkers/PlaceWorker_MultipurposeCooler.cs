using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_MultipurposeCooler : PlaceWorker_DuctBase
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            base.DrawGhost( def, center, rot, ghostCol);
            var map = Find.CurrentMap;
            var room = center.GetRoom(map);
            if (room == null || room.UsesOutdoorTemperature)
            {
                return;
            }
            Building_MultipurposeCooler t = (Building_MultipurposeCooler)Find.CurrentMap.thingGrid.ThingsListAt(center).Where(s => s.GetType() == typeof(Building_MultipurposeCooler)).ElementAtOrDefault(0);

            if (t != null && !t.Net)
                GenDraw.DrawFieldEdges( room.Cells.ToList(), GenTemperature.ColorRoomCold );
            else
                GenDraw.DrawFieldEdges(room.Cells.ToList(), GenTemperature.ColorRoomHot);
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            return center.InBounds(map);
        }
    }
}