using System.Linq;
using Verse;

namespace RedistHeat
{
	public static class Common
	{
		public static void DebugLog(string str)
		{
			if(Prefs.DevMode && Prefs.LogVerbose)
				Log.Message(str);
		}
		public static void DebugWarn(string str)
		{
			if (Prefs.DevMode && Prefs.LogVerbose)
				Log.Warning(str);
		}
		public static void WipeExistingPipe(IntVec3 pos)
		{
			var pipe = Find.ThingGrid.ThingsAt(pos).ToList().Find(s => s.def.defName == "RedistHeat_DuctPipe");
			pipe?.Destroy(DestroyMode.Cancel);
		}
	}
}
