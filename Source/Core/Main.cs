using System.Globalization;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace RWBoilerplate
{
    public static class Main
    {
        [StaticConstructorOnStartup]
        private static class YourModNameHere
        {
            static YourModNameHere()
            {
                ApplyHarmonyPathches();
                PrepareCulture();
            }
        }

        private static void ApplyHarmonyPathches()
        {
            Harmony harmony = new Harmony(Master.modName);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void PrepareCulture()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);
        }
    }
}
