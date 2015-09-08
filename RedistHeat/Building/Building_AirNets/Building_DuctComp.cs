using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_DuctComp : Building_DuctBase, IWallAttachable
    {
        protected const float EqualizationRate = 0.85f;

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

                if ( compPowerTrader == null || compTempControl == null )
                {
                    return;
                }
                if ( isWorking )
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption;
                }
                else
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption*
                                                  compTempControl.props.lowPowerConsumptionFactor;
                }

                compTempControl.operatingAtHighPower = isWorking;
            }
        }


        public override string LabelBase => base.LabelBase + " (" + compAir.currentLayer.ToString().ToLower() + ")";

        public override void SpawnSetup()
        {
            base.SpawnSetup();
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
            if ( !this.IsHashIntervalTick( 250 ) )
            {
                return;
            }

            if ( !Validate() )
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
            if ( compAir.connectedNet == null )
            {
                return false;
            }

            if ( RoomVec.Impassable() )
            {
                return false;
            }

            room = RoomVec.GetRoom();
            if ( room == null )
            {
                return false;
            }

            return !isLocked && (compPowerTrader == null || compPowerTrader.PowerOn);
        }

        protected virtual void Equalize()
        {
            float pointTemp;
            if ( room.UsesOutdoorTemperature )
            {
                pointTemp = room.Temperature;
            }
            else
            {
                pointTemp = (room.Temperature*room.CellCount +
                              compAir.connectedNet.NetTemperature*compAir.connectedNet.nodes.Count)
                             /(room.CellCount + compAir.connectedNet.nodes.Count);
            }

	        if ( compTempControl != null )
	        {
		        // Trying to remove temperature spiking
		        if ( compTempControl.targetTemperature < room.Temperature )
		        {
			        pointTemp = Mathf.Max( pointTemp, compTempControl.targetTemperature );
		        }
		        else
		        {
			        pointTemp = Mathf.Min( pointTemp, compTempControl.targetTemperature );
		        }
	        }

	        compAir.EqualizeWithNet( pointTemp, EqualizationRate );
            if ( !room.UsesOutdoorTemperature )
            {
                compAir.EqualizeWithRoom( room, pointTemp, EqualizationRate );
            }
        }

        public override void Draw()
        {
            base.Draw();
            if ( isLocked )
            {
                OverlayDrawer.DrawOverlay( this, OverlayTypes.ForbiddenBig );
            }
        }

        public override IEnumerable< Gizmo > GetGizmos()
        {
            foreach ( var g in base.GetGizmos() )
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
                isActive = () => isLocked,
                toggleAction = () => isLocked = !isLocked
            };
            yield return l;
        }
    }
}