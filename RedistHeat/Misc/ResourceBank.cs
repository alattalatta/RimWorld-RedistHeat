using System.Security.Policy;
using UnityEngine;
using Verse;

// ReSharper disable All

namespace RedistHeat
{
    [StaticConstructorOnStartup]
    public static class ResourceBank
    {
        public const string modName = "RedistHeat";

        // ===== AirNet.xml ===== //
        public static string CurrentConnectionChannel = "RedistHeat_CurrentConnectionChannel";
        public static string CurrentConnectedNetTemp = "RedistHeat_CurrentConnectedNetTemp".Translate();
        public static string LowerNetTemperature = "RedistHeat_LowerNetTemperature".Translate();
        public static string UpperNetTemperature = "RedistHeat_UpperNetTemperature".Translate();
        public static string NeedsWall = "RedistHeat_NeedsWall".Translate();
        public static string WallAlreadyOccupied = "RedistHeat_WallAlreadyOccupied".Translate();
        public static string CycleLayerLabel = "RedistHeat_CycleLayerLabel".Translate();
        public static string CycleLayerDesc = "RedistHeat_CycleLayerDesc".Translate();
        public static string CycleLayerMote = "RedistHeat_CycleLayerMote";
        public static string DeconstructReversed = "RedistHeat_DesignatorDeconstructReverse".Translate();
        public static string DeconstructReversedDesc = "RedistHeat_DesignatorDeconstructReverseDesc".Translate();

        // ===== TempControl.xml ===== //
        public static string StringState = "RedistHeat_State".Translate();
        public static string StringWorking = "RedistHeat_Working".Translate();
        public static string StringNotWorking = "RedistHeat_NotWorking".Translate();
        public static string StringWorkingDucts = "RedistHeat_WorkingDucts".Translate();

        public static string ExposeHot = "RedistHeat_MustExposeHotSide".Translate();
        public static string ExposeDuct = "RedistHeat_MustExposeDuct".Translate();
        public static string ExposeBoth = "MustPlaceVentWithFreeSpaces".Translate(); // Core mod key text
        public static string ExposeCold = "RedistHeat_MustExposeColdSide".Translate();
        public static string AttachToCooler = "RedistHeat_MustPlaceBackSideOnCooler".Translate();
        public static string NotNearWithOther = "RedistHeat_MustBeNotNearWithOther".Translate();
        public static string NeedConstructedRoof = "RedistHeat_NeedConstructedRoof".Translate();

        // ==== Vent.xml ==== //
        public static string StringToggleAirflowLabel = "RedistHeat_CommandToggleLockLabel".Translate();
        public static string StringToggleAirflowDesc = "RedistHeat_CommandToggleLockDesc".Translate();

        // === Gizmo icons === //
        public static Texture2D UILock = ContentFinder<Texture2D>.Get("UI/Commands/Forbidden", true );
        public static Texture2D UILower = ContentFinder<Texture2D>.Get("UI/Commands/Lower", true);
        public static Texture2D UIUpper = ContentFinder<Texture2D>.Get("UI/Commands/Upper", true);
        public static Texture2D UINetwork = ContentFinder<Texture2D>.Get("UI/Floats/In", true);
        public static Texture2D UIRoom = ContentFinder<Texture2D>.Get("UI/Floats/Out", true);

        // == Pipe textures == //

        public static Graphic graphicSingleUpper = GraphicDatabase.Get<Graphic_Single>("Things/Building/Linked/DuctPipeUpper");
        public static Graphic graphicSingleLower = GraphicDatabase.Get<Graphic_Single>("Things/Building/Linked/DuctPipeLower");
    }
}