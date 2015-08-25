using Verse;

namespace RedistHeat
{
    public static class AirNetManager
    {

        //Overlay drawer update
        public static void NotifyDrawerForGridUpdate(IntVec3 pos)
        {
            Find.MapDrawer.MapMeshDirty(pos, MapMeshFlag.PowerGrid, true, false);
        }
    }
}
