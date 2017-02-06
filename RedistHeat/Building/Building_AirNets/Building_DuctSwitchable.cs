using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_DuctSwitchable : Building_DuctBase
    {
        public bool Net = false;
        protected CompAirTrader compAir;
        protected Room room;
        protected virtual IntVec3 RoomVec => Position + IntVec3.North.RotatedBy(Rotation);

        private bool isWorking;

        protected bool WorkingState
        {
            get { return isWorking; }
            set
            {
                isWorking = value;

                if (compPowerTrader == null || compTempControl == null)
                {
                    return;
                }
                if (isWorking)
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption;
                }
                else
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption *
                                                  compTempControl.Props.lowPowerConsumptionFactor;
                }

                compTempControl.operatingAtHighPower = isWorking;
            }
        }

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            compAir = GetComp<CompAirTrader>();

            Common.WipeExistingPipe(Position);
        }

        public override void Tick()
        {
            base.Tick();
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
        }

        public void CycleMode()
        {
            Net = Net ? false : true;
        }

        protected virtual bool Validate()
        {
            if (compAir.connectedNet == null)
            {
                return false;
            }

            room = RoomVec.GetRoom(this.Map);

            if (room == null)
            {
                return false;
            }

            return compPowerTrader != null && compPowerTrader.PowerOn;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue(ref Net, "net", true);
        }
    }
}