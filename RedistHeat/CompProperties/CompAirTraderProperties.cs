using Verse;

namespace RedistHeat
{
    public class CompAirTraderProperties : CompProperties
    {
        public float transferRate = 12f;
        public int units = 1;

        public CompAirTraderProperties()
        {
            this.compClass = typeof(CompAirTrader);
        }
    }
}
