using Verse;
using UnityEngine;
// ReSharper disable All

namespace RedistHeat
{
	public static class StaticSet
	{
		public static string StringTargetTemperature = "TargetTemperature".Translate();
		public static string StringState = "RedistHeat_State".Translate();

		public static string StringWorking = "RedistHeat_Working".Translate();
		public static string StringNotWorking = "RedistHeat_NotWorking".Translate();

		public static string StringVentOpened = "RedistHeat_Opened".Translate();
		public static string StringVentClosed = "RedistHeat_Closed".Translate();

		public static string StringWorkingDucts = "RedistHeat_WorkingDucts".Translate();

		public static string StringNetworkID = "RedistHeat_NetworkID".Translate();
		public static string StringNetworkTemperature = "RedistHeat_NetworkTemperature".Translate();

		public static string StringExposeDuct = "RedistHeat_MustExposeDuct".Translate();
		public static string StringExposeBoth = "RedistHeat_MustExposeBothSides".Translate();
		public static string StringExposeHot = "RedistHeat_MustExposeHotSide".Translate();
		public static string StringExposeCold = "RedistHeat_MustExposeColdSide".Translate();
		public static string StringAttachToCooler = "RedistHeat_MustPlaceBackSideOnCooler".Translate();

		public static string StringUIRefreshIDLabel = "RedistHeat_CommandRefreshID".Translate();
		public static string StringUIRefreshIDDesc = "RedistHeat_CommandRefreshIDDesc".Translate();
		public static string StringUILockLabel = "Vent_CommandToggleLock".Translate();
		public static string StringUILockDesc = "Vent_CommandToggleLockDesc".Translate();

		public static readonly Texture2D UITempRaise = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true);
		public static readonly Texture2D UITempReset = ContentFinder<Texture2D>.Get("UI/Commands/TempReset", true);
		public static readonly Texture2D UITempLower = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true);

		public static readonly Texture2D UIRefreshID = ContentFinder<Texture2D>.Get("UI/Commands/TryReconnect", true);

		public static readonly Texture2D UILock = ContentFinder<Texture2D>.Get("UI/Commands/Lock", true);

		public static float ControlTemperatureTempChange(Room room, float energyLimit, float targetTemperature)
		{
			if (room == null)
				return 0.0f;
			var a = targetTemperature - room.Temperature;
			var b = energyLimit / room.CellCount;
			return (double)energyLimit <= 0.0 ? Mathf.Min(Mathf.Max(a, b), 0.0f) : Mathf.Max(Mathf.Min(a, b), 0.0f);
		}
		public static float ControlTemperatureTempChange(AirNet airNet, float energyLimit, float targetTemperature)
		{
			var a = targetTemperature - airNet.Temperature;
			var b = energyLimit / airNet.nodes.Count;
			return (double)energyLimit <= 0.0 ? Mathf.Min(Mathf.Max(a, b), 0.0f) : Mathf.Max(Mathf.Min(a, b), 0.0f);
		}
	}
}
