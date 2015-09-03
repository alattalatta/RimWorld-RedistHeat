using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_ActiveVent : Building_Vent
    {
        protected override void Equalize( Room room, float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( room.Temperature - targetTemp );
            var tempSet = compTempControl.targetTemperature;
            var tempRated = tempDiff*rate;
            if ( targetTemp < room.Temperature )
            {
                room.Temperature = Mathf.Max( targetTemp, room.Temperature - tempRated, tempSet );
            }
            else if ( targetTemp > room.Temperature )
            {
                room.Temperature = Mathf.Min( targetTemp, room.Temperature + tempRated, tempSet );
            }
        }

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