namespace RedistHeat
{
    public class Building_SmartDuctOutlet : Building_DuctComp
    {
        private bool isWorking;

        public override void TickRare()
        {
            base.TickRare();
        }

        protected override bool Validate()
        {
            isWorking = (base.Validate() && ValidateTemp( roomNorth.Temperature, compAir.connectedNet.NetTemperature ));
            return isWorking;
        }

        private bool ValidateTemp( float roomTemp, float netTemp )
        {
            return ((roomTemp < compTempControl.targetTemperature && roomTemp < netTemp) ||
                    (roomTemp > compTempControl.targetTemperature && roomTemp > netTemp));
        }
    }
}