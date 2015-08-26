
using System.Diagnostics;
using Verse;

namespace RedistHeat
{
    public class AirNetTicker : MapComponent
    {
        public bool doneInit;

        public override void MapComponentUpdate()
        {
            if(!doneInit)
                Initialize();
#if DEBUG
            //Log.Message( "Updating." );
#endif
            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer();
        }

        public override void MapComponentTick()
        {
            if(!doneInit)
                Initialize();

            AirNetManager.AirNetsTick();
        }

        private void Initialize()
        {
            var watch = new Stopwatch();

            
            watch.Start();
            /*
            //AirNetGrid.Reinit();
            //AirNetManager.Reinit();
            var elapsed = watch.ElapsedMilliseconds;

            Log.Message("LT-RH: " + elapsed + "ms: Initialized RedistHeat.");*/

            AirNetManager.Reload();
            var elapsed = watch.ElapsedMilliseconds;

            Log.Message( "LT-RH: " + elapsed + "ms: Reloaded NetManager.");
            doneInit = true;
            watch.Reset();
        }
    }
}
