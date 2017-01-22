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
            //ResourceBank.UILower = ContentFinder<Texture2D>.Get("UI/Commands/Lower", true);
            //ResourceBank.UIUpper = ContentFinder<Texture2D>.Get("UI/Commands/Upper", true);
            AirNetManager.NotifyCompSpawn( this );
        }

        public override void PostDestroy( DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
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

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var current in base.CompGetGizmosExtra())
                yield return current;

            if(this.parent.Faction == Faction.OfPlayer)
            {
                Command_Action act = new Command_Action();

            act.defaultLabel = ResourceBank.CycleLayerLabel;
            act.defaultDesc = ResourceBank.CycleLayerDesc;
            act.icon = currentLayer == NetLayer.Lower ? ResourceBank.UILower : ResourceBank.UIUpper;
            act.activateSound = SoundDef.Named("DesignateMine");
            act.hotKey = KeyBindingDefOf.CommandColonistDraft;
            act.action = () =>
            {
                var oldLayer = currentLayer;
                currentLayer = currentLayer == NetLayer.Lower ? NetLayer.Upper : NetLayer.Lower;
                MoteMaker.ThrowText(parent.Position.ToVector3Shifted(), parent.Map,
                                        ResourceBank.CycleLayerMote.Translate(currentLayer.ToStringTranslated()) 
                                        );
                AirNetManager.NotifyCompLayerChange(this, oldLayer);
            };
                yield return act;
            }
        }
    }
}