using Verse;

namespace RedistHeat
{
	class AirNetManager
	{

		//Overlay drawer update
		public static void NotifyDrawerForGridUpdate(IntVec3 pos)
		{
			Find.MapDrawer.MapChanged(pos, MapChangeType.PowerGrid, true, false);
		}
	}
}
