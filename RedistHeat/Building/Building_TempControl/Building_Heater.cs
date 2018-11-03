﻿#region

using RimWorld;
using UnityEngine;
using Verse;

#endregion

namespace RedistHeat
{
    internal class Building_Heater : Building_TempControl
    {
        private const float EfficiencyFalloffSpan = 100f;

        public override void Tick()
        {
            if (!this.IsHashIntervalTick( 60 ))
            {
                return;
            }
            if (!compPowerTrader.PowerOn)
            {
                return;
            }

            //None changed
            var temperature = Position.GetTemperature(this.Map);
            float num;
            if (temperature < 20f)
            {
                num = 1f;
            }
            else if (temperature > 120f)
            {
                num = 0f;
            }
            else
            {
                num = Mathf.InverseLerp( 120f, 20f, temperature );
            }
            var energyLimit = compTempControl.Props.energyPerSecond*num*4.16666651f;
            var num2 = GenTemperature.ControlTemperatureTempChange( Position, this.Map, energyLimit,
                                                                    compTempControl.targetTemperature );
            var flag = !Mathf.Approximately( num2, 0f );
            if (flag)
            {
                Position.GetRoom(this.Map).Group.Temperature += num2;
                compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption;
            }
            else
            {
                compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption*
                                              compTempControl.Props.lowPowerConsumptionFactor;
            }
            compTempControl.operatingAtHighPower = flag;
        }
    }
}