using RimWorld;
using Verse;

namespace RedistHeat
{
	public class BuildingActiveVent : BuildingVent
	{
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			compPowerTrader = GetComp<CompPowerTrader>();
			roomNorth = (Position + IntVec3.North.RotatedBy(Rotation)).GetRoom();
			roomSouth = (Position + IntVec3.South.RotatedBy(Rotation)).GetRoom();
		}
		protected override bool Validate()
		{
			return (base.Validate() && compPowerTrader.PowerOn && ValidateTemp(roomNorth.Temperature, roomSouth.Temperature));
		}
		private bool ValidateTemp(float controlled, float other)
		{
			return ((controlled < compTempControl.targetTemperature && controlled < other) ||
			        (controlled > compTempControl.targetTemperature && controlled > other));
		}
	}
}