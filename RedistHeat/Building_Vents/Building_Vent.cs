using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class BuildingVent : Building_TempControl
	{
		private const float EqualizationRate = 0.25f;
		private bool isLocked;

		protected Room roomNorth, roomSouth;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue(ref isLocked, "isLocked", false);
		}
		public override void TickRare()
		{
			if (!Validate())
			{
				return;
			}

			roomNorth = (Position + IntVec3.North.RotatedBy(Rotation)).GetRoom();
			if (roomNorth == null)
			{
				return;
			}
			roomSouth = (Position + IntVec3.South.RotatedBy(Rotation)).GetRoom();
			if (roomSouth == null)
			{
				return;
			}

			if (roomNorth == roomSouth || (roomNorth.UsesOutdoorTemperature && roomSouth.UsesOutdoorTemperature))
			{
				return;
			}

			float tempEq;
			if (roomNorth.UsesOutdoorTemperature)
				tempEq = roomNorth.Temperature;
			else if (roomSouth.UsesOutdoorTemperature)
				tempEq = roomSouth.Temperature;
			else
			{
				tempEq = (roomNorth.Temperature * roomNorth.CellCount + roomSouth.Temperature * roomSouth.CellCount)
					/ (roomNorth.CellCount + roomSouth.CellCount);
			}

			if(!roomNorth.UsesOutdoorTemperature)
				ExchangeHeat(roomNorth, tempEq, EqualizationRate);
			if(!roomSouth.UsesOutdoorTemperature)
				ExchangeHeat(roomSouth, tempEq, EqualizationRate);
			
		}

		private static void ExchangeHeat(Room r, float targetTemp, float rate)
		{
			var tempDiff = Mathf.Abs(r.Temperature - targetTemp);
			var tempRated = tempDiff * rate;
			if (targetTemp < r.Temperature)
				r.Temperature = Mathf.Max(targetTemp, r.Temperature - tempRated);
			else if (targetTemp > r.Temperature)
				r.Temperature = Mathf.Min(targetTemp, r.Temperature + tempRated);
		}

		protected virtual bool Validate()
		{
			return !isLocked;
		}

		public override void Draw()
		{
			base.Draw();
			if (isLocked)
				OverlayDrawer.DrawOverlay(this, OverlayTypes.ForbiddenBig);
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}

			var l = new Command_Toggle
			{
				defaultLabel = StaticSet.StringUILockLabel,
				defaultDesc = StaticSet.StringUILockDesc,
				hotKey = KeyBindingDefOf.CommandItemForbid,
				icon = StaticSet.UILock,
				groupKey = 912515,
				isActive = () => isLocked,
				toggleAction = () => isLocked = !isLocked
			};
			yield return l;
		}
	}
}