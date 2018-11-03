using Verse;

namespace RedistHeat
{
    public class AirNetTicker : MapComponent
    {
        public static bool doneInit;

        public AirNetTicker(Map map) : base(map) { }

        public override void MapComponentUpdate()
        {
            if (!doneInit)
            {
                Initialize(map);
            }
            AirNetManager.AirNetsUpdate(map);
            AirNetManager.UpdateMapDrawer();
        }


        public override void MapComponentTick()
        {
            if (!doneInit)
            {
                Initialize(map);
            }
            AirNetManager.AirNetsTick();
        }

        public static void Initialize(Map map)
        {
            AirNetGrid.Reinit(map);
            AirNetManager.Reinit();
            Log.Message("RedistHeat: Initialized RedistHeat.");
            doneInit = true;
        }
    }
}