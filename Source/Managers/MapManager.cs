using System;
using System.Collections.Generic;
using System.IO;
using Verse;

public static class MapManager
{
    private static readonly string mapExtension = ".map";

    public static void SaveMap(Map map, string saveName)
    {
        MapFile toSave = MapScribeManager.MapToString(map);

        CompressedFile compressedFile = new CompressedFile();
        compressedFile.Contents = GZip.Compress(Serializer.ConvertObjectToBytes(toSave));

        string location = Path.Combine(Master.modFolderPath, saveName + mapExtension);
        Serializer.SerializeToFile(location, compressedFile);

        Find.WindowStack.Add(new MessageWindow("Map was saved correctly!"));
    }

    public static void LoadMap(string path)
    {
        CompressedFile compressedFile = Serializer.SerializeFromFile<CompressedFile>(path);
        MapFile toLoad = Serializer.ConvertBytesToObject<MapFile>(GZip.Decompress(compressedFile.Contents));

        if (ValueParser.ArrayToIntVec3(toLoad.Size) != Find.CurrentMap.Size)
        {
            Find.WindowStack.Add(new MessageWindow("Map doesn't match the size of the current one!"));
        }

        else
        {
            Pawn[] toRecover = GetMapPawns(Find.CurrentMap);

            RemoveOldMap(Find.CurrentMap);

            MapScribeManager.StringToMap(toLoad);

            foreach (Pawn pawn in toRecover) GenSpawn.Spawn(pawn, pawn.Position, Find.CurrentMap, pawn.Rotation);

            Find.WindowStack.Add(new MessageWindow("Map was loaded correctly!"));
        }
    }

    public static void RenameMap(string currentPath, string newPath)
    {
        File.Move(currentPath, Path.Combine(Master.modFolderPath, newPath + mapExtension));
    }

    public static void DeleteMap(string path)
    {
        File.Delete(path);
    }

    public static void OpenMapSaver()
    {
        if (Current.ProgramState != ProgramState.Playing)
        {
            Find.WindowStack.Add(new MessageWindow("You must be playing to use this feature!"));
        }

        else
        {
            string title = "Save Map";
            string description = "Type the name of the map you want to save";
            Action toDo = delegate { SaveMap(Find.CurrentMap, PromptWindow.windowAnswer); };
            Find.WindowStack.Add(new PromptWindow(title, description, toDo));
        }
    }

    public static void OpenMapLoader()
    {
        if (Current.ProgramState != ProgramState.Playing)
        {
            Find.WindowStack.Add(new MessageWindow("You must be playing to use this feature!"));
        }

        else
        {
            string title = "Map Loader";
            string description = "This menu shows all your saved maps";
            Find.WindowStack.Add(new MapListingWindow(title, description, GetAllSavedMapNames()));
        }
    }

    public static void OpenMapDeleter(int mapIndex)
    {
        if (Current.ProgramState != ProgramState.Playing)
        {
            Find.WindowStack.Add(new MessageWindow("You must be playing to use this feature!"));
        }

        else
        {
            string description = "Are you sure you want to delete this map?";
            Action toDo = delegate { DeleteMap(Directory.GetFiles(Master.modFolderPath)[mapIndex]); };
            Find.WindowStack.Add(new YesNoWindow(description, toDo));
        }
    }

    public static void OpenMapRenamer(int mapIndex)
    {
        if (Current.ProgramState != ProgramState.Playing)
        {
            Find.WindowStack.Add(new MessageWindow("You must be playing to use this feature!"));
        }

        else
        {
            string title = "Rename Map";
            string description = "Type the new map name";
            Action toDo = delegate { RenameMap(Directory.GetFiles(Master.modFolderPath)[mapIndex], PromptWindow.windowAnswer); };
            Find.WindowStack.Add(new PromptWindow(title, description, toDo));
        }
    }

    private static void RemoveOldMap(Map map)
    {
        Thing[] toRemove = (Thing[])map.listerThings.AllThings.ToArray();
        foreach (Thing thing in toRemove) thing.DeSpawn();
    }

    private static Pawn[] GetMapPawns(Map map) { return map.mapPawns.AllPawns.ToArray(); }

    private static string[] GetAllSavedMapNames()
    {
        List<string> mapNames = new List<string>();
        foreach(string str in Directory.GetFiles(Master.modFolderPath))
        {
            mapNames.Add(Path.GetFileNameWithoutExtension(str));
        }

        return mapNames.ToArray();
    }
}