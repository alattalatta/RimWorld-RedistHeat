
using Verse;

namespace RedistHeat
{
    public class AirNetTicker : MapComponent
    {
        public override void MapComponentUpdate()
        {
            AirNetManager.AirNetsUpdate();
        }

        public override void MapComponentTick()
        {
            AirNetManager.AirNetsTick();
        }
    }
}
