using Verse;

namespace RedistHeat
{
    public class CompDuctSwitchableProperties : CompProperties
    {
        public bool connectedToNet = true;
        public CompDuctSwitchableProperties()
        {
            this.compClass = typeof(CompDuctSwitchable);
        }
    }
}
