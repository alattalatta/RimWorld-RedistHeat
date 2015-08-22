using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RedistHeat
{
	public class BuildingDuctPipe : Building
	{
		private CompAirTransmitter _compAir;
		private GraphicLinkedAirTransmitter _graphicLinked;

		public override Graphic Graphic
		{
			get
			{
				if (_graphicLinked?.MatSingle != null) return _graphicLinked;
				
				GetGraphic();
				if (_graphicLinked?.MatSingle != null) return _graphicLinked;

				return def.graphic;
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			GetGraphic();
			_compAir = GetComp<CompAirTransmitter>();
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
				action = () => _compAir.TryConnectTo()
			};
			yield return l;
		}

		private void GetGraphic()
		{
			if (_graphicLinked != null && _graphicLinked.MatSingle != null) return;

			var graphicSingle = GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath);
			_graphicLinked = new GraphicLinkedAirTransmitter(graphicSingle);
		}
	}
}