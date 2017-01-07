using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_DuctComp : Building_DuctBase, IWallAttachable
    {
        protected const float EqualizationRate = 0.204f; // RareTick @ 0.85f;

        protected CompAirTrader compAir;
        protected Room room;
        protected virtual IntVec3 RoomVec => Position + IntVec3.North.RotatedBy( Rotation );

        private bool isLocked;
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
                    compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption*
                                                  compTempControl.Props.lowPowerConsumptionFactor;
                }

                compTempControl.operatingAtHighPower = isWorking;
            }
        }


        public override string LabelNoCount => base.LabelNoCount + " (" + compAir.currentLayer.ToString().ToLower() + ")";

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            compAir = GetComp< CompAirTrader >();

            Common.WipeExistingPipe( Position );
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue( ref isLocked, "isLocked", false );
        }

        public override void Tick()
        {
            base.Tick();
            if (!this.IsHashIntervalTick( 60 ))
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

        /// <summary>
        /// Base validater. Checks if vecNorth is passable, room is there, is building locked, and power is on.
        /// </summary>
        protected virtual bool Validate()
        {
            if (compAir.connectedNet == null)
            {
                return false;
            }

            if (RoomVec.Impassable(this.Map))
            {
                return false;
            }

            room = RoomVec.GetRoom(this.Map);

            if (room == null)
            {
                return false;
            }

            return !isLocked && (compPowerTrader == null || compPowerTrader.PowerOn);
        }

        protected virtual void Equalize()
        {
            float pointTemp;
            if (room.UsesOutdoorTemperature)
            {
                pointTemp = room.Temperature;
            }
            else
            {
                pointTemp = (room.Temperature*room.CellCount +
                             compAir.connectedNet.NetTemperature*compAir.connectedNet.nodes.Count*5)
                            /(room.CellCount + compAir.connectedNet.nodes.Count*5);
            }

            if (compTempControl != null)
            {
                // Trying to remove temperature spiking
                if (compTempControl.targetTemperature < room.Temperature)
                {
                    pointTemp = Mathf.Max( pointTemp, compTempControl.targetTemperature ) - 1;
                }
                else
                {
                    pointTemp = Mathf.Min( pointTemp, compTempControl.targetTemperature ) + 1;
                }
            }
//#if DEBUG
//            Log.Message("RedistHeat: DuctComp ----- Device: " + this + ", pointTemp: " + pointTemp);
//#endif
            compAir.EqualizeWithNet( pointTemp, EqualizationRate );
            if (!room.UsesOutdoorTemperature)
            {
                compAir.EqualizeWithRoom( room, pointTemp, EqualizationRate );
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (isLocked)
            {
                this.Map.overlayDrawer.DrawOverlay( this, OverlayTypes.ForbiddenBig );
            }
        }

        public override IEnumerable< Gizmo > GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            var l = new Command_Toggle
            {
                defaultLabel = ResourceBank.StringToggleAirflowLabel,
                defaultDesc = ResourceBank.StringToggleAirflowDesc,
                hotKey = KeyBindingDefOf.CommandItemForbid,
                icon = ResourceBank.UILock,
                groupKey = 912515,
                isActive = () => !isLocked,
                toggleAction = () => isLocked = !isLocked
            };
            yield return l;
        }
    }
}