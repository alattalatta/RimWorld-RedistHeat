using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class CompAir : CompAirBase
	{
		public AirNet ConnectedNet;

		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();

			TryConnectTo();
			if (ConnectedNet == null)
			{
				ConnectedNet = new AirNet(new List<CompAir> { this });
			}
		}

		public override void PostDeSpawn()
		{
			base.PostDeSpawn();
			ConnectedNet.SplitNetAt(this);
		}

		public override string CompInspectStringExtra()
		{
			var str = new StringBuilder();
			if (ConnectedNet == null)
			{
				return "No AirNet";
			}
			str.Append(StaticSet.StringNetworkID + ": ");
			str.Append(ConnectedNet.NetId + " / ");
			str.Append(StaticSet.StringNetworkTemperature + ": ");
			str.Append(Mathf.Round(ConnectedNet.NetTemperature).ToStringTemperature("F0"));

			return str.ToString();
		}

		public void TryConnectTo()
		{
			//Must check inside for underneath pipe
			foreach (var c in GenAdj.CardinalDirectionsAndInside)
			{
				var compAir = AirNetGrid.AirNodeAt(c + Position) as CompAir;
				if (compAir == null || compAir.ConnectedNet == null)
					continue;

				ConnectToNet(compAir.ConnectedNet);
			}
		}
		private void ConnectToNet(AirNet net)
		{
			if (ConnectedNet == null)
				net.RegisterNode(this);
			else
				ConnectedNet.MergeIntoNet(net);
		}
		public override void CompPrintForPowerGrid(SectionLayer layer)
		{
			AirOverlayMat.LinkedOverlayGraphic.Print(layer, parent);
		}
	}
}
