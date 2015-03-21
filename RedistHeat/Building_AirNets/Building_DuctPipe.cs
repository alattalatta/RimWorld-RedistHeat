using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RedistHeat
{
	public class Building_DuctPipe : Building
	{
		private CompAirTransmitter compAir;
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			compAir = GetComp<CompAirTransmitter>();
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}

			var l = new Command_Action
			{
				defaultLabel = StaticSet.StringUIRefreshIDLabel,
				defaultDesc = StaticSet.StringUIRefreshIDDesc,
				hotKey = KeyBindingDefOf.CommandTogglePower,
				icon = StaticSet.UIRefreshID,
				action = () => compAir.TryConnectTo()
			};
			yield return l;
		}
	}
}