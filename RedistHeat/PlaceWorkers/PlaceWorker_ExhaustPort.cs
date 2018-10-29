using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_ExhaustPort : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            var map = Find.CurrentMap;
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            if (!vecNorth.InBounds(map))
            {
                return;
            }

            GenDraw.DrawFieldEdges( new List< IntVec3 >() {vecNorth}, Color.white );
            var room = vecNorth.GetRoom(map);
            if (room == null || room.UsesOutdoorTemperature)
            {
                return;
            }
            GenDraw.DrawFieldEdges( room.Cells.ToList(), GenTemperature.ColorRoomHot );
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            var vecNorth = center + IntVec3.North.RotatedBy( rot );
            var vecSouth = center + IntVec3.South.RotatedBy( rot );
            var vecEast = center + IntVec3.East.RotatedBy(rot);
            var vecWest = center + IntVec3.West.RotatedBy(rot);
            /*if (!vecSouth.InBounds(map) || !vecNorth.InBounds(map))
            {
                return false;
            }
            if (vecNorth.Impassable(map))
            {
                return ResourceBank.ExposeHot;
            }

            var edifice = vecSouth.GetEdifice(map);
            if (edifice == null || edifice.def != ThingDef.Named( "RedistHeat_IndustrialCooler" ))
            {
                return ResourceBank.AttachToCooler;
            }*/
            if (!vecNorth.InBounds(map))
            {
                return false;
            }
            if (vecNorth.Impassable(map))
            {
                return ResourceBank.ExposeHot;
            }

            var edifice = vecSouth.GetEdifice(map);
            if (edifice == null || edifice.def != ThingDef.Named("RedistHeat_IndustrialCooler"))
            {
                edifice = vecEast.GetEdifice(map);
                if (edifice == null || edifice.def != ThingDef.Named("RedistHeat_IndustrialCooler"))
                {
                    edifice = vecWest.GetEdifice(map);
                    if (edifice == null || edifice.def != ThingDef.Named("RedistHeat_IndustrialCooler"))
                    {
                        return ResourceBank.AttachToCooler;
                    }
                }
            }

            return true;
        }
    }
}