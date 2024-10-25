using UnityEngine;
using Verse;

namespace RWBoilerplate
{
    public class ModWindow : Mod
    {
        private readonly ModConfigs modConfigs;

        public ModWindow(ModContentPack content) : base(content)
        {
            modConfigs = GetSettings<ModConfigs>();
        }

        public override string SettingsCategory() { return Master.modName; }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Change test bool", ref modConfigs.testBool, "Change the test bool");
            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }
    }
}
