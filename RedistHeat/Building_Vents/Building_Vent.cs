using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
	public class Building_Vent : Building, ITempExchangable
	{
		private bool isLocked;

		private IntVec3 vecNorth;
		private IntVec3 vecSouth;
		protected Room roomNorth, roomSouth;

		public override void SpawnSetup()
		{
			base.SpawnSetup();
			vecNorth = Position + IntVec3.north.RotatedBy(Rotation);
			vecSouth = Position + IntVec3.south.RotatedBy(Rotation);
			roomNorth = vecNorth.GetRoom();
			roomSouth = vecSouth.GetRoom();
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue(ref isLocked, "isLocked", false);
		}
		public override void Tick()
		{
			base.Tick();
			ExchangeHeat();
		}

		public virtual void ExchangeHeat()
		{
			if (!Validate() || Find.TickManager.TicksGame % ConstSet.E_INTERVAL_VENT != 0)
				return;
			roomNorth = vecNorth.GetRoom();
			roomSouth = vecSouth.GetRoom();

			var temp1 = roomNorth.Temperature;
			var temp2 = roomSouth.Temperature;

			var tempDiff = temp1 - temp2;

			if (roomNorth.UsesOutdoorTemperature && roomSouth.UsesOutdoorTemperature)
				return;
			if (!roomNorth.UsesOutdoorTemperature)
				roomNorth.PushHeat(-tempDiff);
			if (!roomSouth.UsesOutdoorTemperature)
				roomSouth.PushHeat(tempDiff);
		}
		public virtual bool Validate()
		{
			return (!isLocked && !vecNorth.Impassable() && !vecSouth.Impassable());
		}


		public override void Draw()
		{
			base.Draw();
			if (isLocked)
				OverlayDrawer.DrawOverlay(this, OverlayTypes.Locked);
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}

			var l = new Command_Toggle()
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