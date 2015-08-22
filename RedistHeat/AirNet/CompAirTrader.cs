using UnityEngine;

namespace RedistHeat
{
	public class CompAirTrader : CompAir
	{
		public void ExchangeHeatWithNet(float targetTemp, float rate)
		{
			var tempDiff = Mathf.Abs(ConnectedNet.NetTemperature - targetTemp);
			var tempRated = tempDiff * rate * props.energyPerSecond;
			if (targetTemp < ConnectedNet.NetTemperature)
				ConnectedNet.NetTemperature = Mathf.Max(targetTemp, ConnectedNet.NetTemperature - tempRated);
			else if (targetTemp > ConnectedNet.NetTemperature)
				ConnectedNet.NetTemperature = Mathf.Min(targetTemp, ConnectedNet.NetTemperature + tempRated);
		}
	}
}
