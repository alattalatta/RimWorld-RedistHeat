using System;
using System.Linq;
using Verse;

namespace RedistHeat
{
    public enum NetLayer
    {
        Upper,
        Lower
    }
    public static class Common
    {
        public static int NetLayerCount()
        {
            return Enum.GetValues(typeof(NetLayer)).Length;
        }
        public static void WipeExistingPipe( IntVec3 pos )
        {
            var pipe =
                Find.ThingGrid.ThingsAt( pos ).ToList().Find(
                        s => s.def.defName == "RedistHeat_DuctPipeLower" || s.def.defName == "RedistHeat_DuctPipeUpper" );

            pipe?.Destroy( DestroyMode.Kill );
        }
        
    }
}