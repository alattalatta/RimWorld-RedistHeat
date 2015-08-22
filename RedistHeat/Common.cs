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
	}
}
