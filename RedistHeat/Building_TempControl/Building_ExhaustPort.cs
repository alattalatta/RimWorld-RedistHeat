using System.Text;
using RimWorld;
using Verse;

namespace RedistHeat
{
	public class Building_ExhaustPort : Building_TempControl
	{
		public IntVec3 VecNorth { get; private set; }
		public IntVec3 VecSouth { get; private set; }
		public bool isAvailable;

		private Building_IndustrialCooler neighCooler;

		public override void SpawnSetup()
		{
			base.SpawnSetup();
			VecNorth = Position + IntVec3.north.RotatedBy(Rotation);
			VecSouth = Position + IntVec3.south.RotatedBy(Rotation);
			if ((neighCooler = AdjacentCooler()) == null)
				Log.Message("no cooler found during SpawnSetup().");
		}
		public override void TickRare()
		{
			neighCooler = AdjacentCooler();
			if (compPowerTrader.PowerOn && neighCooler != null && !VecNorth.Impassable())
			{
				isAvailable = true;
				compPowerTrader.powerOutput = -compPowerTrader.props.basePowerConsumption;
			}
			else
			{
				isAvailable = false;
				compPowerTrader.powerOutput = -compPowerTrader.props.basePowerConsumption * 0.1f;
			}
		}
		public override string GetInspectString()
		{
			var str = new StringBuilder();
			str.AppendLine(base.GetInspectString());
			str.Append(StaticSet.StringState + " ");

			str.Append(isAvailable ? StaticSet.StringWorking : StaticSet.StringNotWorking);

			return str.ToString();
		}

		private Building_IndustrialCooler AdjacentCooler()
		{
			var finder = Find.ThingGrid.ThingAt<Building_IndustrialCooler>(VecSouth);
			return finder;
		}
	}
}