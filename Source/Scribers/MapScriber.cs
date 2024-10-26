using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

public static class MapScribeManager
{
    public static MapFile MapToString(Map map)
    {
        MapFile mapFile = new MapFile();

        GetMapSize(mapFile, map);

        GetMapTerrain(mapFile, map);

        GetMapThings(mapFile, map);

        return mapFile;
    }

    public static Map StringToMap(MapFile mapFile)
    {
        Map map = SetEmptyMap(mapFile);

        SetMapTerrain(mapFile, map);

        SetMapThings(mapFile, map);

        SetMapFog(map);

        SetMapRoofs(map);

        return map;
    }

    private static void GetMapSize(MapFile mapFile, Map map)
    {
        try { mapFile.Size = ValueParser.IntVec3ToArray(map.Size); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetMapTerrain(MapFile mapFile, Map map)
    {
        try 
        {
            List<TileComponent> toGet = new List<TileComponent>();

            for (int z = 0; z < map.Size.z; ++z)
            {
                for (int x = 0; x < map.Size.x; ++x)
                {
                    TileComponent component = new TileComponent();
                    IntVec3 vectorToCheck = new IntVec3(x, map.Size.y, z);
                    component.DefName = map.terrainGrid.TerrainAt(vectorToCheck).defName;
                    component.IsPolluted = map.pollutionGrid.IsPolluted(vectorToCheck);

                    if (map.roofGrid.RoofAt(vectorToCheck) == null) component.RoofDefName = "null";
                    else component.RoofDefName = map.roofGrid.RoofAt(vectorToCheck).defName;

                    toGet.Add(component);
                }
            }

            mapFile.Tiles = toGet.ToArray();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetMapThings(MapFile mapFile, Map map)
    {
        try 
        {
            List<ThingFile> thingsToAdd = new List<ThingFile>();

            foreach (Thing thing in map.listerThings.AllThings)
            {
                if (!ScribeHelper.CheckIfThingIsHuman(thing) && !ScribeHelper.CheckIfThingIsAnimal(thing))
                {
                    ThingFile thingData = ThingScribeManager.ItemToString(thing, thing.stackCount);
                    
                    if (ScribeHelper.CheckIfThingCanGrow(thing))
                    {
                        Plant plant = thing as Plant;
                        thingData.PlantComponent.GrowthTicks = plant.Growth;
                    }

                    thingsToAdd.Add(thingData);
                }
            }

            mapFile.Things = thingsToAdd.ToArray();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static Map SetEmptyMap(MapFile mapFile)
    {
        IntVec3 mapSize = ValueParser.ArrayToIntVec3(mapFile.Size);
        Map toReturn = GetOrGenerateMapUtility.GetOrGenerateMap(Find.CurrentMap.Tile, mapSize, null);

        return toReturn;
    }

    private static void SetMapTerrain(MapFile mapFile, Map map)
    {
        try
        {
            int index = 0;

            for (int z = 0; z < map.Size.z; ++z)
            {
                for (int x = 0; x < map.Size.x; ++x)
                {
                    TileComponent component = mapFile.Tiles[index];
                    IntVec3 vectorToCheck = new IntVec3(x, map.Size.y, z);

                    try
                    {
                        TerrainDef terrainToUse = DefDatabase<TerrainDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.DefName);
                        map.terrainGrid.SetTerrain(vectorToCheck, terrainToUse);
                        map.pollutionGrid.SetPolluted(vectorToCheck, component.IsPolluted);
                    }
                    catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

                    try
                    {
                        RoofDef roofToUse = DefDatabase<RoofDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.RoofDefName);
                        map.roofGrid.SetRoof(vectorToCheck, roofToUse);
                    }
                    catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

                    index++;
                }
            }
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetMapThings(MapFile mapFile, Map map)
    {
        try
        {
            foreach (ThingFile item in mapFile.Things)
            {
                try
                {
                    Thing toGet = ThingScribeManager.StringToItem(item);

                    if (ScribeHelper.CheckIfThingCanGrow(toGet))
                    {
                        Plant plant = toGet as Plant;
                        plant.Growth = item.PlantComponent.GrowthTicks;
                    }

                    GenPlace.TryPlaceThing(toGet, toGet.Position, map, ThingPlaceMode.Direct, rot: toGet.Rotation);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetMapFog(Map map)
    {
        try 
        { 
            FloodFillerFog.FloodUnfog(MapGenerator.PlayerStartSpot, map); 
            FloodFillerFog.DebugRefogMap(map);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetMapRoofs(Map map)
    {
        try
        {
            map.roofCollapseBuffer.Clear();
            map.roofGrid.Drawer.SetDirty();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }         
    }
}