using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class AirNet
    {
        private float netTemperature;
        private static int lastId;

        public readonly List< CompAir > nodes = new List< CompAir >();

        public float NetTemperature
        {
            get { return netTemperature; }
            set { netTemperature = Mathf.Clamp( value, -270, 2000 ); }
        }

        public int NetId { get; private set; }

        /* Constructors */

        public AirNet()
        {
            NetId = checked(lastId++);
            netTemperature = GenTemperature.OutdoorTemp;
        }

        public AirNet( IEnumerable< CompAir > newNodes )
            : this()
        {
            foreach ( var current in newNodes )
            {
                RegisterNode( current );
            }
        }

        private AirNet( CompAir newNode )
            : this( new List< CompAir > {newNode} )
        {
        }


        public void RegisterNode( CompAir node )
        {
            if ( node.connectedNet == this )
            {
                Log.Warning( "Tried to register " + node + " on net it's already on!" );
            }
            else
            {
                //Deregister if the node is registered to another net
                node.connectedNet?.DeregisterNode( node );
                nodes.Add( node );
                node.connectedNet = this;
            }
        }

        public void DeregisterNode( CompAir node )
        {
            nodes.Remove( node );
            node.connectedNet = null;
        }

        public void MergeIntoNet( AirNet newNet )
        {
            foreach ( var current in new List< CompAir >( nodes ) )
            {
                DeregisterNode( current );
                newNet.RegisterNode( current );
            }
        }

        public void SplitNetAt( CompAir node )
        {
            foreach ( var current in GenAdj.CardinalDirectionsAndInside )
            {
                var compAir = AirNetGrid.AirNodeAt( node.Position + current ) as CompAir;
                if ( compAir == null || compAir.connectedNet != this )
                {
                    //There is no net to split, or it is already done.
                    continue;
                }

                //Make a new AirNet, starting from compAir.
                ContiguousNodes( compAir );
            }
        }

        private static AirNet ContiguousNodes( CompAir root )
        {
            var connectedNet = root.connectedNet;
            connectedNet.DeregisterNode( root );
            //Make a new.
            var rootAirNet = new AirNet( root );

            //Should check inside?
            foreach ( var current in GenAdj.CardinalDirections ) //AndInside)
            {
                var compAir = AirNetGrid.AirNodeAt( root.Position + current ) as CompAir;
                if ( compAir != null && compAir.connectedNet == connectedNet )
                {
                    //Child node will make a new net of its own, and it will be merged into this.
                    ContiguousNodes( compAir ).MergeIntoNet( rootAirNet );
                }
            }

            return rootAirNet;
        }
    }
}