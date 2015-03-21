using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class PlaceWorker_ActiveVent : PlaceWorker
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

			var controlledRoom = vecNorth.GetRoom();
			var otherRoom = vecSouth.GetRoom();
			if (controlledRoom == null && otherRoom == null)
				return;

			if (controlledRoom == otherRoom && !controlledRoom.UsesOutdoorTemperature)
			{
				GenDraw.DrawFieldEdges(controlledRoom.Cells.ToList(), new Color(1f, 0.7f, 0f, 0.5f));
			}
			else if (!controlledRoom.UsesOutdoorTemperature)
			{
				GenDraw.DrawFieldEdges(controlledRoom.Cells.ToList(), new Color(1f, 0.7f, 0f, 0.5f));
			}
		}
		public override AcceptanceReport AllowsPlacing(EntityDef def, IntVec3 center, IntRot rot)
		{
			var vecNorth = center + IntVec3.north.RotatedBy(rot);
			var vecSouth = center + IntVec3.south.RotatedBy(rot);
			if (!vecNorth.InBounds() || !vecSouth.InBounds())
				return false;
			if (vecNorth.Impassable() || vecSouth.Impassable())
			{
				return StaticSet.StringExposeBoth;
			}
			return true;
		}
	}
}
