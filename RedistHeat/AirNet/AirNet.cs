using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class AirNet
    {
        public readonly int debugId;
        private static int debugIdNext;
        private CompAir root;

        public readonly List< CompAir > nodes = new List< CompAir >();

        private float netTemperature;

        public float NetTemperature
        {
            get { return netTemperature; }
            set { netTemperature = Mathf.Clamp( value, -270, 2000 ); }
        }

        public NetLayer Layer { get; }
        public int LayerInt => (int) Layer;


        public AirNet( IEnumerable< CompAir > newNodes, NetLayer layer, CompAir root )
        {
            Layer = layer;
            var compAirs = newNodes.ToList();

            foreach (var current in compAirs)
            {
                RegisterNode( current );
                current.connectedNet = this;
            }

            checked
            {
                debugId = debugIdNext++;
            }
            this.root = root;

            var intake =
                compAirs.Where( s => s.GetType() == typeof(CompAirTrader) )
                        .Cast< CompAirTrader >()
                        .ToList()
                        .Find(
                            s =>
                                s.parent.def.defName == "RedistHeat_DuctIntake" ||
                                s.parent.def.defName == "RedistHeat_DuctCooler" );

            if (intake == null || intake.netTemp == 999)
            {
                NetTemperature = GenTemperature.OutdoorTemp;
            }
            else
            {
                NetTemperature = intake.netTemp;
            }
        }

        public void AirNetTick()
        {
        }

        public void RegisterNode( CompAir node )
        {
            if (nodes.Contains( node ))
            {
                Log.Error( "RedistHeat: AirNet adding node " + node + " which it already has." );
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
                  .Append( root.parent.Position )
                  .Append( ", temp " )
                  .Append( netTemperature );
            return result.ToString();
        }
    }
}