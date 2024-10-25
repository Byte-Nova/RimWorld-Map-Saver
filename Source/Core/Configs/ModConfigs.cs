using Verse;

namespace RWBoilerplate
{
    public class ModConfigs : ModSettings
    {
        public bool testBool;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref testBool, "testBool");
            
            base.ExposeData();
        }
    }
}
