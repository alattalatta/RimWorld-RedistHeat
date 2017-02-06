using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompDuctSwitchable : ThingComp
    {
        
        public override string CompInspectStringExtra()
        {
#if !DEBUG
            var result = new StringBuilder();
            result.Append(((Building_DuctSwitchable)parent).Net ? "Connected to the network" : "Connected to the room");

            return result.ToString();
#endif
#if DEBUG
            return "";
#endif
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var current in base.CompGetGizmosExtra())
                yield return current;

            if (parent.Faction == Faction.OfPlayer)
            {
                Command_Action act = new Command_Action();

                act.defaultLabel = ((Building_DuctSwitchable)parent).Net ? "Network" : "Room";
                act.defaultDesc = "Cycle between the duct network and the room";
                act.icon = ((Building_DuctSwitchable)parent).Net ? ResourceBank.UINetwork : ResourceBank.UIRoom;
                act.activateSound = SoundDef.Named("DesignateMine");
                act.hotKey = KeyBindingDefOf.CommandColonistDraft;
                act.action = () =>
                {
                    ((Building_DuctSwitchable)parent).CycleMode();
                    MoteMaker.ThrowText(((Building_DuctSwitchable)parent).Position.ToVector3Shifted(), ((Building_DuctSwitchable)parent).Map,
                                            "{0}".Translate(((Building_DuctSwitchable)parent).Net ? "Network" : "Room")
                                            );
                };
                yield return act;
            }
        }
    }
}