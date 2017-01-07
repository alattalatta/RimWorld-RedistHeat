using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_ActiveVent : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            var vecSouth = center + IntVec3.South.RotatedBy( rot );
            if (!vecNorth.InBounds(this.Map) || !vecSouth.InBounds(this.Map))
            {
                return;
            }

            GenDraw.DrawFieldEdges( new List< IntVec3 >
            {
                vecNorth
            }, new Color( 1f, 0.7f, 0f, 0.5f ) );
            GenDraw.DrawFieldEdges( new List< IntVec3 >
            {
                vecSouth
            }, Color.white );

            var controlledRoom = vecNorth.GetRoom(this.Map);
            var otherRoom = vecSouth.GetRoom(this.Map);

            if (controlledRoom == null || otherRoom == null)
            {
                return;
            }

            if (!controlledRoom.UsesOutdoorTemperature)
            {
                GenDraw.DrawFieldEdges( controlledRoom.Cells.ToList(), new Color( 1f, 0.7f, 0f, 0.5f ) );
            }
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot, Thing thingToIgnore = null)
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            var vecSouth = center + IntVec3.South.RotatedBy( rot );
            if (!vecNorth.InBounds(this.Map) || !vecSouth.InBounds(this.Map))
            {
                return false;
            }
            if (vecNorth.Impassable(this.Map) || vecSouth.Impassable(this.Map))
            {
                return ResourceBank.ExposeBoth;
            }
            return true;
        }
    }
}