using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class PlaceWorkerExhaustPort : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
		{
			var vecNorth = center + IntVec3.North.RotatedBy(rot);
			GenDraw.DrawFieldEdges(new List<IntVec3>() { vecNorth }, Color.white);
			var room = vecNorth.GetRoom();
			if (room == null || room.UsesOutdoorTemperature)
				return;
			GenDraw.DrawFieldEdges(room.Cells.ToList(), GenTemperature.ColorRoomHot);
		}

		public override AcceptanceReport AllowsPlacing(EntityDef def, IntVec3 center, Rot4 rot)
		{
			var vecNorth = center + IntVec3.North.RotatedBy(rot);
			var vecSouth = center + IntVec3.South.RotatedBy(rot);
			if (!vecSouth.InBounds() || !vecNorth.InBounds())
				return false;
			if (vecNorth.Impassable())
				return StaticSet.StringExposeHot;

			var edifice = vecSouth.GetEdifice();
			if (edifice == null || edifice.def != ThingDef.Named("RedistHeat_IndustrialCooler"))
				return StaticSet.StringAttachToCooler;

			return true;
		}
	}
}
