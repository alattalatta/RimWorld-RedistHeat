using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_IndustrialHeater : Building_TempControl
    {
        private int sprayTicksLeft;

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

            ControlTemperature();
            if (compTempControl.operatingAtHighPower)
            {
                SteamTick();
            }
        }

        private void ControlTemperature()
        {
            var temperature = Position.GetTemperature();
            float energyMod;
            if (temperature < 20f)
            {
                energyMod = 1f;
            }
            else
            {
                energyMod = temperature > 120f
                    ? 0f
                    : Mathf.InverseLerp( 120f, 20f, temperature );
            }
            var energyLimit = compTempControl.Props.energyPerSecond*energyMod*4.16666651f;
            var hotAir = GenTemperature.ControlTemperatureTempChange( Position, energyLimit,
                                                                      compTempControl.targetTemperature );

            var hotIsHot = !Mathf.Approximately( hotAir, 0f );
            if (hotIsHot)
            {
                Position.GetRoom().Temperature += hotAir;
                compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption;
            }
            else
            {
                compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption*
                                              compTempControl.Props.lowPowerConsumptionFactor;
            }
            compTempControl.operatingAtHighPower = hotIsHot;
        }

        private void SteamTick()
        {
            if (sprayTicksLeft > 0)
            {
                sprayTicksLeft--;
                if (Rand.Value < 0.8f)
                {
                    MoteThrower.ThrowAirPuffUp( this.TrueCenter() );
                }
                if (sprayTicksLeft <= 0)
                {
                    sprayTicksLeft = Rand.RangeInclusive( 1, 10 );
                }
            }
            else
            {
                sprayTicksLeft = Rand.RangeInclusive( 5, 10 );
            }
        }
    }
}