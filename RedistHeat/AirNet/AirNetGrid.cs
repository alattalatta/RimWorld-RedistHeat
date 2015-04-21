using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
	public static class AirNetGrid
	{
		private static List<CompAirBase>[] _netGrid;
		static AirNetGrid()
		{
			AirNetGrid._netGrid = new List<CompAirBase>[CellIndices.NumGridCells];
			foreach (var current in CellIndices.AllCellIndicesOnMap)
			{
				AirNetGrid._netGrid[current] = new List<CompAirBase>();
			}
		}

		#region Node management
		//Register CompAirBase
		public static void Register(CompAirBase comp)
		{
			if (!comp.Position.InBounds())
			{
				Log.Warning(comp + " tried to register out of bounds at " + comp.Position + ". Destroying.");
				comp.parent.Destroy();
			}
			else
			{
				var index = CellIndices.CellToIndex(comp.Position);
				if (AirNetGrid._netGrid[index] == null)
					AirNetGrid._netGrid[index] = new List<CompAirBase>();
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
		//Deregister Base
		public static void Deregister(CompAirBase comp)
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
			NotifyDrawerForGridUpdate(comp.Position);
		}

		//Overlay drawer update
		private static void NotifyDrawerForGridUpdate(IntVec3 pos)
		{
			Find.MapDrawer.MapChanged(pos, MapChangeType.PowerGrid, true, false);
		}
		#endregion //#region Node management

		#region Finders

		private static List<CompAirBase> AirNodeListAt(IntVec3 pos)
		{
			List<CompAirBase> result;
			if (!pos.InBounds())
			{
				Log.Error("Got ThingsListAt out of bounds: " + pos);
				result = new List<CompAirBase>();
			}
			else
			{
				result = AirNetGrid._netGrid[CellIndices.CellToIndex(pos)];
			}
			return result;
		}
		public static CompAirBase AirNodeAt(IntVec3 c)
		{
			return AirNetGrid.AirNodeListAt(c).Find(s => true);
		}
		#endregion //#region Finders
	}
}
