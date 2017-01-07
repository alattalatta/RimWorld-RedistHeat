using Verse;

namespace RedistHeat
{
    public class AirNetTicker : MapComponent
    {
        public static bool doneInit;

        public AirNetTicker(Map map) : base(map)
        {
        }

        public override void MapComponentUpdate()
        {
            if (!doneInit)
            {
                Initialize();
            }
            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer(map);
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