namespace RedistHeat
{
    public class Building_ActiveVent : Building_Vent
    {
        protected override bool Validate()
        {
            if ( compPowerTrader == null )
            {
                return true;
            }

            return (base.Validate() && compPowerTrader.PowerOn &&
                    ValidateTemp( roomNorth.Temperature, roomSouth.Temperature ));
        }

        private bool ValidateTemp( float controlled, float other )
        {
            return ((controlled < compTempControl.targetTemperature && controlled < other) ||
                    (controlled > compTempControl.targetTemperature && controlled > other));
        }
    }
}