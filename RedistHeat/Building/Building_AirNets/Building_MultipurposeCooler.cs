using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_MultipurposeCooler : Building_DuctComp
    {
        protected override IntVec3 RoomVec => Position;
        //protected Building_DuctPipe pipe;

        private float Energy => compTempControl.Props.energyPerSecond;
        private int Units => compAir.Props.units;
        public bool Cooling = true;

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            //pipe = (Building_DuctPipe) GenSpawn.Spawn(ThingDef.Named("RedistHeat_DuctPipeLower"), Position, map);
        }

        protected override void Equalize()
        {
            var nodeCount = compAir.connectedNet.nodes.Count;
            var netTemp = compAir.connectedNet.NetTemperature;
            var roomTemp = room.Temperature;
            var targetTemp = compTempControl.targetTemperature;
            var tempDiff = roomTemp - netTemp;
            tempDiff *= Cooling ? 1f : -1f;
             
            var maxTempDiff = 130f;

            if (roomTemp - tempDiff > 40.0)
            {
                tempDiff = roomTemp - 40f;
            }

            var effectiveness = 1f - tempDiff/maxTempDiff;
            if (effectiveness < 0.0)
            {
                effectiveness = 0.0f;
            }

            var energyLimit = (float)(Energy * effectiveness * 4.16666650772095f);

#if DEBUG
            Log.Message("RedistHeat: Exchanger netTemp: "+netTemp+", roomTemp: "+roomTemp+", tempDiff: "+tempDiff+", energyLimit: "+energyLimit+", Cooling: "+Cooling);
#endif

            if (Cooling)
            {
                var coldAir = ControlTemperatureTempChange(nodeCount, netTemp, energyLimit,
                                                        compTempControl.targetTemperature);
#if DEBUG
                Log.Message("RedistHeat: Exchanger coldAir: "+coldAir);
#endif
                WorkingState = !Mathf.Approximately(coldAir, 0.0f);
                if (!WorkingState)
                {
                    return;
                }
                compAir.SetNetTemperatureDirect(coldAir);

                if (room.UsesOutdoorTemperature)
                {
                    return;
                }

                var hotAir = -energyLimit * 1.25f;
#if DEBUG
                Log.Message("RedistHeat: Exchanger hotAir: "+hotAir);
#endif
                if (Mathf.Approximately(hotAir, 0.0f))
                {
                    return;
                }
                GenTemperature.PushHeat(RoomVec, this.Map, hotAir);
            }
            else
            {
                var coldAir = ControlTemperatureTempChange(room.CellCount, room.Temperature, energyLimit,
                                                        compTempControl.targetTemperature);
#if DEBUG
                Log.Message("RedistHeat: Exchanger coldAir: " + coldAir);
#endif
                WorkingState = !Mathf.Approximately(coldAir, 0.0f);
                if (!WorkingState)
                {
                    return;
                }

                var hotAir = (-energyLimit * 1.25f)/nodeCount;

                compAir.SetNetTemperatureDirect(hotAir);

                if (room.UsesOutdoorTemperature)
                {
                    return;
                }

                room.Temperature += coldAir;
#if DEBUG
                Log.Message("RedistHeat: Exchanger coldAir: " + hotAir);
#endif
                
            }
        }

        protected override bool Validate()
        {
            if (!base.Validate())
            {
                return false;
            }

            return Cooling ? compTempControl.targetTemperature - 1 < compAir.connectedNet.NetTemperature - 3 : compTempControl.targetTemperature - 1 < room.Temperature - 3;
        }

        public void CycleMode()
        {
            if (Cooling)
                Cooling = false;
            else
                Cooling = true;
        }

        private static float ControlTemperatureTempChange( int count, float temperature, float energyLimit, float targetTemperature )
        {
            var b = energyLimit/count;
            var a = targetTemperature - temperature;
            float num;
            if (energyLimit > 0f)
            {
                num = Mathf.Min( a, b );
                num = Mathf.Max( num, 0f );
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