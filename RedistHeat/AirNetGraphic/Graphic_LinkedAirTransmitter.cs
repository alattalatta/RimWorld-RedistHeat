using UnityEngine;
using Verse;
namespace RedistHeat
{
	public class GraphicLinkedAirTransmitter : Graphic_Linked
	{
		public GraphicLinkedAirTransmitter(Graphic subGraphic)
			: base(subGraphic)
		{
		}
		public override bool ShouldLinkWith(IntVec3 c, Thing parent)
		{
			return c.InBounds() && AirNetGrid.AirNodeAt(c) != null;
		}
		public override void Print(SectionLayer layer, Thing thing)
		{
			base.Print(layer, thing);
			for (var i = 0; i < 4; i++)
			{
				var neighCell = thing.Position + GenAdj.CardinalDirections[i];
				if (!neighCell.InBounds()) continue;

				var transmitter = AirNetGrid.AirNodeAt(neighCell);
				if (transmitter == null || transmitter.parent.def.graphicData.Linked) continue;

				var mat = LinkedDrawMatFrom(thing, neighCell);
				Printer_Plane.PrintPlane(layer, neighCell.ToVector3ShiftedWithAltitude(thing.def.Altitude), Vector2.one, mat, 0f);
			}
		}
	}
}
