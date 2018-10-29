using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_DuctComp : PlaceWorker_DuctBase
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            base.DrawGhost( def, center, rot, ghostCol);
            var map = Find.CurrentMap;
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            if (!vecNorth.InBounds(map))
            {
                return;
            }

            GenDraw.DrawFieldEdges( new List< IntVec3 > {vecNorth}, new Color( 1f, 0.7f, 0f, 0.5f ) );
            var room = vecNorth.GetRoom(map);
            if (room == null || room.UsesOutdoorTemperature)
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), new Color( 1f, 0.7f, 0f, 0.5f ) );
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            if (!vecNorth.InBounds(map))
            {
                return false;
            }

            if (vecNorth.Impassable(map))
            {
                return ResourceBank.ExposeDuct;
            }

            return true;
        }
    }
}