using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_DuctComp : Building_TempControl
    {
        private const float EqualizationRate = 0.85f;
        private bool isLocked;
        
        protected CompAirTrader compAir;
        protected Room roomNorth;

        public override string LabelBase => base.LabelBase + " (" + compAir.currentLayer.ToString().ToLower() + ")";

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            compAir = GetComp< CompAirTrader >();
            roomNorth = (Position + IntVec3.North.RotatedBy( Rotation )).GetRoom();
            Common.WipeExistingPipe( Position );
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue( ref isLocked, "isLocked", false );
        }

        public override void TickRare()
        {
            if ( !Validate() )
            {
                return;
            }

            var connectedNet = compAir.connectedNet;
            roomNorth = (Position + IntVec3.North.RotatedBy( Rotation )).GetRoom();

            float tempEq;
            if ( roomNorth.UsesOutdoorTemperature )
            {
                tempEq = roomNorth.Temperature;
            }
            else
            {
                tempEq = (roomNorth.Temperature*roomNorth.CellCount +
                          connectedNet.NetTemperature*connectedNet.nodes.Count)
                         /(roomNorth.CellCount + connectedNet.nodes.Count);
            }

            compAir.ExchangeHeatWithNets( tempEq, EqualizationRate );
            if ( !roomNorth.UsesOutdoorTemperature )
            {
                ExchangeHeat( roomNorth, tempEq, EqualizationRate );
            }
        }

        private static void ExchangeHeat( Room r, float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( r.Temperature - targetTemp );
            var tempRated = tempDiff*rate;
            if ( targetTemp < r.Temperature )
            {
                r.Temperature = Mathf.Max( targetTemp, r.Temperature - tempRated );
            }
            else if ( targetTemp > r.Temperature )
            {
                r.Temperature = Mathf.Min( targetTemp, r.Temperature + tempRated );
            }
        }

        protected virtual bool Validate()
        {
            if ( roomNorth == null )
            {
                return false;
            }
            return !isLocked && compPowerTrader.PowerOn;
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