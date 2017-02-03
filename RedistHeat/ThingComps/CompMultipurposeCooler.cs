using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompMultipurposeCooler : CompAir
    {
        public override string CompInspectStringExtra()
        {
#if !DEBUG
            var result = new StringBuilder();
            result.Append(((Building_DuctHeatExchanger)parent).Cooling ? "Cooling the network" : "Cooling the room");

            return result.ToString();
#endif
            return "";
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var current in base.CompGetGizmosExtra())
                yield return current;

            if(this.parent.Faction == Faction.OfPlayer)
            {
                Command_Action act = new Command_Action();

            act.defaultLabel = "Cycle Mode";
            act.defaultDesc = "Cycle between cooling the duct network and the room";
            act.icon = ((Building_MultipurposeCooler)parent).Cooling ? ResourceBank.UICooling : ResourceBank.UIHeating;
            act.activateSound = SoundDef.Named("DesignateMine");
            act.hotKey = KeyBindingDefOf.CommandColonistDraft;
            act.action = () =>
            {
                ((Building_MultipurposeCooler)parent).CycleMode();
                MoteMaker.ThrowText(parent.Position.ToVector3Shifted(), parent.Map,
                                        "{0}".Translate(((Building_MultipurposeCooler)parent).Cooling ? "Network" : "Room") 
                                        );
            };
                yield return act;
            }
        }
    }
}