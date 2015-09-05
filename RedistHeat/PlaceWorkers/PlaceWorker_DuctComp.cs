using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_DuctComp : PlaceWorker_DuctBase
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            base.DrawGhost( def, center, rot );

            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            if ( !vecNorth.InBounds() )
            {
                return;
            }

            GenDraw.DrawFieldEdges( new List< IntVec3 > {vecNorth}, new Color( 1f, 0.7f, 0f, 0.5f ) );
            var room = vecNorth.GetRoom();
            if ( room == null || room.UsesOutdoorTemperature )
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), new Color( 1f, 0.7f, 0f, 0.5f ) );
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot )
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            if ( !vecNorth.InBounds() )
            {
                return false;
            }

            if ( vecNorth.Impassable() )
            {
                return ResourceBank.ExposeDuct;
            }

            return true;
        }
    }
}