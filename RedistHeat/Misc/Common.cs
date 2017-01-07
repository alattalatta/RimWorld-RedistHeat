using System;
using System.Linq;
using System.Linq.Expressions;
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
            return Enum.GetValues( typeof(NetLayer) ).Length;
        }

        public static string ToStringTranslated( this NetLayer layer )
        {
            return ("RedistHeat_" + layer + "ChannelTranslated").Translate();
        }

        public static void WipeExistingPipe( IntVec3 pos)
        {
            var pipe =
                Find.VisibleMap.thingGrid.ThingsAt( pos ).ToList().Find(
                    s => s.def.defName == "RedistHeat_DuctPipeLower" || s.def.defName == "RedistHeat_DuctPipeUpper" );

            pipe?.Destroy();
        }
    }
}