using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
	public static class AirNetGrid
	{
		private static List<CompAir>[] netGrid;
		static AirNetGrid()
		{
			AirNetGrid.netGrid = new List<CompAir>[CellIndices.NumGridCells];
			foreach (var current in CellIndices.AllCellIndicesOnMap)
			{
				AirNetGrid.netGrid[current] = new List<CompAir>();
			}
		}
		private static void RegisterInCell(CompAir comp, IntVec3 c)
		{
			//Log.Message("AirNetGrid.RegisterInCell");
			if (!c.InBounds())
			{
				Log.Warning(string.Concat(new object[]
				{
					comp,
					" tried to register out of bounds at ",
					c,
					". Destroying."
				}));
				comp.parent.Destroy();
			}
			else
			{
				AirNetGrid.netGrid[CellIndices.CellToIndex(c)].Add(comp);
			}
		}
		private static void DeregisterInCell(CompAir comp, IntVec3 c)
		{
			//Log.Message("AirNetGrid.DeregisterInCell");
			if (!c.InBounds())
			{
				Log.Error(comp + " tried to de-register out of bounds at " + c);
			}
			else
			{
				var num = CellIndices.CellToIndex(c);
				if (AirNetGrid.netGrid[num].Contains(comp))
				{
					AirNetGrid.netGrid[num].Remove(comp);
				}
			}
		}
		public static void Register(CompAir comp)
		{
			AirNetGrid.RegisterInCell(comp, comp.Position);
		}
		public static void Deregister(CompAir comp)
		{
			AirNetGrid.DeregisterInCell(comp, comp.Position);
		}
		public static List<CompAir> AirListAt(IntVec3 c)
		{
			List<CompAir> result;
			if (!c.InBounds())
			{
				Log.Error("Got ThingsListAt out of bounds: " + c);
				result = new List<CompAir>();
			}
			else
			{
				result = AirNetGrid.netGrid[CellIndices.CellToIndex(c)];
			}
			return result;
		}
		public static IEnumerable<CompAir> AirAt(IntVec3 c)
		{
			foreach (var current in AirNetGrid.AirListAt(c))
			{
				yield return current;
			}
		}
		public static CompAir AirNodeAt(IntVec3 c)
		{
			return AirNetGrid.AirListAt(c).Find(s => true);
		}
		public static IEnumerable<CompAirTrader> AirTradersAt(IntVec3 c)
		{
			foreach (var current in AirNetGrid.AirListAt(c))
			{
				var compAirTrader = current as CompAirTrader;
				if (compAirTrader != null)
				{
					yield return compAirTrader;
				}
			}
		}
	}
}
