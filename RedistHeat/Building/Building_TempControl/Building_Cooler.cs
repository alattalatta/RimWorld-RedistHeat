#region

using RimWorld;
using UnityEngine;
using Verse;

#endregion

namespace RedistHeat
{
    internal class Building_Cooler : Building_TempControl
    {
        public override void Tick()
        {
            base.Tick();
            if (!this.IsHashIntervalTick( 60 ))
            {
                return;
            }
            if (!compPowerTrader.PowerOn)
            {
                return;
            }

            //None changed
            var intVec = Position + IntVec3.South.RotatedBy( Rotation );
            var intVec2 = Position + IntVec3.North.RotatedBy( Rotation );
            var flag = false;
            if (!intVec2.Impassable() && !intVec.Impassable())
            {
                var temperature = intVec2.GetTemperature();
                var temperature2 = intVec.GetTemperature();
                var num = temperature - temperature2;
                if (temperature - 40f > num)
                {
                    num = temperature - 40f;
                }
                var num2 = 1f - num*0.0076923077f;
                if (num2 < 0f)
                {
                    num2 = 0f;
                }
                var num3 = compTempControl.Props.energyPerSecond*num2*4.16666651f;
                var num4 = GenTemperature.ControlTemperatureTempChange( intVec, num3,
                                                                        compTempControl.targetTemperature );
                flag = !Mathf.Approximately( num4, 0f );
                if (flag)
                {
                    intVec.GetRoom().Temperature += num4;
                    GenTemperature.PushHeat( intVec2, -num3*1.25f );
                }
            }
            if (flag)
            {
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