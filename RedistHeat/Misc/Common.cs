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
        public const float AbsoluteZero = -273.15f;
        
        public static int NetLayerCount()
        {
            return Enum.GetValues( typeof(NetLayer) ).Length;
        }

        public static string ToStringTranslated( this NetLayer layer )
        {
            return ("RedistHeat_" + layer + "ChannelTranslated").Translate();
        }

        public static void WipeExistingPipe(Map map, IntVec3 pos)
        {
            foreach (var pipe in map.thingGrid.ThingsAt(pos).Select(s => s as Building_DuctPipe))
            {
                pipe?.Destroy();
            }
        }
    }
}