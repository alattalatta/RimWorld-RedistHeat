using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RedistHeat
{
	public class PlaceWorker_ExhaustPort : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, IntRot rot)
		{
			var vecNorth = center + IntVec3.north.RotatedBy(rot);
			GenDraw.DrawFieldEdges(new List<IntVec3>() { vecNorth }, GenTemperature.ColorSpotHot);
			var room = vecNorth.GetRoom();
			if (room == null || room.UsesOutdoorTemperature)
				return;
			GenDraw.DrawFieldEdges(room.Cells.ToList(), GenTemperature.ColorRoomHot);
		}

		public override AcceptanceReport AllowsPlacing(EntityDef def, IntVec3 center, IntRot rot)
		{
			var vecNorth = center + IntVec3.north.RotatedBy(rot);
			var vecSouth = center + IntVec3.south.RotatedBy(rot);
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
