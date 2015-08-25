using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RedistHeat
{
	public class BuildingDuctPipe : Building
	{
		private CompAirTransmitter compAir;
		private GraphicLinkedAirTransmitter graphicLinked;

		public override Graphic Graphic
		{
			get
			{
				if (graphicLinked?.MatSingle != null) return graphicLinked;
				
				GetGraphic();
				if (graphicLinked?.MatSingle != null) return graphicLinked;

				return def.graphic;
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			GetGraphic();
			compAir = GetComp<CompAirTransmitter>();
		}

		public override void DeSpawn()
		{
			base.DeSpawn();
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

		private void GetGraphic()
		{
			if (graphicLinked != null && graphicLinked.MatSingle != null) return;

			var graphicSingle = GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath);
			graphicLinked = new GraphicLinkedAirTransmitter(graphicSingle);
		}
	}
}