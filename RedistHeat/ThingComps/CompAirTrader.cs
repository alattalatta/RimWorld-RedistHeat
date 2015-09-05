using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAirTrader : CompAir
    {
        public int netTemp;

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();
            if ( netTemp == 0 )
                netTemp = (int) GenTemperature.OutdoorTemp;
        }

        public void EqualizeWithRoom( Room room, float targetTemp, float rate )
        {
            //Will do full push when EPS is 1
            var tempDiff = Mathf.Abs( room.Temperature - targetTemp );
            var tempRated = tempDiff*rate*(1-props.energyPerSecond);

            if ( targetTemp < room.Temperature )
            {
                room.Temperature = Mathf.Max(targetTemp, room.Temperature - tempRated);
            }
            else if ( targetTemp > room.Temperature )
            {
                room.Temperature = Mathf.Min(targetTemp, room.Temperature + tempRated);
            }
        }

        public void EqualizeWithNet( float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( netTemp - targetTemp );
            var tempRated = tempDiff*rate*props.energyPerSecond;

            if ( targetTemp < connectedNet.NetTemperature )
            {
                connectedNet.NetTemperature = Mathf.Max( targetTemp, netTemp - tempRated );
            }
            else if ( targetTemp > connectedNet.NetTemperature )
            {
                connectedNet.NetTemperature = Mathf.Min( targetTemp, netTemp + tempRated );
            }

            netTemp = (int)connectedNet.NetTemperature;
        }

        public void SetNetTemperatureDirect( float temp )
        {
            connectedNet.NetTemperature += temp;
            netTemp = (int)connectedNet.NetTemperature;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.LookValue( ref netTemp, "netTemp", 999 );
        }
    }
}