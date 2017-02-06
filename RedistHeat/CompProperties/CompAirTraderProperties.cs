using Verse;

namespace RedistHeat
{
    public class CompAirTraderProperties : CompProperties
    {
        public float transferRate = 12f;
        public int units = 0;

        public CompAirTraderProperties()
        {
            this.compClass = typeof(CompAirTrader);
        }
    }
}
