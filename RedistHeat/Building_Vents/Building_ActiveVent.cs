using RimWorld;

namespace RedistHeat
{
    public class Building_ActiveVent : Building_Vent
    {
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            compPowerTrader = GetComp< CompPowerTrader >();
        }

        protected override bool Validate()
        {
            if ( compPowerTrader == null)
                return false;

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