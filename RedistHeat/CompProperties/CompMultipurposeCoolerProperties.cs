using Verse;

namespace RedistHeat
{
    public class CompMultipurposeCoolerProperties : CompProperties
    {
        public float transferRate = 12f;
        public int units = 1;

        public CompMultipurposeCoolerProperties()
        {
            this.compClass = typeof(CompMultipurposeCooler);
        }
    }
}
