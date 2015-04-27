using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RedistHeat
{
	public class AirNet
	{
		private float temperature;
		private static int _lastId;

		public List<CompAir> Nodes = new List<CompAir>();

		public float Temperature
		{
			get { return temperature; }
			set { temperature = Mathf.Clamp(value, -270, 2000); }
		}

		public int NetId
		{
			get;
			private set;
		}
		public AirNet()
		{
			NetId = checked(AirNet._lastId++);
			temperature = GenTemperature.OutdoorTemp;
		}
		public AirNet(IEnumerable<CompAir> newNodes)
			: this()
		{
			foreach (var current in newNodes)
			{
				RegisterNode(current);
			}
		}
		private AirNet(CompAir newNode)
			: this(new List<CompAir> { newNode })
		{
		}
		public void RegisterNode(CompAir node)
		{
			if (node.ConnectedNet == this)
			{
				Log.Warning("Tried to register " + node + " on net it's already on!");
			}
			else
			{
				if (node.ConnectedNet != null)
					node.ConnectedNet.DeregisterNode(node);
				else
				{
					Nodes.Add(node);
					node.ConnectedNet = this;
				}
			}
		}
		// ReSharper disable once MemberCanBePrivate.Global
		public void DeregisterNode(CompAir node)
		{
			Nodes.Remove(node);
			node.ConnectedNet = null;
		}/*
		public void PushHeat(float e)
		{
			if(Nodes.Count == 1)
				Temperature += e / Nodes.Count;
			else
				Temperature += e * 2 / Nodes.Count;
		}*/
		public void MergeIntoNet(AirNet newNet)
		{
			foreach (var current in new List<CompAir>(Nodes))
			{
				DeregisterNode(current);
				newNet.RegisterNode(current);
			}
		}
		public void SplitNetAt(CompAir node)
		{
			//Must check inside for underneath pipe
			foreach (var current in GenAdj.CardinalDirectionsAndInside)
			{
				var compAir = AirNetGrid.AirNodeAt(node.Position + current) as CompAir;
				if (compAir == null || compAir.ConnectedNet != this)
					continue;
				AirNet.ContiguousNodes(compAir);
			}
		}
		private static AirNet ContiguousNodes(CompAir root)
		{
			var connectedNet = root.ConnectedNet;
			connectedNet.DeregisterNode(root);
			var airNet = new AirNet(root);

			//Should check inside?
			foreach (var current in GenAdj.CardinalDirectionsAndInside)
			{
				var compAir = AirNetGrid.AirNodeAt(root.Position + current) as CompAir;
				if (compAir != null && compAir.ConnectedNet == connectedNet)
				{
					AirNet.ContiguousNodes(compAir).MergeIntoNet(airNet);
				}
			}
			return airNet;
		}
	}
}
