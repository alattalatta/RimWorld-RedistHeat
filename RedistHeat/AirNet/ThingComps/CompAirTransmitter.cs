using System.Collections.Generic;
using Verse;

namespace RedistHeat
{
    public class CompAirTransmitter : CompAir
    {
        private NetLayer layer;

        public override void PostSpawnSetup()
        {
            var compAirProps = props as CompAirTransmitterProperties;
            if ( compAirProps == null )
            {
                Log.Error( "LT-RH: Could not find CompAirTransmtterProperties for CompAirTransmitter! " );
                layer = NetLayer.Lower;
            }
            else
            {
                layer = compAirProps.layer;
            }

            base.PostSpawnSetup();
        }

        public override bool IsLayerOf( NetLayer ly )
        {
            return layer == ly;
        }

        public override IEnumerable< Command > CompGetGizmosExtra()
        {
            return null;
        }
    }
}