
using Verse;

namespace RedistHeat
{
    public class AirNetTicker : MapComponent
    {
        private bool ready;

        public override void MapComponentUpdate()
        {
            if ( !ready )
            {
                return;
            }

            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer();
        }

        public override void MapComponentTick()
        {
            if ( Find.TickManager.TicksGame < 2 )
            {
                AirNetGrid.Reinit();
                AirNetManager.Reinit();
                Log.Message( "LT-RH: Initialized RedistHeat." );
                ready = true;
            }
            AirNetManager.AirNetsTick();
        }
    }
}
