using Verse;

public class ModConfigs : ModSettings
{
    public bool verboseBool;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref verboseBool, "verboseBool");
        
        base.ExposeData();
    }
}