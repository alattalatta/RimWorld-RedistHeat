using UnityEngine;
using Verse;

// ReSharper disable All

namespace RedistHeat
{
    public static class ResourceBank
    {
        // ===== AirNet.xml ===== //
        public static string StringLowerNetTemperature  = "RedistHeat_LowerNetTemperature".Translate();
        public static string StringUpperNetTemperature  = "RedistHeat_UpperNetTemperature".Translate();
        public static string StringCycleLayerLabel      = "RedistHeat_CycleLayerLabel".Translate();
        public static string StringCycleLayerDesc       = "RedistHeat_CycleLayerDesc".Translate();
        
        // ===== TempControl.xml ===== //
        public static string StringState                = "RedistHeat_State".Translate();
        public static string StringWorking              = "RedistHeat_Working".Translate();
        public static string StringNotWorking           = "RedistHeat_NotWorking".Translate();

        public static string StringWorkingDucts         = "RedistHeat_WorkingDucts".Translate();

        public static string StringExposeDuct           = "RedistHeat_MustExposeDuct".Translate();
        public static string StringExposeBoth           = "MustPlaceVentWithFreeSpaces".Translate();
        public static string StringExposeHot            = "RedistHeat_MustExposeHotSide".Translate();
        public static string StringExposeCold           = "RedistHeat_MustExposeColdSide".Translate();
        public static string StringAttachToCooler       = "RedistHeat_MustPlaceBackSideOnCooler".Translate();

        // ==== Vent.xml ==== //
        
        public static string StringToggleAirflowLabel   = "RedistHeat_CommandToggleLockLabel".Translate();
        public static string StringToggleAirflowDesc    = "RedistHeat_CommandToggleLockDesc".Translate();

        public static readonly Texture2D UILock         = ContentFinder< Texture2D >.Get( "UI/Commands/Forbidden", true );
    }
}