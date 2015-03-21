using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class Building_DuctHeater : Building, ITempExchangable
	{
		private CompAirConditioner compAir;
		private CompPowerTrader compPower;

		public override void SpawnSetup()
		{
			base.SpawnSetup();
			compAir = GetComp<CompAirConditioner>();
			compPower = GetComp<CompPowerTrader>();
		}
		public override void Tick()
		{
			base.Tick();
			ExchangeHeat();
		}

		public virtual void ExchangeHeat()
		{
			if (Find.TickManager.TicksGame % ConstSet.E_INTERVAL_SEC != 0 || !Validate())
				return;

			var room = Position.GetRoom();
			var netTemp = compAir.connectedNet.Temperature;

			//Vanilla heater codes
			float num;
			if (netTemp < 20f)
			{
				num = 1f;
			}
			else if (netTemp > 120f)
			{
				num = 0f;
			}
			else
			{
				num = Mathf.InverseLerp(120f, 20f, netTemp);
			}
			var energyLimit = compAir.props.energyPerSecond * num * 4.16666651f;
			var num2 = StaticSet.ControlTemperatureTempChange(room, energyLimit, compAir.targetTemperature);
			var flag = !Mathf.Approximately(num2, 0f);
			if (flag)
			{
				room.Temperature += num2 / 2;
				compAir.connectedNet.Temperature += num2;
				compPower.powerOutput = -compPower.props.basePowerConsumption;
			}
			else
			{
				compPower.powerOutput = -compPower.props.basePowerConsumption * 0.1f;
			}
		}
		public virtual bool Validate()
		{
			return compPower == null || compPower.PowerOn && (ValidateTemp(compAir.connectedNet.Temperature));
		}
		private bool ValidateTemp(float netTemp)
		{
			return netTemp < compAir.targetTemperature;
		}
	}
}