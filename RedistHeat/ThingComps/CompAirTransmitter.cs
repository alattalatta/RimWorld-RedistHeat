using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
    public class CompAirTransmitter : CompAir
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            var compAirProps = props as CompAirTransmitterProperties;
            if (compAirProps == null)
            {
                Log.Error("RedistHeat: Could not find CompAirTransmtterProperties for CompAirTransmitter! ");
                currentLayer = NetLayer.Lower;
            }
            else
            {
                currentLayer = compAirProps.layer;
            }

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield break;
        }
    }
}