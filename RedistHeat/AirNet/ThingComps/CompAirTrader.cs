using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAirTrader : CompAir
    {
        public void EqualizeWithRoom( Room room, float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( room.Temperature - targetTemp );
            var tempRated = tempDiff*rate*(1-props.energyPerSecond);

            if ( targetTemp < room.Temperature )
            {
                room.Temperature = Mathf.Max(targetTemp, room.Temperature - tempRated);
            }
            else if ( targetTemp > room.Temperature )
            {
                room.Temperature = Mathf.Min(targetTemp, room.Temperature + tempRated);
            }
        }

        public void EqualizeWithNet( float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( connectedNet.NetTemperature - targetTemp );
            var tempRated = tempDiff*rate*props.energyPerSecond;

            if ( targetTemp < connectedNet.NetTemperature )
            {
                connectedNet.NetTemperature = Mathf.Max( targetTemp, connectedNet.NetTemperature - tempRated );
            }
            else if ( targetTemp > connectedNet.NetTemperature )
            {
                connectedNet.NetTemperature = Mathf.Min( targetTemp, connectedNet.NetTemperature + tempRated );
            }
        }
    }
}