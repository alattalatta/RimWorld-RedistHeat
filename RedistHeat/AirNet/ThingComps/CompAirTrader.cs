using UnityEngine;

namespace RedistHeat
{
    public class CompAirTrader : CompAir
    {
        public void ExchangeHeatWithNets( float tempEq, float rate )
        {
            var tempDiff = Mathf.Abs( connectedNet.NetTemperature - tempEq );
            var tempRated = tempDiff*rate*props.energyPerSecond;
            if ( tempEq < connectedNet.NetTemperature )
            {
                connectedNet.NetTemperature = Mathf.Max( tempEq, connectedNet.NetTemperature - tempRated );
            }
            else if ( tempEq > connectedNet.NetTemperature )
            {
                connectedNet.NetTemperature = Mathf.Min( tempEq, connectedNet.NetTemperature + tempRated );
            }
        }
    }
}