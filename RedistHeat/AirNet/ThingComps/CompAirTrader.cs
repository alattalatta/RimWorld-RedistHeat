using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAirTrader : CompAir
    {
        public void EqualizeWithNets( float targetTemp, float rate )
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