using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAir : CompAirBase
    {
        public AirNet connectedNet;

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();

            TryConnectTo();
            if ( connectedNet == null )
            {
                connectedNet = new AirNet( new List< CompAir > {this} );
            }
        }

        public override void PostDeSpawn()
        {
            base.PostDeSpawn();
            connectedNet.SplitNetAt( this );
        }

        public override string CompInspectStringExtra()
        {
            var str = new StringBuilder();
            if ( connectedNet == null )
            {
                return "No AirNet";
            }
            str.Append( StaticSet.StringNetworkID + ": " );
            str.Append( connectedNet.NetId + " / " );
            str.Append( StaticSet.StringNetworkTemperature + ": " );
            str.Append( Mathf.Round( connectedNet.NetTemperature ).ToStringTemperature( "F0" ) );

            return str.ToString();
        }

        public void TryConnectTo()
        {
            //Must check inside for underneath pipe
            foreach ( var c in GenAdj.CardinalDirectionsAndInside )
            {
                var compAir = AirNetGrid.AirNodeAt( c + Position ) as CompAir;
                if ( compAir == null || compAir.connectedNet == null )
                {
                    continue;
                }

                ConnectToNet( compAir.connectedNet );
            }
        }

        private void ConnectToNet( AirNet net )
        {
            if ( connectedNet == null )
            {
                net.RegisterNode( this );
            }
            else
            {
                connectedNet.MergeIntoNet( net );
            }
        }

        public override void CompPrintForPowerGrid( SectionLayer layer )
        {
            AirOverlayMat.LinkedOverlayGraphic.Print( layer, parent );
        }
    }
}