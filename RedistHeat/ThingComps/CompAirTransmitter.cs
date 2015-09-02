using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
    public class CompAirTransmitter : CompAir
    {
        public override void PostSpawnSetup()
        {
            var compAirProps = props as CompAirTransmitterProperties;
            if ( compAirProps == null )
            {
                Log.Error( "LT-RH: Could not find CompAirTransmtterProperties for CompAirTransmitter! " );
                currentLayer = NetLayer.Lower;
            }
            else
            {
                currentLayer = compAirProps.layer;
            }

            base.PostSpawnSetup();
        }

        public override IEnumerable< Command > CompGetGizmosExtra()
        {
            yield break;
        }
    }
}