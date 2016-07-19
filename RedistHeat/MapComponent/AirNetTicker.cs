using Verse;

namespace RedistHeat
{
    public class AirNetTicker : MapComponent
    {
        public static bool doneInit;

        public override void MapComponentUpdate()
        {
            if (!doneInit)
            {
                Initialize();
            }
            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer();
        }


        public override void MapComponentTick()
        {
            if (!doneInit)
            {
                Initialize();
            }
            AirNetManager.AirNetsTick();
        }

        public static void Initialize()
        {
            AirNetGrid.Reinit();
            AirNetManager.Reinit();
            Log.Message("RedistHeat: Initialized RedistHeat.");
            doneInit = true;
        }
    }
}