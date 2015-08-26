using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAir : ThingComp
    {
        public AirNet connectedNet;
        public NetLayer currentLayer = NetLayer.Lower;

        public virtual bool IsLayerOf( NetLayer ly )
        {
            return currentLayer == ly;
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
            AirOverlayMat.GetLayeredOverlayGraphic( this ).Print( layer, parent );
        }

        public override string CompInspectStringExtra()
        {
            OverlayDrawHandler.DrawPowerGridOverlayThisFrame();
            var result = new StringBuilder();
            result.Append( ResourceBank.CurrentConnectionChannel.Translate(currentLayer.ToStringTranslated()) );

            result.Append(ResourceBank.CurrentConnectedNetTemp);
            if ( connectedNet != null )
            {
                result.Append( Mathf.Round( connectedNet.NetTemperature).ToStringTemperature( "F0" ) );
            }

            return result.ToString();
        }

        public override IEnumerable< Command > CompGetGizmosExtra()
        {
            var iconTex = currentLayer == NetLayer.Lower ? ResourceBank.UILower : ResourceBank.UIUpper;
            var com = new Command_Action()
            {
                defaultLabel  = ResourceBank.CycleLayerLabel,
                defaultDesc   = ResourceBank.CycleLayerDesc,
                icon          = iconTex,
                activateSound = SoundDef.Named( "DesignateMine" ),
                hotKey        = KeyBindingDefOf.CommandColonistDraft,
                action        = () =>
                {
                    var oldLayer = currentLayer;
                    currentLayer = currentLayer == NetLayer.Lower ? NetLayer.Upper : NetLayer.Lower;
                    MoteThrower.ThrowText( parent.Position.ToVector3Shifted(), ResourceBank.CycleLayerMote.Translate(currentLayer.ToStringTranslated()) );
                    AirNetManager.NotifyCompLayerChange( this, oldLayer );
                }
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