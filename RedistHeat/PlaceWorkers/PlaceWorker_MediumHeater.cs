using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_MediumHeater : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            if (!vecNorth.InBounds(base.Map))
            {
                return;
            }

            GenDraw.DrawFieldEdges( new List< IntVec3 >() {vecNorth}, GenTemperature.ColorRoomHot );
            var room = vecNorth.GetRoom(base.Map);
            if (room == null || room.UsesOutdoorTemperature)
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), GenTemperature.ColorRoomHot );
        }
    }
}