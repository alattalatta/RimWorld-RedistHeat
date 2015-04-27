using UnityEngine;

namespace RedistHeat
{
	public class CompAirTrader : CompAir
	{
		public void ExchangeHeatNet(float targetTemp, float rate)
		{
			var tempDiff = Mathf.Abs(ConnectedNet.Temperature - targetTemp);
			var tempRated = tempDiff * rate;
			if (targetTemp < ConnectedNet.Temperature)
				ConnectedNet.Temperature = Mathf.Max(targetTemp, ConnectedNet.Temperature - tempRated);
			else if (targetTemp > ConnectedNet.Temperature)
				ConnectedNet.Temperature = Mathf.Min(targetTemp, ConnectedNet.Temperature + tempRated);
		}
	}
}
