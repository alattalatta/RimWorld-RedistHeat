using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RedistHeat
{
	public class AirNet
	{
		private float _netTemperature;
		private static int _lastId;

		public readonly List<CompAir> Nodes = new List<CompAir>();
		public float NetTemperature
		{
			get { return _netTemperature; }
			set { _netTemperature = Mathf.Clamp(value, -270, 2000); }
		}
		public int NetId
		{
			get;
			private set;
		}

		/* Constructors */
		public AirNet()
		{
			NetId = checked(_lastId++);
			_netTemperature = GenTemperature.OutdoorTemp;
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
			: this(new List<CompAir> { newNode }) { }


		public void RegisterNode(CompAir node)
		{
			if (node.ConnectedNet == this)
			{
				Log.Warning("Tried to register " + node + " on net it's already on!");
			}
			else
			{
				//Deregister if the node is registered to another net
				node.ConnectedNet?.DeregisterNode(node);
				Nodes.Add(node);
				node.ConnectedNet = this;
			}
		}
		
		public void DeregisterNode(CompAir node)
		{
			Nodes.Remove(node);
			node.ConnectedNet = null;
		}

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
			foreach (var current in GenAdj.CardinalDirectionsAndInside)
			{
				var compAir = AirNetGrid.AirNodeAt(node.Position + current) as CompAir;
				if (compAir == null || compAir.ConnectedNet != this)
					//There is no net to split, or it is already done.
					continue;
				
				//Make a new AirNet, starting from compAir.
				ContiguousNodes(compAir);
			}
		}
		private static AirNet ContiguousNodes(CompAir root)
		{
			var connectedNet = root.ConnectedNet;
			connectedNet.DeregisterNode(root);
			//Make a new.
			var rootAirNet = new AirNet(root);

			//Should check inside?
			foreach (var current in GenAdj.CardinalDirections)//AndInside)
			{
				var compAir = AirNetGrid.AirNodeAt(root.Position + current) as CompAir;
				if (compAir != null && compAir.ConnectedNet == connectedNet)
				{
					//Child node will make a new net of its own, and it will be merged into this.
					ContiguousNodes(compAir).MergeIntoNet(rootAirNet);
				}
			}

			return rootAirNet;
		}
	}
}
