using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class Building_IndustrialHeater : Building_TempControl
	{
		private int ticksUntilSpray = 6;
		private int sprayTicksLeft;

		public override void TickRare()
		{
			if (!compPowerTrader.PowerOn)
				return;

			ControlTemperature();
			if (compTempControl.operatingAtHighPower)
				SteamTick();
		}
		private void ControlTemperature()
		{
			var temperature = Position.GetTemperature();
			float powerMod;
			if (temperature < 20f)
			{
				powerMod = 1f;
			}
			else
			{
				if (temperature > 140f)
				{
					powerMod = 0f;
				}
				else
				{
					powerMod = Mathf.InverseLerp(120f, 20f, temperature);
				}
			}
			var energyLimit = compTempControl.props.energyPerSecond * powerMod * 4.16666651f;
			var num2 = GenTemperature.ControlTemperatureTempChange(Position, energyLimit, compTempControl.targetTemperature);
			var flag = !Mathf.Approximately(num2, 0f);
			if (flag)
			{
				Position.GetRoom().Temperature += num2;
				compPowerTrader.powerOutput = -compPowerTrader.props.basePowerConsumption;
			}
			else
			{
				compPowerTrader.powerOutput = -compPowerTrader.props.basePowerConsumption * compTempControl.props.lowPowerConsumptionFactor;
			}
			compTempControl.operatingAtHighPower = flag;
		}
		private void SteamTick()
		{
			if (sprayTicksLeft > 0)
			{
				sprayTicksLeft--;
				if (Rand.Value < 0.8f)
				{
					MoteThrower.ThrowAirPuffUp(this.TrueCenter());
				}
				if (sprayTicksLeft <= 0)
				{
					ticksUntilSpray = Rand.RangeInclusive(0, 10);
				}
			}
			else
			{
				ticksUntilSpray--;
				if (ticksUntilSpray <= 0)
				{
					sprayTicksLeft = Rand.RangeInclusive(5, 10);
				}
			}
		}
	}
}
