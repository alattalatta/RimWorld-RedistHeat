using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class Building_InDoorCooler : Building, ITempExchangable
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
			compAir.roomTemp = room.Temperature;
			var netTemp = compAir.connectedNet.Temperature;

			var tempDiff = netTemp - compAir.roomTemp;

			if (netTemp - 30.0 > tempDiff)
				tempDiff = netTemp - 40f;
			var num2 = (float)(1.0 - tempDiff * (1.0 / 65.0));
			if (num2 < 0.0)
				num2 = 0.0f;
			var energyLimit = (float)(compAir.props.energyPerSecond * num2 * 4.16666650772095);
			var tempChange = StaticSet.ControlTemperatureTempChange(room, energyLimit, compAir.targetTemperature);
			var isWorking = !Mathf.Approximately(tempChange, 0.0f);
			if (!isWorking)
				return;

			var tempPush = -energyLimit * 1.25f;
			if (Mathf.Approximately(tempPush, 0.0f))
				tempPush = 0.0f;
			room.Temperature += tempChange;

			compAir.connectedNet.PushHeat(tempPush);
		}
		public virtual bool Validate()
		{
			return compPower == null || compPower.PowerOn && (ValidateTemp(Position.GetTemperature()));
		}
		private bool ValidateTemp(float roomTemp)
		{
			return roomTemp > compAir.targetTemperature;
		}
	}
}