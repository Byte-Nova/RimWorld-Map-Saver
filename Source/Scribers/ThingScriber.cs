using System;
using System.Linq;
using RimWorld;
using Verse;

public static class ThingScribeManager
{
    public static ThingFile ItemToString(Thing thing, int thingCount)
    {
        ThingFile thingData = new ThingFile();

        Thing toUse = null;
        if (GetItemMinified(thing, thingData)) toUse = thing.GetInnerIfMinified();
        else toUse = thing;

        GetItemName(toUse, thingData);

        GetItemMaterial(toUse, thingData);

        GetItemQuantity(toUse, thingData, thingCount);

        GetItemQuality(toUse, thingData);

        GetItemHitpoints(toUse, thingData);

        GetItemTransform(toUse, thingData);

        if (ScribeHelper.CheckIfThingHasColor(thing)) GetColorDetails(toUse, thingData);;
        return thingData;
    }

    public static Thing StringToItem(ThingFile thingData)
    {
        Thing thing = SetItem(thingData);

        SetItemQuantity(thing, thingData);

        SetItemQuality(thing, thingData);

        SetItemHitpoints(thing, thingData);

        SetItemTransform(thing, thingData);

        if (ScribeHelper.CheckIfThingHasColor(thing)) SetColorDetails(thing, thingData);
        return thing;
    }

    private static void GetItemName(Thing thing, ThingFile thingData)
    {
        try { thingData.DefName = thing.def.defName; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemMaterial(Thing thing, ThingFile thingData)
    {
        try
        {
            if (ScribeHelper.CheckIfThingHasMaterial(thing)) thingData.MaterialDefName = thing.Stuff.defName;
            else thingData.MaterialDefName = null;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemQuantity(Thing thing, ThingFile thingData, int thingCount)
    {
        try { thingData.Quantity = thingCount; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemQuality(Thing thing, ThingFile thingData)
    {
        try { thingData.Quality = ScribeHelper.GetThingQuality(thing); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemHitpoints(Thing thing, ThingFile thingData)
    {
        try { thingData.Hitpoints = thing.HitPoints; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemTransform(Thing thing, ThingFile thingData)
    {
        try
        {
            thingData.TransformComponent.Position = new int[] { thing.Position.x, thing.Position.y, thing.Position.z };
            thingData.TransformComponent.Rotation = thing.Rotation.AsInt;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static bool GetItemMinified(Thing thing, ThingFile thingData)
    {
        try
        {
            thingData.IsMinified = ScribeHelper.CheckIfThingIsMinified(thing);
            return thingData.IsMinified;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        return false;
    }

    private static void GetColorDetails(Thing thing, ThingFile thingData) 
    {
        thingData.Color[0] = thing.DrawColor.r;
        thingData.Color[1] = thing.DrawColor.g;
        thingData.Color[2] = thing.DrawColor.b;
        thingData.Color[3] = thing.DrawColor.a;
    }

    private static Thing SetItem(ThingFile thingData)
    {
        try
        {
            ThingDef thingDef = DefDatabase<ThingDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == thingData.DefName);
            ThingDef defMaterial = DefDatabase<ThingDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == thingData.MaterialDefName);
            return ThingMaker.MakeThing(thingDef, defMaterial);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        throw new IndexOutOfRangeException(thingData.ToString());
    }

    private static void SetItemQuantity(Thing thing, ThingFile thingData)
    {
        try { thing.stackCount = thingData.Quantity; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetItemQuality(Thing thing, ThingFile thingData)
    {
        if (thingData.Quality != -1)
        {
            try
            {
                CompQuality compQuality = thing.TryGetComp<CompQuality>();
                if (compQuality != null)
                {
                    QualityCategory iCategory = (QualityCategory)thingData.Quality;
                    compQuality.SetQuality(iCategory, ArtGenerationContext.Outsider);
                }
            }
            catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
        }
    }

    private static void SetItemHitpoints(Thing thing, ThingFile thingData)
    {
        try { thing.HitPoints = thingData.Hitpoints; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetItemTransform(Thing thing, ThingFile thingData)
    {
        try
        { 
            thing.Position = new IntVec3(thingData.TransformComponent.Position[0], thingData.TransformComponent.Position[1], thingData.TransformComponent.Position[2]);
            thing.Rotation = new Rot4(thingData.TransformComponent.Rotation);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetColorDetails(Thing thing, ThingFile thingData) 
    {
        thing.SetColor(new UnityEngine.Color(
            thingData.Color[0],
            thingData.Color[1],
            thingData.Color[2],
            thingData.Color[3]));
    }
}