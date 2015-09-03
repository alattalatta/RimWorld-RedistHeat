namespace RedistHeat
{
    public class Building_SmartDuctOutlet : Building_DuctComp
    {

        protected override bool Validate()
        {
            if ( !base.Validate() )
                return false;

            return ValidateTemp( room.Temperature, compAir.connectedNet.NetTemperature );
        }

        private bool ValidateTemp( float roomTemp, float netTemp )
        {
            return ((roomTemp < compTempControl.targetTemperature && roomTemp < netTemp) ||
                    (roomTemp > compTempControl.targetTemperature && roomTemp > netTemp));
        }
    }
}