using Verse;

namespace RedistHeat
{
    public class CompAirTraderProperties : CompProperties
    {
        public float energyPerSecond = 12f;

        public CompAirTraderProperties()
        {
            this.compClass = typeof(CompAirTrader);
        }
    }
}
