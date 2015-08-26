using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_IndustrialCooler : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            var vecSouth = center + IntVec3.South.RotatedBy( rot );
            var vecSouthEast = vecSouth + IntVec3.East.RotatedBy( rot );
            GenDraw.DrawFieldEdges( new List< IntVec3 >() {vecSouth, vecSouthEast}, GenTemperature.ColorSpotCold );
            var room = vecSouth.GetRoom();
            if ( room == null || room.UsesOutdoorTemperature )
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), GenTemperature.ColorRoomCold );
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot )
        {
            var vecSouth = center + IntVec3.South.RotatedBy( rot );
            var vecSouthEast = vecSouth + IntVec3.East.RotatedBy( rot );
            if ( !vecSouth.InBounds() || !vecSouthEast.InBounds() )
            {
                return false;
            }
            if ( vecSouth.Impassable() || vecSouthEast.Impassable() )
            {
                return ResourceBank.StringExposeCold;
            }
            return true;
        }
    }
}