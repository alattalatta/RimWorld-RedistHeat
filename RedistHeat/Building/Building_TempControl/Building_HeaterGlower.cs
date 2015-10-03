using Verse;

namespace RedistHeat
{
    public class Building_HeaterGlower : Building
    {
        private Building_MediumHeater heater;

        public void Reinit( Building_MediumHeater h )
        {
            heater = h;
        }

        public override void Tick()
        {
            if (!this.IsHashIntervalTick( 60 ))
                return;
            
            if (heater == null || heater.glower != this)
            {
                Destroy();
            }
        }
    }
}
