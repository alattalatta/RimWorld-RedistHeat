using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class PlaceWorker_MediumHeater : PlaceWorker
	{
		public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
		{
			var vecNorth = center + IntVec3.North.RotatedBy(rot);
			if (!vecNorth.InBounds())
			{
				return;
			}

			GenDraw.DrawFieldEdges(new List<IntVec3>() { vecNorth }, Color.white);
			var room = vecNorth.GetRoom();
			if (room == null || room.UsesOutdoorTemperature)
			{
				return;
			}
			GenDraw.DrawFieldEdges(room.Cells.ToList(), GenTemperature.ColorRoomHot);
		}
	}
}
