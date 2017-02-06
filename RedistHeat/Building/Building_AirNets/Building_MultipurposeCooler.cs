using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_MultipurposeCooler : Building_DuctSwitchable
    {
        protected override IntVec3 RoomVec => Position;
        //protected Building_DuctPipe pipe;

        private float Energy => compTempControl.Props.energyPerSecond;

        public Building_MultipurposeCooler()
        {
            Net = true;
        }

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            //pipe = (Building_DuctPipe) GenSpawn.Spawn(ThingDef.Named("RedistHeat_DuctPipeLower"), Position, map);
        }

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
            Equalize();
        }
        protected void Equalize()
        {
            var nodeCount = compAir.connectedNet.nodes.Count;
            var netTemp = compAir.connectedNet.NetTemperature;
            var roomTemp = room.Temperature;
            var targetTemp = compTempControl.targetTemperature;
            var tempDiff = roomTemp - netTemp;
            tempDiff *= Net ? 1f : -1f;
             
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

            if (Net)
            {
                var coldAir = ControlTemperatureTempChange(nodeCount, netTemp, energyLimit,
                                                        compTempControl.targetTemperature);

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

                
            }
        }

        protected override bool Validate()
        {
            room = RoomVec.GetRoom(this.Map);
            return Net ? compTempControl.targetTemperature - 1 < compAir.connectedNet.NetTemperature - 3 : compTempControl.targetTemperature - 1 < room.Temperature - 3;
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