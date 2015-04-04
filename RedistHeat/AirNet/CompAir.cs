using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class CompAir : ThingComp
	{
		public AirNet connectedNet;

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
			if (connectedNet == null)
			{
				connectedNet = new AirNet(new List<CompAir> { this });
			}
			AirNetGrid.Register(this);
			StaticSet.WipeExistingPipe(parent);
		}
		public override void PostDeSpawn()
		{
			base.PostDeSpawn();
			AirNetGrid.Deregister(this);
		}
		public override string CompInspectStringExtra()
		{
			var str = new StringBuilder();
			if (connectedNet == null)
			{
				return "No AirNet";
			}
			str.Append(StaticSet.StringNetworkTemperature + ": ");
			str.AppendLine(Mathf.Round(connectedNet.Temperature).ToStringTemperature("F0"));
			str.Append((StaticSet.StringNetworkID + ": "));
			str.Append(connectedNet.NetId.ToString());

			return str.ToString();
		}

		public void TryConnectTo()
		{
			//Must check inside for underneath pipe
			foreach (var c in GenAdj.CardinalDirectionsAndInside)
			{
				var compAir = AirNetGrid.AirNodeAt(c + Position);
				if (compAir == null || compAir.connectedNet == null)
					continue;

				ConnectToNet(compAir.connectedNet);
			}
		}
		private void ConnectToNet(AirNet net)
		{
			if (connectedNet == null)
				net.RegisterNode(this);
			else
				connectedNet.MergeIntoNet(net);
		}
		public override void CompPrintForPowerGrid(SectionLayer layer)
		{
			AirOverlayMat.LinkedOverlayGraphic.Print(layer, parent);
		}
	}
}
