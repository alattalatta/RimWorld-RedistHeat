using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
	public static class AirNetGrid
	{
		private static List<CompAirBase>[] _netGrid;
		static AirNetGrid()
		{
			_netGrid = new List<CompAirBase>[CellIndices.NumGridCells];
			foreach (var current in CellIndices.AllCellIndicesOnMap)
			{
				_netGrid[current] = new List<CompAirBase>();
			}
		}

		#region Node management
		//Register Base
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
				if (_netGrid[index] == null)
					_netGrid[index] = new List<CompAirBase>();
				_netGrid[index].Add(comp);
			}
			AirNetManager.NotifyDrawerForGridUpdate(comp.Position);
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
				if (_netGrid[index].Contains(comp))
				{
					_netGrid[index].Remove(comp);
				}
			}
			AirNetManager.NotifyDrawerForGridUpdate(comp.Position);
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
				result = _netGrid[CellIndices.CellToIndex(pos)];
			}
			return result;
		}
		public static CompAirBase AirNodeAt(IntVec3 c)
		{
			return AirNodeListAt(c).Find(s => true);
		}
		#endregion //#region Finders
	}
}
