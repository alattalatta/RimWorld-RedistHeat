using UnityEngine;
using Verse;
namespace RedistHeat
{
	public class Graphic_LinkedAirTransmitterOverlay : Graphic_Linked
	{
		public Graphic_LinkedAirTransmitterOverlay(Graphic subGraphic)
			: base(subGraphic)
		{
		}
		public override bool ShouldLinkWith(IntVec3 c, Thing parent)
		{
			return c.InBounds() && AirNetGrid.AirNodeAt(c) != null;
		}
		public override void Print(SectionLayer layer, Thing parent)
		{
			var intRect = parent.OccupiedRect();
			for (var i = intRect.minZ; i <= intRect.maxZ; i++)
			{
				for (var j = intRect.minX; j <= intRect.maxX; j++)
				{
					var cell = new IntVec3(j, 0, i);
					var center = cell.ToVector3ShiftedWithAltitude(AltitudeLayer.WorldDataOverlay);
					Printer_Plane.PrintPlane(layer, center, new Vector2(1f, 1f), LinkedDrawMatFrom(parent, cell), 0f);
				}
			}
		}
	}
}
