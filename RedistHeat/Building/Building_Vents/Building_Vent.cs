using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_Vent : Building_TempControl, IWallAttachable
    {
        private const float EqualizationRate = 0.28f;

        protected IntVec3 vecNorth, vecSouth;
        protected Room roomNorth, roomSouth;

        private bool isLocked;
        private bool isWorking;
        private bool WorkingState
        {
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
                    compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption *
                                                  compTempControl.props.lowPowerConsumptionFactor;
                }

                compTempControl.operatingAtHighPower = isWorking;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue( ref isLocked, "isLocked", false );
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            vecNorth = Position + IntVec3.North.RotatedBy( Rotation );
            vecSouth = Position + IntVec3.South.RotatedBy( Rotation );
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

            float pointTemp;
            if ( roomNorth.UsesOutdoorTemperature )
            {
                pointTemp = roomNorth.Temperature;
            }
            else if ( roomSouth.UsesOutdoorTemperature )
            {
                pointTemp = roomSouth.Temperature;
            }
            else
            {
                //Average temperature with cell counts in account
                pointTemp = (roomNorth.Temperature * roomNorth.CellCount + roomSouth.Temperature * roomSouth.CellCount) /
                             (roomNorth.CellCount + roomSouth.CellCount);
			}

			if (compTempControl != null)
			{
				// Trying to remove temperature spiking
				if (compTempControl.targetTemperature < roomNorth.Temperature)
				{
					pointTemp = Mathf.Max(pointTemp, compTempControl.targetTemperature) - 1;
				}
				else
				{
					pointTemp = Mathf.Min(pointTemp, compTempControl.targetTemperature) + 1;
				}
			}

			if ( !roomNorth.UsesOutdoorTemperature )
            {
                Equalize( roomNorth, pointTemp, EqualizationRate );
            }
            if ( !roomSouth.UsesOutdoorTemperature )
            {
                Equalize( roomSouth, pointTemp, EqualizationRate );
            }
        }

        protected virtual void Equalize( Room room, float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( room.Temperature - targetTemp );
            var tempRated = tempDiff * rate;
            if ( targetTemp < room.Temperature )
            {
                room.Temperature = Mathf.Max( targetTemp, room.Temperature - tempRated );
            }
            else if ( targetTemp > room.Temperature )
            {
                room.Temperature = Mathf.Min( targetTemp, room.Temperature + tempRated );
            }
        }

        protected virtual bool Validate()
        {
            if ( vecNorth.Impassable() || vecSouth.Impassable() )
			{
				return false;
            }

            roomNorth = (Position + IntVec3.North.RotatedBy( Rotation )).GetRoom();
            roomSouth = (Position + IntVec3.South.RotatedBy( Rotation )).GetRoom();
            if ( roomNorth == null || roomSouth == null || roomNorth == roomSouth)
            {
                return false;
            }

            if ( roomNorth.UsesOutdoorTemperature && roomSouth.UsesOutdoorTemperature )
            {
                return false;
            }

            return !isLocked;
        }

        public override void Draw()
        {
            base.Draw();
            if ( isLocked )
            {
                OverlayDrawer.DrawOverlay( this, OverlayTypes.ForbiddenBig );
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
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