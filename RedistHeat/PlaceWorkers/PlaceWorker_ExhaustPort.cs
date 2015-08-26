using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_ExhaustPort : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            GenDraw.DrawFieldEdges( new List< IntVec3 >() {vecNorth}, Color.white );
            var room = vecNorth.GetRoom();
            if ( room == null || room.UsesOutdoorTemperature )
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), GenTemperature.ColorRoomHot );
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot )
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            var vecSouth = center + IntVec3.South.RotatedBy( rot );
            if ( !vecSouth.InBounds() || !vecNorth.InBounds() )
            {
                return false;
            }
            if ( vecNorth.Impassable() )
            {
                return ResourceBank.StringExposeHot;
            }

            var edifice = vecSouth.GetEdifice();
            if ( edifice == null || edifice.def != ThingDef.Named( "RedistHeat_IndustrialCooler" ) )
            {
                return ResourceBank.StringAttachToCooler;
            }

            return true;
        }
    }
}