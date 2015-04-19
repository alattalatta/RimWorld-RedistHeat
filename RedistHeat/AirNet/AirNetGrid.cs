using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
	public static class AirNetGrid
	{
		private static List<CompAir>[] _netGrid;
		static AirNetGrid()
		{
			AirNetGrid._netGrid = new List<CompAir>[CellIndices.NumGridCells];
			foreach (var current in CellIndices.AllCellIndicesOnMap)
			{
				AirNetGrid._netGrid[current] = new List<CompAir>();
			}
		}

		#region Node management
		//Register
		public static void Register(CompAir comp)
		{
			if (!comp.Position.InBounds())
			{
				Log.Warning(comp + " tried to register out of bounds at " + comp.Position + ". Destroying.");
				comp.parent.Destroy();
			}
			else
			{
				var index = CellIndices.CellToIndex(comp.Position);
				if(AirNetGrid._netGrid[index] == null)
					AirNetGrid._netGrid[index] = new List<CompAir>();
				AirNetGrid._netGrid[index].Add(comp);
			}
			NotifyDrawerForGridUpdate(comp.Position);
		}

		//Deregister
		public static void Deregister(CompAir comp)
		{
			if (!comp.Position.InBounds())
			{
				Log.Error(comp + " tried to de-register out of bounds at " + comp.Position);
			}
			else
			{
				var index = CellIndices.CellToIndex(comp.Position);
				if (AirNetGrid._netGrid[index].Contains(comp))
				{
					AirNetGrid._netGrid[index].Remove(comp);
				}
			}
			if (AirNetGrid.AirNodeAt(comp.Position) == null)
			{
				comp.ConnectedNet.SplitNetAt(comp);
			}
			NotifyDrawerForGridUpdate(comp.Position);
		}

		//Overlay drawer update
		private static void NotifyDrawerForGridUpdate(IntVec3 pos)
		{
			Find.MapDrawer.MapChanged(pos, MapChangeType.PowerGrid, true, false);
		}
		#endregion

		#region Finders
		//AirNodeListAt: public for future use
		// ReSharper disable once MemberCanBePrivate.Global
		public static List<CompAir> AirNodeListAt(IntVec3 pos)
		{
			List<CompAir> result;
			if (!pos.InBounds())
			{
				Log.Error("Got ThingsListAt out of bounds: " + pos);
				result = new List<CompAir>();
			}
			else
			{
				result = AirNetGrid._netGrid[CellIndices.CellToIndex(pos)];
			}
			return result;
		}
		public static CompAir AirNodeAt(IntVec3 c)
		{
			return AirNetGrid.AirNodeListAt(c).Find(s => true);
		}
		#endregion
	}
}
