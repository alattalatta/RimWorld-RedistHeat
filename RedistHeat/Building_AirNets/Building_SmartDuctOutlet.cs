namespace RedistHeat
{
    public class Building_SmartDuctOutlet : Building_DuctComp
    {
        protected override bool Validate()
        {
            var sourceNet = compAir.connectedNet[(int) compAir.currentLayer];
            return (base.Validate() && ValidateTemp( roomNorth.Temperature, sourceNet.NetTemperature ));
        }

        private bool ValidateTemp( float roomTemp, float netTemp )
        {
            return ((roomTemp < compTempControl.targetTemperature && roomTemp < netTemp) ||
                    (roomTemp > compTempControl.targetTemperature && roomTemp > netTemp));
        }
    }
}