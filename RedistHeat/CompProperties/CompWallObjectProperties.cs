using Verse;

namespace RedistHeat
{
    class CompWallObjectProperties : CompProperties
    {
        public int checkTickInterval = 120;
        public CompWallObjectProperties()
        {
            this.compClass = typeof(CompWallObject);
        }
    }
}
