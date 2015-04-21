using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
	public class CompAirIntersection : CompAirBase
	{
		public override void CompPrintForPowerGrid(SectionLayer layer)
		{
			AirOverlayMat.LinkedOverlayGraphic.Print(layer, parent);
		}
	}
}
