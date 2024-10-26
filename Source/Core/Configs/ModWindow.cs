using System.Diagnostics;
using UnityEngine;
using Verse;

public class ModWindow : Mod
{
    public static ModConfigs modConfigs;

    public ModWindow(ModContentPack content) : base(content) { modConfigs = GetSettings<ModConfigs>(); }

    public override string SettingsCategory() { return Master.modName; }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);

        listingStandard.GapLine();
        listingStandard.Label("Map Loader");
        if (listingStandard.ButtonTextLabeled("Save the map you are currently at", "Save")) { MapManager.OpenMapSaver(); }
        if (listingStandard.ButtonTextLabeled("Load a map from the list of saved ones", "Load")) { MapManager.OpenMapLoader(); }
        if (listingStandard.ButtonTextLabeled("Open maps folder", "Open")) { Process.Start(Master.modFolderPath); }

        listingStandard.GapLine();
        listingStandard.Label("Experimental");
        listingStandard.CheckboxLabeled("Enable verbose logs", ref modConfigs.verboseBool, "Enable verbose logs");

        listingStandard.End();
        base.DoSettingsWindowContents(inRect);
    }
}