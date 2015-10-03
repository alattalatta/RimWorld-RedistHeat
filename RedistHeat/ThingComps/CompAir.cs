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

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.LookValue( ref currentLayer, "currentLayer", NetLayer.Lower );
        }

        public void CompPrintForAirGrid( SectionLayer layer )
        {
            AirOverlayMat.GetLayeredOverlayGraphic( this ).Print( layer, parent );
        }

        public override string CompInspectStringExtra()
        {
            OverlayDrawHandler_AirNet.DrawAitNetOverlayThisFrame();
            var result = new StringBuilder();
            result.Append( ResourceBank.CurrentConnectionChannel.Translate( currentLayer.ToStringTranslated() ) );

            result.Append( ResourceBank.CurrentConnectedNetTemp );
            if (connectedNet != null)
            {
                result.Append( Mathf.Round( connectedNet.NetTemperature ).ToStringTemperature( "F0" ) );
#if DEBUG
                result.AppendLine().Append( "Debug ID: " ).Append( connectedNet.debugId );
#endif
            }

            return result.ToString();
        }

        public override IEnumerable< Command > CompGetGizmosExtra()
        {
            var com = new Command_Action
            {
                defaultLabel = ResourceBank.CycleLayerLabel,
                defaultDesc = ResourceBank.CycleLayerDesc,
                icon = currentLayer == NetLayer.Lower ? ResourceBank.UILower : ResourceBank.UIUpper,
                activateSound = SoundDef.Named( "DesignateMine" ),
                hotKey = KeyBindingDefOf.CommandColonistDraft,
                action = () =>
                {
                    var oldLayer = currentLayer;
                    currentLayer = currentLayer == NetLayer.Lower ? NetLayer.Upper : NetLayer.Lower;
                    MoteThrower.ThrowText( parent.Position.ToVector3Shifted(),
                                           ResourceBank.CycleLayerMote.Translate( currentLayer.ToStringTranslated() ) );
                    AirNetManager.NotifyCompLayerChange( this, oldLayer );
                }
            };

            foreach (var current in base.CompGetGizmosExtra())
                yield return current;

            yield return com;
        }
    }
}