using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAirTrader : CompAir
    {
        public int netTemp;

        public CompAirTraderProperties Props
        {
            get
            {
                return (CompAirTraderProperties)this.props;
            }
        }

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();
            if (netTemp == 0)
            {
                netTemp = (int) GenTemperature.OutdoorTemp;
            }
        }

        public void EqualizeWithRoom( Room room, float pointTemp, float rate )
        {
            //Will do full push when EPS is 1
            var tempDiff = Mathf.Abs( room.Temperature - pointTemp );
            var tempRated = tempDiff*rate*(1 - Props.energyPerSecond);

            if (pointTemp < room.Temperature)
            {
                room.Temperature = Mathf.Max( pointTemp, room.Temperature - tempRated );
            }
            else if (pointTemp > room.Temperature)
            {
                room.Temperature = Mathf.Min( pointTemp, room.Temperature + tempRated );
            }
        }

        public void EqualizeWithNet( float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( netTemp - targetTemp );
            var tempRated = tempDiff*rate*Props.energyPerSecond;

            if (targetTemp < connectedNet.NetTemperature)
            {
                connectedNet.NetTemperature = Mathf.Max( targetTemp, netTemp - tempRated );
            }
            else if (targetTemp > connectedNet.NetTemperature)
            {
                connectedNet.NetTemperature = Mathf.Min( targetTemp, netTemp + tempRated );
            }

            netTemp = (int) connectedNet.NetTemperature;
        }

        public void SetNetTemperatureDirect( float temp )
        {
            connectedNet.NetTemperature += temp;
            netTemp = (int) connectedNet.NetTemperature;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.LookValue( ref netTemp, "netTemp", 999 );
        }
    }
}