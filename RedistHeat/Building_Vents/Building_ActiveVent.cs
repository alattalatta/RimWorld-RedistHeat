using RimWorld;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RedistHeat
{
	public class Building_ActiveVent : Building_Vent
	{
		private CompPowerTrader compPower;
		private float targetTemperature = 21f;

		public override void SpawnSetup()
		{
			base.SpawnSetup();
			compPower = GetComp<CompPowerTrader>();
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue(ref targetTemperature, "targetTemperature", 21f);
		}

		public override bool Validate()
		{
			return (base.Validate() && compPower.PowerOn && ValidateTemp(roomNorth.Temperature, roomSouth.Temperature));
		}
		private bool ValidateTemp(float controlled, float other)
		{
			return ((controlled < targetTemperature && controlled < other) ||
			        (controlled > targetTemperature && controlled > other));
		}

		#region Gizmo

		public override string GetInspectString()
		{
			var strbuilder = new StringBuilder();
			strbuilder.Append(base.GetInspectString());
			strbuilder.AppendLine();
			strbuilder.Append(StaticSet.StringTargetTemperature + ": ");
			strbuilder.AppendLine(targetTemperature.ToStringTemperature("F0"));
			return strbuilder.ToString();
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}

			var l2 = new Command_Action
			{
				defaultLabel = (-10f).ToStringTemperature("F0"),
				defaultDesc = "CommandLowerTempDesc".Translate(),
				icon = StaticSet.UITempLower,
				action = () => ChangeTargetTemp(-10f)
			};
			yield return l2;

			var l1 = new Command_Action
			{
				defaultLabel = (-1f).ToStringTemperature("F0"),
				defaultDesc = "CommandLowerTempDesc".Translate(),
				icon = StaticSet.UITempLower,
				action = () => ChangeTargetTemp(-1f)
			};
			yield return l1;

			var s = new Command_Action
			{
				defaultLabel = "CommandResetTemp".Translate(),
				defaultDesc = "CommandResetTempDesc".Translate(),
				icon = StaticSet.UITempReset,
				action = () => targetTemperature = 21f
			};
			yield return s;

			var r1 = new Command_Action
			{
				defaultLabel = (1f).ToStringTemperature("F0"),
				defaultDesc = "CommandRaiseTempDesc".Translate(),
				icon = StaticSet.UITempRaise,
				action = () => ChangeTargetTemp(1f)
			};
			yield return r1;

			var r2 = new Command_Action
			{
				defaultLabel = (10f).ToStringTemperature("F0"),
				defaultDesc = "CommandRaiseTempDesc".Translate(),
				icon = StaticSet.UITempRaise,
				action = () => ChangeTargetTemp(10f)
			};
			yield return r2;
		}

		private void ChangeTargetTemp(float offset)
		{
			if (offset > 0f)
				SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
			else
				SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
			targetTemperature += offset;
			targetTemperature = Mathf.Clamp(targetTemperature, -270f, 2000f);
			ThrowCurrentTemperatureText();
		}
		private void ThrowCurrentTemperatureText()
		{
			MoteThrower.ThrowText(this.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), targetTemperature.ToStringTemperature("F0"), Color.white);
		}

		#endregion Gizmo
	}
}