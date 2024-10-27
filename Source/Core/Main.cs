using System.Globalization;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Verse;

public static class Main
{
    [StaticConstructorOnStartup]
    private static class MapSaver
    {
        static MapSaver()
        {
            ApplyHarmonyPathches();
            PrepareCulture();
            PreparePaths();

            DisplayLoadMessage();
        }
    }

    private static void ApplyHarmonyPathches()
    {
        Harmony harmony = new Harmony(Master.modName);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    private static void PrepareCulture()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
        CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);
    }

    private static void PreparePaths()
    {
        Master.mainPath = GenFilePaths.SaveDataFolderPath;

        Master.modFolderPath = Path.Combine(Master.mainPath, "Map Saver");
        if (!Directory.Exists(Master.modFolderPath)) Directory.CreateDirectory(Master.modFolderPath);
    }

    private static void DisplayLoadMessage() { Logger.Message("Mod loaded correctly!"); }
}
