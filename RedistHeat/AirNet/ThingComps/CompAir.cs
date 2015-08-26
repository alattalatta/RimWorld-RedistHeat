using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAir : ThingComp
    {
        public AirNet []connectedNet = new AirNet[Common.NetLayerCount()];
        public NetLayer currentLayer = NetLayer.Lower;

        public virtual bool IsLayerOf( NetLayer ly )
        {
            return currentLayer == ly;
        }

        public virtual void ResetAirVars()
        {
            for(var i = 0; i < Common.NetLayerCount(); i++)
                connectedNet[i] = null;

            currentLayer = NetLayer.Lower;
        }

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();
            AirNetManager.NotifyCompSpawn( this );
        }

        public override void PostDestroy( DestroyMode mode = DestroyMode.Vanish )
        {
            base.PostDestroy( mode );
            AirNetManager.NotifyCompDespawn( this );
        }

        public override void CompPrintForPowerGrid( SectionLayer layer )
        {
            foreach(var current in AirOverlayMat.GetLayeredOverlayGraphics( this ))
                current.Print( layer, parent );
        }

        public override string CompInspectStringExtra()
        {
            var result = new StringBuilder();
            if ( connectedNet[(int) NetLayer.Lower] == null )
            {
                result.Append( ResourceBank.StringLowerNetTemperature + ": " );
                result.Append( Mathf.Round( connectedNet[(int) NetLayer.Lower].NetTemperature).ToStringTemperature( "F0" ) );
            }
            if ( connectedNet[(int)NetLayer.Upper] == null )
            {
                result.Append(ResourceBank.StringUpperNetTemperature + ": ");
                result.Append( Mathf.Round( connectedNet[(int) NetLayer.Upper].NetTemperature ).ToStringTemperature( "F0" ) );
            }

            return result.ToString();
        }

        public override IEnumerable< Command > CompGetGizmosExtra()
        {
            var com = new Command_Action()
            {
                defaultLabel = "RedistHeat_CycleLayerLabel",
                defaultDesc = "RedistHeat_CycleLayerDesc",
                icon = Texture2D.blackTexture,
                action = () =>
                {
                    var oldLayer = currentLayer;
                    var nextLayerInt = (int)oldLayer + 1;
                    if ( nextLayerInt == Common.NetLayerCount() )
                        nextLayerInt = 0;

                    currentLayer = (NetLayer) nextLayerInt;
                    AirNetManager.NotifyCompLayerChange( this, oldLayer );
                },
                activateSound = SoundDef.Named( "DesignatePlaceBuilding" ),
                hotKey        = KeyBindingDefOf.CommandColonistDraft
            };

            foreach ( var current in base.CompGetGizmosExtra() )
                yield return current;

            yield return com;
        }

        /*
        public void TryConnectTo()
        {
            //Must check inside for underneath pipe
            foreach ( var c in GenAdj.CardinalDirectionsAndInside )
            {
                var compAir = AirNetGrid.NetAt( c + Position ) as CompAir;
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
        */
    }
}