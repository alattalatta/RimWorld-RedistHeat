using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class CompAirTrader : CompAir
    {
        public float netTemp;

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
            if (netTemp == 0f)
            {
                netTemp = this.parent.Map.mapTemperature.OutdoorTemp;
            }
        }

        public void EqualizeWithRoom( Room room, float targetTemp, float rate )
        {
            //Will do full push when transferRate is 1
            var tempDiff = Mathf.Abs( room.Temperature - targetTemp);
            var tempRated = tempDiff*rate*(1 - Props.transferRate);

//#if DEBUG
//            Log.Message("RedistHeat: Device: " + this + ", targetTemp: " + targetTemp + ", room: " + room.Temperature + ", rated: " + tempRated);
//#endif

            if (targetTemp < room.Temperature)
            {
                room.Temperature = Mathf.Max(targetTemp, room.Temperature - tempRated );
            }
            else if (targetTemp > room.Temperature)
            {
                room.Temperature = Mathf.Min(targetTemp, room.Temperature + tempRated );
            }
        }

        public void EqualizeWithNet( float targetTemp, float rate )
        {
            var tempDiff = Mathf.Abs( netTemp - targetTemp );
            var tempRated = tempDiff*rate*Props.transferRate;

            if (targetTemp < connectedNet.NetTemperature)
            {
                connectedNet.NetTemperature = Mathf.Max(targetTemp, netTemp - tempRated);
            }
            else if (targetTemp > connectedNet.NetTemperature)
            {
//#if DEBUG
//                Log.Message("RedistHeat: PreCooling ----- targetTemp: " + targetTemp + ", net: " + netTemp + ", rated: " + tempRated + ", net+rated: " + (netTemp + tempRated));
//#endif
                connectedNet.NetTemperature = Mathf.Min(targetTemp, netTemp + tempRated);
            }
            netTemp = connectedNet.NetTemperature;            
        }

        public void SetNetTemperatureDirect( float temp )
        {
            connectedNet.NetTemperature += temp;
            netTemp = connectedNet.NetTemperature;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.LookValue( ref netTemp, "netTemp", 999 );
        }
    }
}