using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class AirNet
    {
        private static int debugIdNext;
        public readonly int debugId;

        private float netTemperature;

        public readonly List< CompAir > nodes = new List< CompAir >();

        public float NetTemperature
        {
            get { return netTemperature; }
            set { netTemperature = Mathf.Clamp( value, -270, 2000 ); }
        }

        public NetLayer Layer { get; }
        public int LayerInt => (int) Layer;

        #region Constructors

        public AirNet( IEnumerable< CompAir > newNodes, NetLayer layer, float temperature )
        {
            Layer = layer;
            netTemperature = temperature;

            foreach ( var current in newNodes )
            {
                RegisterNode( current );
                current.connectedNet = this;
            }

            checked
            {
                debugId = debugIdNext++;
            }
        }

        #endregion

        public void AirNetTick()
        {

        }

        public void RegisterNode( CompAir node )
        {
            if ( nodes.Contains( node ) )
            {
                Log.Error( "LT-RH: AirNet adding node " + node + " which it already has." );
            }
            else
            {
                nodes.Add( node );
            }
        }

        public void DeregisterNode( CompAir node )
        {
            nodes.Remove( node );
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append( "AirNet " ).Append( debugId ).Append( " (nodes count: " ).Append( nodes.Count ).Append( ", layer " ).Append( Layer ).Append( ")" );
            return result.ToString();
        }

        /*
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
                var compAir = AirNetGrid.NetAt( node.parent.Position + current ) as CompAir;
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
                var compAir = AirNetGrid.NetAt( root.Position + current ) as CompAir;
                if ( compAir != null && compAir.connectedNet == connectedNet )
                {
                    //Child node will make a new net of its own, and it will be merged into this.
                    ContiguousNodes( compAir ).MergeIntoNet( rootAirNet );
                }
            }

            return rootAirNet;
        }*/
    }
}