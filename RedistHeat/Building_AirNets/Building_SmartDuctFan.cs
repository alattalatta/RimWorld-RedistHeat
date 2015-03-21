namespace RedistHeat
{
	public class Building_SmartDuctFan : Building_DuctComp
	{
		private CompAirController compAir;

		public override void SpawnSetup()
		{
			base.SpawnSetup();
			compAir = GetComp<CompAirController>();
			compAir.roomTemp = ThisRoom.Temperature;
		}
		public override bool Validate()
		{
			return (base.Validate() && ValidateTemp(compAir.roomTemp, compAir.connectedNet.Temperature));
		}
		private bool ValidateTemp(float roomTemp, float netTemp)
		{
			return ((roomTemp < compAir.targetTemperature && roomTemp < netTemp) || (roomTemp > compAir.targetTemperature && roomTemp > netTemp));
		}
	}
}