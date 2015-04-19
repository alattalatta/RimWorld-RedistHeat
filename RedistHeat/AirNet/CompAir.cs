using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class CompAir : ThingComp
	{
		public AirNet ConnectedNet;

		public IntVec3 Position
		{
			get;
			private set;
		}

		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();

			Position = parent.Position;
			TryConnectTo();
			if (ConnectedNet == null)
			{
				ConnectedNet = new AirNet(new List<CompAir> { this });
			}
			AirNetGrid.Register(this);
		}
		public override void PostDeSpawn()
		{
			base.PostDeSpawn();
			AirNetGrid.Deregister(this);
		}
		public override string CompInspectStringExtra()
		{
			var str = new StringBuilder();
			if (ConnectedNet == null)
			{
				return "No AirNet";
			}
			str.Append(StaticSet.StringNetworkTemperature + ": ");
			str.AppendLine(Mathf.Round(ConnectedNet.Temperature).ToStringTemperature("F0"));
			str.Append((StaticSet.StringNetworkID + ": "));
			str.Append(ConnectedNet.NetId.ToString());

			return str.ToString();
		}

		public void TryConnectTo()
		{
			//Must check inside for underneath pipe
			foreach (var c in GenAdj.CardinalDirectionsAndInside)
			{
				var compAir = AirNetGrid.AirNodeAt(c + Position);
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
