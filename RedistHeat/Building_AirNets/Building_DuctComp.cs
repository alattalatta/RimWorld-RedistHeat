using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RedistHeat
{
    public class Building_DuctComp : Building_TempControl
    {
        private const float EqualizationRate = 0.85f;

        protected CompAirTrader compAir;
        protected Room roomNorth;
        protected IntVec3 vecNorth;

        private int netTemp;
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
            vecNorth = Position + IntVec3.North.RotatedBy( Rotation );

            Common.WipeExistingPipe( Position );
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue( ref isLocked, "isLocked", false );
            Scribe_Values.LookValue( ref netTemp, "netTemp", 999 );
        }

        public override void Tick()
        {
            if ( !this.IsHashIntervalTick( 250 ) )
            {
                return;
            }
            netTemp = (int)compAir.connectedNet.NetTemperature;

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
            if ( vecNorth.Impassable() )
            {
                return false;
            }

            roomNorth = vecNorth.GetRoom();
            if ( roomNorth == null )
            {
                return false;
            }

            return !isLocked && (compPowerTrader == null || compPowerTrader.PowerOn);
        }

        protected virtual void Equalize()
        {
            float targetTemp;
            if ( roomNorth.UsesOutdoorTemperature )
            {
                targetTemp = roomNorth.Temperature;
            }
            else
            {
                targetTemp = (roomNorth.Temperature*roomNorth.CellCount +
                              compAir.connectedNet.NetTemperature*compAir.connectedNet.nodes.Count)
                             /(roomNorth.CellCount + compAir.connectedNet.nodes.Count);
            }

            compAir.EqualizeWithNet( targetTemp, EqualizationRate );
            if ( !roomNorth.UsesOutdoorTemperature )
            {
                compAir.EqualizeWithRoom( roomNorth, targetTemp, EqualizationRate );
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