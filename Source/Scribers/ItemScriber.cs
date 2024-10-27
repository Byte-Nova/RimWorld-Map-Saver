using System;
using System.Linq;
using RimWorld;
using Verse;

public static class ItemScriber
{
    public static ItemFile ItemToString(Thing thing, int thingCount)
    {
        ItemFile itemFile = new ItemFile();

        Thing toUse = null;
        if (GetItemMinified(thing, itemFile)) toUse = thing.GetInnerIfMinified();
        else toUse = thing;

        GetItemName(toUse, itemFile);

        GetItemMaterial(toUse, itemFile);

        GetItemQuantity(toUse, itemFile, thingCount);

        GetItemQuality(toUse, itemFile);

        GetItemHitpoints(toUse, itemFile);

        GetItemTransform(toUse, itemFile);

        if (ScribeHelper.CheckIfThingHasColor(thing)) GetColorDetails(toUse, itemFile);;
        return itemFile;
    }

    public static Thing StringToItem(ItemFile itemFile)
    {
        Thing thing = SetItem(itemFile);

        SetItemQuantity(thing, itemFile);

        SetItemQuality(thing, itemFile);

        SetItemHitpoints(thing, itemFile);

        SetItemTransform(thing, itemFile);

        if (ScribeHelper.CheckIfThingHasColor(thing)) SetColorDetails(thing, itemFile);
        return thing;
    }

    private static void GetItemName(Thing thing, ItemFile itemFile)
    {
        try { itemFile.DefName = thing.def.defName; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemMaterial(Thing thing, ItemFile itemFile)
    {
        try
        {
            if (ScribeHelper.CheckIfThingHasMaterial(thing)) itemFile.MaterialDefName = thing.Stuff.defName;
            else itemFile.MaterialDefName = null;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemQuantity(Thing thing, ItemFile itemFile, int thingCount)
    {
        try { itemFile.Quantity = thingCount; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemQuality(Thing thing, ItemFile itemFile)
    {
        try { itemFile.Quality = ScribeHelper.GetThingQuality(thing); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemHitpoints(Thing thing, ItemFile itemFile)
    {
        try { itemFile.Hitpoints = thing.HitPoints; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetItemTransform(Thing thing, ItemFile itemFile)
    {
        try
        {
            itemFile.TransformComponent.Position = new int[] { thing.Position.x, thing.Position.y, thing.Position.z };
            itemFile.TransformComponent.Rotation = thing.Rotation.AsInt;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static bool GetItemMinified(Thing thing, ItemFile itemFile)
    {
        try
        {
            itemFile.IsMinified = ScribeHelper.CheckIfThingIsMinified(thing);
            return itemFile.IsMinified;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        return false;
    }

    private static void GetColorDetails(Thing thing, ItemFile itemFile) 
    {
        itemFile.Color[0] = thing.DrawColor.r;
        itemFile.Color[1] = thing.DrawColor.g;
        itemFile.Color[2] = thing.DrawColor.b;
        itemFile.Color[3] = thing.DrawColor.a;
    }

    private static Thing SetItem(ItemFile itemFile)
    {
        try
        {
            ThingDef thingDef = DefDatabase<ThingDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == itemFile.DefName);
            ThingDef defMaterial = DefDatabase<ThingDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == itemFile.MaterialDefName);
            return ThingMaker.MakeThing(thingDef, defMaterial);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        throw new IndexOutOfRangeException(itemFile.ToString());
    }

    private static void SetItemQuantity(Thing thing, ItemFile itemFile)
    {
        try { thing.stackCount = itemFile.Quantity; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetItemQuality(Thing thing, ItemFile itemFile)
    {
        if (itemFile.Quality != -1)
        {
            try
            {
                CompQuality compQuality = thing.TryGetComp<CompQuality>();
                if (compQuality != null)
                {
                    QualityCategory iCategory = (QualityCategory)itemFile.Quality;
                    compQuality.SetQuality(iCategory, ArtGenerationContext.Outsider);
                }
            }
            catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
        }
    }

    private static void SetItemHitpoints(Thing thing, ItemFile itemFile)
    {
        try { thing.HitPoints = itemFile.Hitpoints; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetItemTransform(Thing thing, ItemFile itemFile)
    {
        try
        { 
            thing.Position = new IntVec3(itemFile.TransformComponent.Position[0], itemFile.TransformComponent.Position[1], itemFile.TransformComponent.Position[2]);
            thing.Rotation = new Rot4(itemFile.TransformComponent.Rotation);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetColorDetails(Thing thing, ItemFile itemFile) 
    {
        thing.SetColor(new UnityEngine.Color(
            itemFile.Color[0],
            itemFile.Color[1],
            itemFile.Color[2],
            itemFile.Color[3]));
    }
}