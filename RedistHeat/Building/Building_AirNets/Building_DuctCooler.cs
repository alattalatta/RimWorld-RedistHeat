using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_DuctCooler : Building_DuctComp
    {
        protected override IntVec3 RoomVec => Position;

        private float Energy => compTempControl.Props.energyPerSecond;

        protected override void Equalize()
        {
            var tempHot = room.Temperature;

            //Cooler's temperature
            var tempCold = compAir.connectedNet.NetTemperature;
            var tempDiff = tempHot - tempCold;

            if (tempHot - tempDiff > 40.0)
            {
                tempDiff = tempHot - 40f;
            }

            var num2 = 1.0 - tempDiff*(1.0/130.0);
            if (num2 < 0.0)
            {
                num2 = 0.0f;
            }

            var energyLimit = (float) (Energy*num2*4.16666650772095);
            var coldAir = ControlTemperatureTempChange( compAir.connectedNet, energyLimit,
                                                        compTempControl.targetTemperature );
            WorkingState = !Mathf.Approximately( coldAir, 0.0f );
            if (!WorkingState)
            {
                return;
            }
            compAir.SetNetTemperatureDirect( coldAir );

            if (room.UsesOutdoorTemperature)
            {
                return;
            }

            var hotAir = -energyLimit*1.25f;
            if (Mathf.Approximately( hotAir, 0.0f ))
            {
                return;
            }

            GenTemperature.PushHeat( RoomVec, this.Map, hotAir);
        }

        protected override bool Validate()
        {
            if (!base.Validate())
            {
                return false;
            }

            return compTempControl.targetTemperature < compAir.connectedNet.NetTemperature;
        }

        private static float ControlTemperatureTempChange( AirNet net, float energyLimit, float targetTemperature )
        {
            var b = energyLimit/net.nodes.Count;
            var a = targetTemperature - net.NetTemperature;
            float num;
            if (energyLimit > 0f)
            {
                num = Mathf.Min( a, b );
                num = Mathf.Max( num, 0f );
            }
            else
            {
                num = Mathf.Max( a, b );
                num = Mathf.Min( num, 0f );
            }
            return num;
        }
    }
}