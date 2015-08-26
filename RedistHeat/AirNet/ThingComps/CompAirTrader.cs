using UnityEngine;

namespace RedistHeat
{
    public class CompAirTrader : CompAir
    {
        public void ExchangeHeatWithNets( float tempEq, float rate )
        {
            var targNet = connectedNet[(int) currentLayer];

            var tempDiff = Mathf.Abs( targNet.NetTemperature - tempEq );
            var tempRated = tempDiff*rate*props.energyPerSecond;
            if ( tempEq < targNet.NetTemperature )
            {
                targNet.NetTemperature = Mathf.Max( tempEq, targNet.NetTemperature - tempRated );
            }
            else if ( tempEq > targNet.NetTemperature )
            {
                targNet.NetTemperature = Mathf.Min( tempEq, targNet.NetTemperature + tempRated );
            }
        }
    }
}