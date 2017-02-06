using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_IndustrialHeater : Building_DuctSwitchable
    {
        private int sprayTicksLeft;

        public override void Tick()
        {
            if (!this.IsHashIntervalTick(60))
            {
                return;
            }
            if (!Validate())
            {
                WorkingState = false;
                return;
            }
            WorkingState = true;

            ControlTemperature();
            if (compTempControl.operatingAtHighPower)
            {
                SteamTick();
            }
        }

        protected override bool Validate()
        {
            if (compAir.connectedNet == null)
            {
                return false;
            }
            room = RoomVec.GetRoom(this.Map);
            return Net ? compTempControl.targetTemperature + 1 > compAir.connectedNet.NetTemperature + 5 : compTempControl.targetTemperature + 1 > room.Temperature + 5;
        }

        private void ControlTemperature()
        {
            float temperature = 0f;
            int count = 0;

            if (Net)
            {
                temperature = compAir.connectedNet.NetTemperature;
                count = compAir.connectedNet.nodes.Count;
            }
            else
            {
                temperature = room.Temperature;
                count = room.CellCount;
            }

            float energyMod;
            if (temperature < 20f)
            {
                energyMod = 1f;
            }
            else
            {
                energyMod = temperature > 150f
                    ? 0f
                    : Mathf.InverseLerp(150f, 20f, temperature);
            }

#if DEBUG
            Log.Message("RedistHeat: IH the net has "+compAir.connectedNet.pullers+" pullers and "+compAir.connectedNet.pushers+"pushers");
#endif

            var energyLimit = compTempControl.Props.energyPerSecond * energyMod * 4.16666651f;
            var hotAir = ControlTemperatureTempChange(count, temperature, energyLimit,
                                                                        compTempControl.targetTemperature);

            WorkingState = !Mathf.Approximately(hotAir, 0f);
            if (!WorkingState)
            {
                return;
            }

            if (Net)
            {
                compAir.SetNetTemperatureDirect(hotAir);
            }
            else
            {
                room.Temperature += hotAir;
            }

        }

        private void SteamTick()
        {
            if (sprayTicksLeft > 0)
            {
                sprayTicksLeft--;
                if (Rand.Value < 0.8f)
                {
                    MoteMaker.ThrowAirPuffUp(this.TrueCenter(), this.Map);
                }
                if (sprayTicksLeft <= 0)
                {
                    sprayTicksLeft = Rand.RangeInclusive(1, 10);
                }
            }
            else
            {
                sprayTicksLeft = Rand.RangeInclusive(5, 10);
            }
        }

        private static float ControlTemperatureTempChange(int count, float temperature, float energyLimit, float targetTemperature)
        {
            var b = energyLimit / count;
            var a = targetTemperature - temperature;
            float num;
            if (energyLimit > 0f)
            {
                num = Mathf.Min(a, b);
                num = Mathf.Max(num, 0f);
            }
            else
            {
                num = Mathf.Max(a, b);
                num = Mathf.Min(num, 0f);
            }
            return num;
        }
    }
}