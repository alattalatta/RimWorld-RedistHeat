using Verse;

namespace RedistHeat
{
	public class BuildingSmartDuctOutlet : BuildingDuctComp
	{
		protected override bool Validate()
		{
			return (base.Validate() && ValidateTemp(RoomNorth.Temperature, CompAir.ConnectedNet.Temperature));
		}
		private bool ValidateTemp(float roomTemp, float netTemp)
		{
			return ((roomTemp < compTempControl.targetTemperature && roomTemp < netTemp) || (roomTemp > compTempControl.targetTemperature && roomTemp > netTemp));
		}
	}
}