using UnityEngine;

namespace RedistHeat
{
	public class CompAirTrader : CompAir
	{
		public void ExchangeHeatWithNet(float tempEq, float rate)
		{
			var tempDiff = Mathf.Abs(ConnectedNet.NetTemperature - tempEq);
			var tempRated = tempDiff * rate * props.energyPerSecond;
			if (tempEq < ConnectedNet.NetTemperature)
				ConnectedNet.NetTemperature = Mathf.Max(tempEq, ConnectedNet.NetTemperature - tempRated);
			else if (tempEq > ConnectedNet.NetTemperature)
				ConnectedNet.NetTemperature = Mathf.Min(tempEq, ConnectedNet.NetTemperature + tempRated);
		}
	}
}
