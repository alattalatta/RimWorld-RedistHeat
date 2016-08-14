using Verse;

namespace RedistHeat
{
    public class CompAirTraderProperties : CompProperties
    {
        public float transferRate = 12f;

        public CompAirTraderProperties()
        {
            this.compClass = typeof(CompAirTrader);
        }
    }
}
