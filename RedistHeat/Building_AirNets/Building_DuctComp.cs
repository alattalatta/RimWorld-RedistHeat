using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RedistHeat
{
	public class Building_DuctComp : Building, ITempExchangable
	{
		private CompAirTrader compAir;
		private CompPowerTrader compPower;
		private bool isLocked;

		protected IntVec3 vec;

		protected Room ThisRoom
		{
			get { return vec.GetRoom(); }
		}

		public override void SpawnSetup()
		{
			base.SpawnSetup();
			vec = Position + IntVec3.north.RotatedBy(Rotation);
			compAir = GetComp<CompAirTrader>();
			compPower = GetComp<CompPowerTrader>();
			compAir.roomTemp = ThisRoom.Temperature;
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
			if (Find.TickManager.TicksGame % ConstSet.E_INTERVAL_DUCT != 0 || !Validate())
				return;

			var netTemp = compAir.connectedNet.Temperature;

			compAir.connectedNet.PushHeat((ThisRoom.Temperature - netTemp) * compAir.props.energyPerSecond);

			if (ThisRoom.UsesOutdoorTemperature)
				return;
			ThisRoom.PushHeat(netTemp - ThisRoom.Temperature);
		}
		public virtual bool Validate()
		{
			if (ThisRoom == null)
				return false;
			compAir.roomTemp = ThisRoom.Temperature;
			return (compPower == null
				? !isLocked && !vec.Impassable()
				: !isLocked && !vec.Impassable() && compPower.PowerOn);
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