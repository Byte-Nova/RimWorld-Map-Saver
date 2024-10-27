using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

public static class AnimalScriber
{
    public static AnimalFile AnimalToString(Pawn animal)
    {
        AnimalFile animalFile = new AnimalFile();

        GetAnimalBioDetails(animal, animalFile);

        GetAnimalKind(animal, animalFile);

        GetAnimalFaction(animal, animalFile);

        GetAnimalHediffs(animal, animalFile);

        GetAnimalSkills(animal, animalFile);

        GetAnimalTransform(animal, animalFile);

        return animalFile;
    }

    public static Pawn StringToAnimal(AnimalFile animalFile)
    {
        PawnKindDef kind = SetAnimalKind(animalFile);

        Faction faction = SetAnimalFaction(animalFile);

        Pawn animal = SetAnimal(kind, faction, animalFile);

        SetAnimalBioDetails(animal, animalFile);

        SetAnimalHediffs(animal, animalFile);

        SetAnimalSkills(animal, animalFile);

        SetAnimalTransform(animal, animalFile);

        return animal;
    }

    private static void GetAnimalBioDetails(Pawn animal, AnimalFile animalFile)
    {
        try
        {
            animalFile.DefName = animal.def.defName;
            animalFile.Name = animal.LabelShortCap.ToString();
            animalFile.BiologicalAge = animal.ageTracker.AgeBiologicalTicks.ToString();
            animalFile.ChronologicalAge = animal.ageTracker.AgeChronologicalTicks.ToString();
            animalFile.Gender = animal.gender.ToString();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetAnimalKind(Pawn animal, AnimalFile animalFile)
    {
        try { animalFile.KindDef = animal.kindDef.defName; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetAnimalFaction(Pawn animal, AnimalFile animalFile)
    {
        try { animalFile.FactionDef = animal.Faction.def.defName; }
        catch { animalFile.FactionDef = Faction.OfPlayer.def.defName; }
    }

    private static void GetAnimalHediffs(Pawn animal, AnimalFile animalFile)
    {
        if (animal.health.hediffSet.hediffs.Count() > 0)
        {
            List<HediffComponent> toGet = new List<HediffComponent>();

            foreach (Hediff hd in animal.health.hediffSet.hediffs)
            {
                try
                {
                    HediffComponent component = new HediffComponent();
                    component.DefName = hd.def.defName;

                    if (hd.Part != null)
                    {
                        component.PartDefName = hd.Part.def.defName;
                        component.PartLabel = hd.Part.Label;
                    }

                    component.Severity = hd.Severity;
                    component.IsPermanent = hd.IsPermanent();

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            animalFile.Hediffs = toGet.ToArray();
        }
    }

    private static void GetAnimalSkills(Pawn animal, AnimalFile animalFile)
    {
        if (animal.training == null) return;

        List<TrainableComponent> toGet = new List<TrainableComponent>();

        foreach (TrainableDef trainable in DefDatabase<TrainableDef>.AllDefsListForReading)
        {
            try
            {
                TrainableComponent component = new TrainableComponent();
                component.DefName = trainable.defName;
                component.CanTrain = animal.training.CanAssignToTrain(trainable).Accepted;
                component.HasLearned = animal.training.HasLearned(trainable);
                component.IsDisabled = animal.training.GetWanted(trainable);

                toGet.Add(component);
            }
            catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
        }

        animalFile.Trainables = toGet.ToArray();
    }

    private static void GetAnimalTransform(Pawn animal, AnimalFile animalFile)
    {
        try
        {
            animalFile.Transform.Position = new int[] { animal.Position.x, animal.Position.y, animal.Position.z};
            animalFile.Transform.Rotation = animal.Rotation.AsInt;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static PawnKindDef SetAnimalKind(AnimalFile animalFile)
    {
        try { return DefDatabase<PawnKindDef>.AllDefs.First(fetch => fetch.defName == animalFile.DefName); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        return null;
    }

    private static Faction SetAnimalFaction(AnimalFile animalFile)
    {
        try { return Find.FactionManager.AllFactions.First(fetch => fetch.def.defName == animalFile.FactionDef); }
        catch { return Faction.OfPlayer; }
    }

    private static Pawn SetAnimal(PawnKindDef kind, Faction faction, AnimalFile animalFile)
    {
        try { return PawnGenerator.GeneratePawn(kind, faction); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        return null;
    }

    private static void SetAnimalBioDetails(Pawn animal, AnimalFile animalFile)
    {
        try
        {
            animal.Name = new NameSingle(animalFile.Name);
            animal.ageTracker.AgeBiologicalTicks = long.Parse(animalFile.BiologicalAge);
            animal.ageTracker.AgeChronologicalTicks = long.Parse(animalFile.ChronologicalAge);

            Enum.TryParse(animalFile.Gender, true, out Gender animalGender);
            animal.gender = animalGender;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetAnimalHediffs(Pawn animal, AnimalFile animalFile)
    {
        try
        {
            animal.health.RemoveAllHediffs();
            animal.health.Reset();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        if (animalFile.Hediffs.Length > 0)
        {
            for (int i = 0; i < animalFile.Hediffs.Length; i++)
            {
                try
                {
                    HediffComponent component = animalFile.Hediffs[i];
                    HediffDef hediffDef = DefDatabase<HediffDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.DefName);
                    BodyPartRecord bodyPart = animal.RaceProps.body.AllParts.ToList().Find(x => x.def.defName == component.PartDefName &&
                        x.Label == component.PartLabel);

                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, animal, bodyPart);
                    hediff.Severity = component.Severity;

                    if (component.IsPermanent)
                    {
                        HediffComp_GetsPermanent hediffComp = hediff.TryGetComp<HediffComp_GetsPermanent>();
                        hediffComp.IsPermanent = true;
                    }

                    animal.health.AddHediff(hediff);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetAnimalSkills(Pawn animal, AnimalFile animalFile)
    {
        if (animalFile.Trainables.Length > 0)
        {
            for (int i = 0; i < animalFile.Trainables.Length; i++)
            {
                try
                {
                    TrainableComponent component = animalFile.Trainables[i];
                    TrainableDef trainable = DefDatabase<TrainableDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.DefName);
                    if (component.CanTrain) animal.training.Train(trainable, null, complete: component.HasLearned);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetAnimalTransform(Pawn animal, AnimalFile animalFile)
    {
        try
        {
            animal.Position = new IntVec3(animalFile.Transform.Position[0], animalFile.Transform.Position[1], animalFile.Transform.Position[2]);
            animal.Rotation = new Rot4(animalFile.Transform.Rotation);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }
}