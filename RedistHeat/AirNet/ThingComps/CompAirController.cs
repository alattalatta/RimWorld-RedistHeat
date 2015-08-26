using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RedistHeat
{
    public class CompAirController : CompAirTrader
    {
        public float targetTemperature = 21f;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.LookValue( ref targetTemperature, "targetTemperature", 0f );
        }

        public override string CompInspectStringExtra()
        {
            var str = new StringBuilder();
            str.Append( ResourceBank.StringTargetTemperature + ": " );
            str.AppendLine( targetTemperature.ToStringTemperature( "F0" ) );

            str.Append( base.CompInspectStringExtra() );
            return str.ToString();
        }

        public override IEnumerable< Command > CompGetGizmosExtra()
        {
            foreach ( var g in base.CompGetGizmosExtra() )
            {
                yield return g;
            }

            var l2 = new Command_Action
            {
                defaultLabel = (-10f).ToStringTemperature( "F0" ),
                defaultDesc = "CommandLowerTempDesc".Translate(),
                icon = ResourceBank.UITempLower,
                action = () => ChangeTargetTemp( -10f )
            };
            yield return l2;

            var l1 = new Command_Action
            {
                defaultLabel = (-1f).ToStringTemperature( "F0" ),
                defaultDesc = "CommandLowerTempDesc".Translate(),
                icon = ResourceBank.UITempLower,
                action = () => ChangeTargetTemp( -1f )
            };
            yield return l1;

            var s = new Command_Action
            {
                defaultLabel = "CommandResetTemp".Translate(),
                defaultDesc = "CommandResetTempDesc".Translate(),
                icon = ResourceBank.UITempReset,
                action = () => targetTemperature = 21f
            };
            yield return s;

            var r1 = new Command_Action
            {
                defaultLabel = (1f).ToStringTemperature( "F0" ),
                defaultDesc = "CommandRaiseTempDesc".Translate(),
                icon = ResourceBank.UITempRaise,
                action = () => ChangeTargetTemp( 1f )
            };
            yield return r1;

            var r2 = new Command_Action
            {
                defaultLabel = (10f).ToStringTemperature( "F0" ),
                defaultDesc = "CommandRaiseTempDesc".Translate(),
                icon = ResourceBank.UITempRaise,
                action = () => ChangeTargetTemp( 10f )
            };
            yield return r2;
        }

        private void ChangeTargetTemp( float offset )
        {
            if ( offset > 0f )
            {
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
            }
            else
            {
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
            }
            targetTemperature += offset;
            targetTemperature = Mathf.Clamp( targetTemperature, -270f, 2000f );
            ThrowCurrentTemperatureText();
        }

        private void ThrowCurrentTemperatureText()
        {
            MoteThrower.ThrowText( parent.TrueCenter() + new Vector3( 0.5f, 0f, 0.5f ),
                targetTemperature.ToStringTemperature( "F0" ), Color.white );
        }
    }
}