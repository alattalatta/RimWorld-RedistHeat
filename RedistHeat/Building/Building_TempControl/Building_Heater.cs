#region

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
            var temperature = Position.GetTemperature();
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
            var energyLimit = compTempControl.props.energyPerSecond*num*4.16666651f;
            var num2 = GenTemperature.ControlTemperatureTempChange( Position, energyLimit,
                                                                    compTempControl.targetTemperature );
            var flag = !Mathf.Approximately( num2, 0f );
            if (flag)
            {
                Position.GetRoom().Temperature += num2;
                compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption;
            }
            else
            {
                compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption*
                                              compTempControl.props.lowPowerConsumptionFactor;
            }
            compTempControl.operatingAtHighPower = flag;
        }
    }
}