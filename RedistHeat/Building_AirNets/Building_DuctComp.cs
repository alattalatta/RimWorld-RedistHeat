using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class BuildingDuctComp : Building_TempControl
	{
		private const float EqualizationRate = 0.33f;
		private bool isLocked;
		
		protected CompAirTrader CompAir;
		protected Room RoomNorth;

		public override void SpawnSetup()
		{
			base.SpawnSetup();
			CompAir = GetComp<CompAirTrader>();
			RoomNorth = (Position + IntVec3.North.RotatedBy(Rotation)).GetRoom();
			StaticSet.WipeExistingPipe(Position);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue(ref isLocked, "isLocked", false);
		}
		public override void TickRare()
		{
			//base.Tick();

			if (!Validate())
				return;

			RoomNorth = (Position + IntVec3.North.RotatedBy(Rotation)).GetRoom();

			float tempEq;
			if (RoomNorth.UsesOutdoorTemperature)
				tempEq = RoomNorth.Temperature;
			else
			{
				tempEq = (RoomNorth.Temperature * RoomNorth.CellCount + CompAir.ConnectedNet.Temperature * CompAir.ConnectedNet.Nodes.Count)
					/ (RoomNorth.CellCount + CompAir.ConnectedNet.Nodes.Count);
			}

			//compAir.connectedNet.PushHeat((roomNorth.Temperature - compAir.connectedNet.Temperature) * compAir.props.energyPerSecond);

			CompAir.ExchangeHeatNet(tempEq, EqualizationRate);
			if (!RoomNorth.UsesOutdoorTemperature)
				ExchangeHeat(RoomNorth, tempEq, EqualizationRate);
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
			if (RoomNorth == null) return false;
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