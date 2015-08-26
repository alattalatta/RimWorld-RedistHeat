using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class AirNet
    {
        private static int debugIdNext;
        public readonly int debugId;

        private IntVec3 root;

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

        public AirNet( IEnumerable< CompAir > newNodes, NetLayer layer, float temperature, IntVec3 root )
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
            this.root = root;
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
            result.Append( "AirNet " )
                  .Append( debugId )
                  .Append( " (nodes count: " )
                  .Append( nodes.Count )
                  .Append( ", layer " )
                  .Append( Layer )
                  .Append( ", root " )
                  .Append( root )
                  .Append( ")" );
            return result.ToString();
        }
    }
}