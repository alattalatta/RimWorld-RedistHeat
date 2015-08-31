using Verse;
using RimWorld;
using UnityEngine;

namespace RedistHeat
{
    class Class1
    {
        public void SomeThing()
        {
            CompPowerTrader compPowerTrader = new CompPowerTrader();
            CompTempControl compTempControl = new CompTempControl();
            IntVec3 Position = IntVec3.Zero;
            Rot4 Rotation = Rot4.East;

            //https://ludeon.com/forums/index.php?topic=174.msg156834#msg156834
            if (compPowerTrader.PowerOn)
            {
                // This version is lazy, it saves time when possible
                IntVec3 intVec = Position + IntVec3.South.RotatedBy(Rotation);
                IntVec3 intVec2 = Position + IntVec3.North.RotatedBy(Rotation);
                if (!intVec2.Impassable() && !intVec.Impassable())
                {
                    // temperature (the red side) is not used, only temperature2 is needed
                    float roomBlueTemp = intVec.GetTemperature();

                    if (roomBlueTemp > compTempControl.targetTemperature)
                    {
                        Room roomBlue = RoomQuery.RoomAt(intVec);
                        // energyTarget, and energyLimit, correspond to energy change of blue room; therefore, energyLimit is negative, energyTarget is too if cooling
                        float energyTarget = (compTempControl.targetTemperature - roomBlueTemp) * roomBlue.CellCount;

                        // this conditional is a simplification of (energyTarget < 0f && !Mathf.Approximately(energyTarget, 0f) )
                        if (energyTarget <= -1.121039e-44f)
                        {
                            // not idle
                            compTempControl.operatingAtHighPower = true;
                            // energyPerSecond / 60f * 250f is energy per TickRare
                            float energyLimit = compTempControl.props.energyPerSecond / 60f * 250f * Mathf.Max(0f, 1f + EfficiencyLossPerDegreeDifference * (Mathf.Min(roomBlueTemp, 40f) - intVec2.GetTemperature()));
                            Room roomRed = RoomQuery.RoomAt(intVec2);

                            if (energyLimit < energyTarget)
                            {
                                intVec.GetRoom().Temperature = compTempControl.targetTemperature;
                                intVec2.GetRoom().Temperature -= energyTarget / roomRed.CellCount * HeatOutputMultiplier;
                                compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption * (compTempControl.props.lowPowerConsumptionFactor +
                                    (1 - compTempControl.props.lowPowerConsumptionFactor) * energyTarget / energyLimit);
                            }
                            else
                            {
                                intVec.GetRoom().Temperature += energyLimit / roomBlue.CellCount;
                                intVec2.GetRoom().Temperature -= energyLimit / roomRed.CellCount * HeatOutputMultiplier;
                                compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption;
                            }
                            return;
                        }
                    }
                }
                // idle
                compTempControl.operatingAtHighPower = false;
                compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption * compTempControl.props.lowPowerConsumptionFactor;
            }
        }
    }
}
