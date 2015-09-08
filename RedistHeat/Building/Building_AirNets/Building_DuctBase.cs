using System.Linq;
using RimWorld;
using Verse;

namespace RedistHeat
{
    public class Building_DuctBase : Building_TempControl
    {
        public void PrintForAirGrid(SectionLayer layer)
        {
            foreach ( var current in AllComps.Where( s => s is CompAir ).Cast< CompAir >() )
            {
                current.CompPrintForAirGrid( layer );
            }
        }
    }
}
