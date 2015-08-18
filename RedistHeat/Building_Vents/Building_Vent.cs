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

		protected Room RoomNorth, RoomSouth;

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

			RoomNorth = (Position + IntVec3.North.RotatedBy(Rotation)).GetRoom();
			if (RoomNorth == null)
			{
				return;
			}
			RoomSouth = (Position + IntVec3.South.RotatedBy(Rotation)).GetRoom();
			if (RoomSouth == null)
			{
				return;
			}

			if (RoomNorth == RoomSouth || (RoomNorth.UsesOutdoorTemperature && RoomSouth.UsesOutdoorTemperature))
			{
				return;
			}

			float tempEq;
			if (RoomNorth.UsesOutdoorTemperature)
				tempEq = RoomNorth.Temperature;
			else if (RoomSouth.UsesOutdoorTemperature)
				tempEq = RoomSouth.Temperature;
			else
			{
				tempEq = (RoomNorth.Temperature * RoomNorth.CellCount + RoomSouth.Temperature * RoomSouth.CellCount)
					/ (RoomNorth.CellCount + RoomSouth.CellCount);
			}

			if(!RoomNorth.UsesOutdoorTemperature)
				ExchangeHeat(RoomNorth, tempEq, EqualizationRate);
			if(!RoomSouth.UsesOutdoorTemperature)
				ExchangeHeat(RoomSouth, tempEq, EqualizationRate);
			
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