using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class PlaceWorker_DuctCompNorth : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, IntRot rot)
		{
			var vecNorth = center + IntVec3.north.RotatedBy(rot);
			GenDraw.DrawFieldEdges(new List<IntVec3>() { vecNorth }, new Color(1f, 0.7f, 0f, 0.5f));
			var room = vecNorth.GetRoom();
			if (room == null || room.UsesOutdoorTemperature)
				return;
			GenDraw.DrawFieldEdges(room.Cells.ToList(), new Color(1f, 0.7f, 0f, 0.5f));
		}

		public override AcceptanceReport AllowsPlacing(EntityDef def, IntVec3 center, IntRot rot)
		{
			var vecNorth = center + IntVec3.north.RotatedBy(rot);
			if (!vecNorth.InBounds())
				return false;

			if (vecNorth.Impassable())
				return StaticSet.StringExposeDuct;

			return true;
		}
	}
}
