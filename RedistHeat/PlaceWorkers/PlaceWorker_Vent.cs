using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class PlaceWorker_Vent : PlaceWorker
	{
		public override void DrawGhost(ThingDef def, IntVec3 center, IntRot rot)
		{
			var vecNorth = center + IntVec3.north.RotatedBy(rot);
			var vecSouth = center + IntVec3.south.RotatedBy(rot);
			GenDraw.DrawFieldEdges(new List<IntVec3>
			{
				vecSouth
			}, new Color(1f, 0.7f, 0f, 0.5f));
			GenDraw.DrawFieldEdges(new List<IntVec3>
			{
				vecNorth
			}, new Color(1f, 0.7f, 0f, 0.5f));
			var room = vecNorth.GetRoom();
			var room2 = vecSouth.GetRoom();

			if (room == null || room2 == null)
				return;

			if (room == room2 && !room.UsesOutdoorTemperature)
			{
				return;
			}
			if (!room.UsesOutdoorTemperature)
			{
				GenDraw.DrawFieldEdges(room.Cells.ToList(), new Color(1f, 0.7f, 0f, 0.5f));
			}
			if (!room2.UsesOutdoorTemperature)
			{
				GenDraw.DrawFieldEdges(room2.Cells.ToList(), new Color(1f, 0.7f, 0f, 0.5f));
			}
		}
		public override AcceptanceReport AllowsPlacing(EntityDef def, IntVec3 center, IntRot rot)
		{
			var vecNorth = center + IntVec3.north.RotatedBy(rot);
			var vecSouth = center + IntVec3.south.RotatedBy(rot);
			if (!vecNorth.InBounds() || !vecSouth.InBounds())
				return false;
			if (vecSouth.Impassable() || vecNorth.Impassable())
			{
				return StaticSet.StringExposeBoth;
			}
			return true;
		}
	}
}
