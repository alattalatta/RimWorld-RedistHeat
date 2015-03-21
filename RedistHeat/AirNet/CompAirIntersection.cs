using Verse;

namespace RedistHeat
{
	public class CompAirIntersection : ThingComp
	{
		public override void CompPrintForPowerGrid(SectionLayer layer)
		{
			AirOverlayMat.LinkedOverlayGraphic.Print(layer, parent);
		}
	}
}
