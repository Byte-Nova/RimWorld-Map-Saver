using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine.Assertions.Must;
using Verse;

public static class HumanScriber
{
    public static HumanFile HumanToString(Pawn pawn, bool passInventory = true)
    {
        HumanFile humanFile = new HumanFile();

        GetHumanBioDetails(pawn, humanFile);

        GetHumanKind(pawn, humanFile);

        GetHumanFaction(pawn, humanFile);

        GetHumanHediffs(pawn, humanFile);

        if (ModsConfig.BiotechActive)
        {
            GetHumanChildState(pawn, humanFile);

            GetHumanXenotype(pawn, humanFile);

            GetHumanXenogenes(pawn, humanFile);

            GetHumanEndogenes(pawn, humanFile);
        }

        GetHumanStory(pawn, humanFile);

        GetHumanSkills(pawn, humanFile);

        GetHumanTraits(pawn, humanFile);

        GetHumanApparel(pawn, humanFile);

        GetHumanEquipment(pawn, humanFile);

        if (passInventory) GetHumanInventory(pawn, humanFile);

        GetHumanFavoriteColor(pawn, humanFile);

        GetHumanTransform(pawn, humanFile);

        return humanFile;
    }

    public static Pawn StringToHuman(HumanFile humanFile)
    {
        PawnKindDef kind = SetHumanKind(humanFile);

        Faction faction = SetHumanFaction(humanFile);

        Pawn pawn = SetHuman(kind, faction, humanFile);

        SetHumanHediffs(pawn, humanFile);

        if (ModsConfig.BiotechActive)
        {
            SetHumanChildState(pawn, humanFile);

            SetHumanXenotype(pawn, humanFile);

            SetHumanXenogenes(pawn, humanFile);

            SetHumanEndogenes(pawn, humanFile);
        }

        SetHumanBioDetails(pawn, humanFile);

        SetHumanStory(pawn, humanFile);

        SetHumanSkills(pawn, humanFile);

        SetHumanTraits(pawn, humanFile);

        SetHumanApparel(pawn, humanFile);

        SetHumanEquipment(pawn, humanFile);

        SetHumanInventory(pawn, humanFile);

        SetHumanFavoriteColor(pawn, humanFile);

        SetHumanTransform(pawn, humanFile);

        return pawn;
    }

    private static void GetHumanBioDetails(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            humanFile.DefName = pawn.def.defName;
            humanFile.Name = pawn.LabelShortCap.ToString();
            humanFile.BiologicalAge = pawn.ageTracker.AgeBiologicalTicks.ToString();
            humanFile.ChronologicalAge = pawn.ageTracker.AgeChronologicalTicks.ToString();
            humanFile.Gender = pawn.gender.ToString();
            
            humanFile.HairDefName = pawn.story.hairDef.defName.ToString();
            humanFile.HairColor = pawn.story.HairColor.ToString();
            humanFile.HeadTypeDefName = pawn.story.headType.defName.ToString();
            humanFile.SkinColor = pawn.story.SkinColor.ToString();
            humanFile.BeardDefName = pawn.style.beardDef.defName.ToString();
            humanFile.BodyTypeDefName = pawn.story.bodyType.defName.ToString();
            humanFile.FaceTattooDefName = pawn.style.FaceTattoo.defName.ToString();
            humanFile.BodyTattooDefName = pawn.style.BodyTattoo.defName.ToString();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetHumanKind(Pawn pawn, HumanFile humanFile)
    {
        try { humanFile.KindDef = pawn.kindDef.defName; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetHumanFaction(Pawn pawn, HumanFile humanFile)
    {
        try { humanFile.FactionDef = pawn.Faction.def.defName; }
        catch { humanFile.FactionDef = Faction.OfPlayer.def.defName; }
    }

    private static void GetHumanHediffs(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.health.hediffSet.hediffs.Count() > 0)
        {
            List<HediffComponent> toGet = new List<HediffComponent>();

            foreach (Hediff hd in pawn.health.hediffSet.hediffs)
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

                    if (hd.def.CompProps<HediffCompProperties_Immunizable>() != null) component.Immunity = pawn.health.immunity.GetImmunity(hd.def);
                    else component.Immunity = -1f;

                    if (hd.def.tendable)
                    {
                        HediffComp_TendDuration comp = hd.TryGetComp<HediffComp_TendDuration>();
                        if (comp.IsTended)
                        {
                            component.TendQuality = comp.tendQuality;
                            component.TendDuration = comp.tendTicksLeft;
                        } 

                        else 
                        {
                            component.TendDuration = -1;
                            component.TendQuality = -1;
                        }

                        if (comp.TProps.disappearsAtTotalTendQuality >= 0)
                        {
                            Type type = comp.GetType();
                            FieldInfo fieldInfo = type.GetField("totalTendQuality", BindingFlags.NonPublic | BindingFlags.Instance);
                            component.TotalTendQuality = (float)fieldInfo.GetValue(comp);
                        }
                        else component.TotalTendQuality = -1f;
                    } 

                    else 
                    {
                        component.TendDuration = -1;
                        component.TendQuality = -1;
                        component.TotalTendQuality = -1f;
                    }

                    component.Severity = hd.Severity;
                    component.IsPermanent = hd.IsPermanent();

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            humanFile.Hediffs = toGet.ToArray();
        }
    }

    private static void GetHumanChildState(Pawn pawn, HumanFile humanFile)
    {
        try { humanFile.GrowthPoints = pawn.ageTracker.growthPoints; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetHumanXenotype(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            if (pawn.genes.Xenotype != null) humanFile.Xenotype.DefName = pawn.genes.Xenotype.defName.ToString();
            else humanFile.Xenotype.DefName = "null";

            if (pawn.genes.CustomXenotype != null) humanFile.Xenotype.CustomXenotypeName = pawn.genes.xenotypeName.ToString();
            else humanFile.Xenotype.CustomXenotypeName = "null";
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetHumanXenogenes(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.genes.Xenogenes.Count() > 0)
        {
            List<XenogeneComponent> toGet = new List<XenogeneComponent>();

            foreach (Gene gene in pawn.genes.Xenogenes)
            {
                try                 
                { 
                    XenogeneComponent component = new XenogeneComponent();
                    component.DefName = gene.def.defName;

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            humanFile.Xenogenes = toGet.ToArray();
        }
    }

    private static void GetHumanEndogenes(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.genes.Endogenes.Count() > 0)
        {
            List<EndogeneComponent> toGet = new List<EndogeneComponent>();

            foreach (Gene gene in pawn.genes.Endogenes)
            {
                try 
                {  
                    EndogeneComponent component = new EndogeneComponent();
                    component.DefName = gene.def.defName;

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            humanFile.Endogenes = toGet.ToArray();
        }
    }

    private static void GetHumanFavoriteColor(Pawn pawn, HumanFile humanFile)
    {
        try { humanFile.FavoriteColor = pawn.story.favoriteColor.ToString(); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetHumanStory(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            if (pawn.story.Childhood != null) humanFile.Stories.ChildhoodStoryDefName = pawn.story.Childhood.defName.ToString();
            else humanFile.Stories.ChildhoodStoryDefName = "null";

            if (pawn.story.Adulthood != null) humanFile.Stories.AdulthoodStoryDefName = pawn.story.Adulthood.defName.ToString();
            else humanFile.Stories.AdulthoodStoryDefName = "null";
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void GetHumanSkills(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.skills.skills.Count() > 0)
        {
            List<SkillComponent> toGet = new List<SkillComponent>();

            foreach (SkillRecord skill in pawn.skills.skills)
            {
                try
                {
                    SkillComponent component = new SkillComponent();
                    component.DefName = skill.def.defName;
                    component.Level = skill.levelInt;
                    component.Passion = skill.passion.ToString();

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            humanFile.Skills = toGet.ToArray();
        }
    }

    private static void GetHumanTraits(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.story.traits.allTraits.Count() > 0)
        {
            List<TraitComponent> toGet = new List<TraitComponent>();

            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                try
                {
                    TraitComponent component = new TraitComponent();
                    component.DefName = trait.def.defName;
                    component.Degree = trait.Degree;

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            humanFile.Traits = toGet.ToArray();
        }
    }

    private static void GetHumanApparel(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.apparel.WornApparel.Count() > 0)
        {
            List<ApparelComponent> toGet = new List<ApparelComponent>();

            foreach (Apparel ap in pawn.apparel.WornApparel)
            {
                try
                {
                    ItemFile thingData = ItemScriber.ItemToString(ap, 1);
                    ApparelComponent component = new ApparelComponent();
                    component.EquippedApparel = thingData;
                    component.WornByCorpse = ap.WornByCorpse;

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            humanFile.Apparel = toGet.ToArray();
        }
    }

    private static void GetHumanEquipment(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.equipment.Primary != null)
        {
            try
            {
                ThingWithComps weapon = pawn.equipment.Primary;
                ItemFile thingData = ItemScriber.ItemToString(weapon, weapon.stackCount);
                humanFile.Weapon = thingData;
            }
            catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
        }
    }

    private static void GetHumanInventory(Pawn pawn, HumanFile humanFile)
    {
        if (pawn.inventory.innerContainer.Count() != 0)
        {
            List<ItemComponent> toGet = new List<ItemComponent>();

            foreach (Thing thing in pawn.inventory.innerContainer)
            {
                try
                {
                    ItemFile thingData = ItemScriber.ItemToString(thing, thing.stackCount);
                    ItemComponent component = new ItemComponent();
                    component.Thing = thingData;

                    toGet.Add(component);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }

            humanFile.Items = toGet.ToArray();
        }
    }

    private static void GetHumanTransform(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            humanFile.Transform.Position = new int[] 
            { 
                pawn.Position.x,
                pawn.Position.y, 
                pawn.Position.z 
            };

            humanFile.Transform.Rotation = pawn.Rotation.AsInt;
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    //Setters

    private static PawnKindDef SetHumanKind(HumanFile humanFile)
    {
        try { return DefDatabase<PawnKindDef>.AllDefs.First(fetch => fetch.defName == humanFile.KindDef); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        return null;
    }

    private static Faction SetHumanFaction(HumanFile humanFile)
    {
        try { return Find.FactionManager.AllFactions.First(fetch => fetch.def.defName == humanFile.FactionDef); }
        catch { return Faction.OfPlayer; }
    }

    private static Pawn SetHuman(PawnKindDef kind, Faction faction, HumanFile humanFile)
    {
        try { return PawnGenerator.GeneratePawn(kind, faction); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        return null;
    }

    private static void SetHumanBioDetails(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            pawn.Name = new NameSingle(humanFile.Name);
            pawn.ageTracker.AgeBiologicalTicks = long.Parse(humanFile.BiologicalAge);
            pawn.ageTracker.AgeChronologicalTicks = long.Parse(humanFile.ChronologicalAge);

            Enum.TryParse(humanFile.Gender, true, out Gender humanGender);
            pawn.gender = humanGender;

            pawn.story.hairDef = DefDatabase<HairDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.HairDefName);
            pawn.story.headType = DefDatabase<HeadTypeDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.HeadTypeDefName);
            pawn.style.beardDef = DefDatabase<BeardDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.BeardDefName);
            pawn.story.bodyType = DefDatabase<BodyTypeDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.BodyTypeDefName);
            pawn.style.FaceTattoo = DefDatabase<TattooDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.FaceTattooDefName);
            pawn.style.BodyTattoo = DefDatabase<TattooDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.BodyTattooDefName);

            string hairColor = humanFile.HairColor.Replace("RGBA(", "").Replace(")", "");
            string[] isolatedHair = hairColor.Split(',');
            float r = float.Parse(isolatedHair[0]);
            float g = float.Parse(isolatedHair[1]);
            float b = float.Parse(isolatedHair[2]);
            float a = float.Parse(isolatedHair[3]);
            pawn.story.HairColor = new UnityEngine.Color(r, g, b, a);

            string skinColor = humanFile.SkinColor.Replace("RGBA(", "").Replace(")", "");
            string[] isolatedSkin = skinColor.Split(',');
            r = float.Parse(isolatedSkin[0]);
            g = float.Parse(isolatedSkin[1]);
            b = float.Parse(isolatedSkin[2]);
            a = float.Parse(isolatedSkin[3]);
            pawn.story.SkinColorBase = new UnityEngine.Color(r, g, b, a);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetHumanHediffs(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            pawn.health.RemoveAllHediffs();
            pawn.health.Reset();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        if (humanFile.Hediffs.Length > 0)
        {
            for (int i = 0; i < humanFile.Hediffs.Length; i++)
            {
                try
                {
                    HediffComponent component = humanFile.Hediffs[i];
                    HediffDef hediffDef = DefDatabase<HediffDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.DefName);
                    BodyPartRecord bodyPart = pawn.RaceProps.body.AllParts.FirstOrDefault(x => x.def.defName == component.PartDefName && 
                        x.Label == component.PartLabel);

                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
                    hediff.Severity = component.Severity;

                    if (component.IsPermanent)
                    {
                        HediffComp_GetsPermanent hediffComp = hediff.TryGetComp<HediffComp_GetsPermanent>();
                        hediffComp.IsPermanent = true;
                    }

                    pawn.health.AddHediff(hediff, bodyPart);
                    if (component.Immunity != -1f)
                    {
                        pawn.health.immunity.TryAddImmunityRecord(hediffDef, hediffDef);
                        ImmunityRecord immunityRecord = pawn.health.immunity.GetImmunityRecord(hediffDef);
                        immunityRecord.immunity = component.Immunity;
                    }

                    if (component.TendDuration != -1)
                    {
                        HediffComp_TendDuration comp = hediff.TryGetComp<HediffComp_TendDuration>();
                        comp.tendQuality = component.TendQuality;
                        comp.tendTicksLeft = component.TendDuration;
                    }
                    
                    if (component.TotalTendQuality != -1f) 
                    {
                        HediffComp_TendDuration comp = hediff.TryGetComp<HediffComp_TendDuration>();
                        Type type = comp.GetType();
                        FieldInfo fieldInfo = type.GetField("totalTendQuality", BindingFlags.NonPublic | BindingFlags.Instance);
                        fieldInfo.SetValue(comp, component.TotalTendQuality);
                    }
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetHumanChildState(Pawn pawn, HumanFile humanFile)
    {
        try { pawn.ageTracker.growthPoints = humanFile.GrowthPoints; }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetHumanXenotype(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            if (humanFile.Xenotype.DefName != "null")
            {
                pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.Xenotype.DefName));
            }

            if (humanFile.Xenotype.CustomXenotypeName != "null")
            {
                pawn.genes.xenotypeName = humanFile.Xenotype.CustomXenotypeName;
            }
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetHumanXenogenes(Pawn pawn, HumanFile humanFile)
    {
        try { pawn.genes.Xenogenes.Clear(); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        if (humanFile.Xenogenes.Length > 0)
        {
            for (int i = 0; i < humanFile.Xenogenes.Length; i++)
            {
                try
                {
                    XenogeneComponent component = humanFile.Xenogenes[i];
                    GeneDef def = DefDatabase<GeneDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.DefName);
                    if (def != null) pawn.genes.AddGene(def, true);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetHumanEndogenes(Pawn pawn, HumanFile humanFile)
    {
        try { pawn.genes.Endogenes.Clear(); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        if (humanFile.Endogenes.Length > 0)
        {
            for (int i = 0; i < humanFile.Endogenes.Length; i++)
            {
                try
                {
                    EndogeneComponent component = humanFile.Endogenes[i];
                    GeneDef def = DefDatabase<GeneDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.DefName);
                    if (def != null) pawn.genes.AddGene(def, true);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetHumanFavoriteColor(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            float r;
            float g;
            float b;
            float a;

            string favoriteColor = humanFile.FavoriteColor.Replace("RGBA(", "").Replace(")", "");
            string[] isolatedFavoriteColor = favoriteColor.Split(',');
            r = float.Parse(isolatedFavoriteColor[0]);
            g = float.Parse(isolatedFavoriteColor[1]);
            b = float.Parse(isolatedFavoriteColor[2]);
            a = float.Parse(isolatedFavoriteColor[3]);
            pawn.story.favoriteColor = new UnityEngine.Color(r, g, b, a);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetHumanStory(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            if (humanFile.Stories.ChildhoodStoryDefName != "null")
            {
                pawn.story.Childhood = DefDatabase<BackstoryDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.Stories.ChildhoodStoryDefName);
            }

            if (humanFile.Stories.AdulthoodStoryDefName != "null")
            {
                pawn.story.Adulthood = DefDatabase<BackstoryDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == humanFile.Stories.AdulthoodStoryDefName);
            }
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }

    private static void SetHumanSkills(Pawn pawn, HumanFile humanFile)
    {
        if (humanFile.Skills.Length > 0)
        {
            for (int i = 0; i < humanFile.Skills.Length; i++)
            {
                try
                {
                    SkillComponent component = humanFile.Skills[i];
                    pawn.skills.skills[i].levelInt = component.Level;

                    Enum.TryParse(component.Passion, true, out Passion passion);
                    pawn.skills.skills[i].passion = passion;
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetHumanTraits(Pawn pawn, HumanFile humanFile)
    {
        try { pawn.story.traits.allTraits.Clear(); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        if (humanFile.Traits.Length > 0)
        {
            for (int i = 0; i < humanFile.Traits.Length; i++)
            {
                try
                {
                    TraitComponent component = humanFile.Traits[i];
                    TraitDef traitDef = DefDatabase<TraitDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == component.DefName);
                    Trait trait = new Trait(traitDef, component.Degree);
                    pawn.story.traits.GainTrait(trait);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetHumanApparel(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            pawn.apparel.DestroyAll();
            pawn.apparel.DropAllOrMoveAllToInventory();
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        if (humanFile.Apparel.Length > 0)
        {
            for (int i = 0; i < humanFile.Apparel.Length; i++)
            {
                try
                {
                    ApparelComponent component = humanFile.Apparel[i];
                    Apparel apparel = (Apparel)ItemScriber.StringToItem(component.EquippedApparel);
                    if (component.WornByCorpse) apparel.WornByCorpse.MustBeTrue();
                    else apparel.WornByCorpse.MustBeFalse();

                    pawn.apparel.Wear(apparel);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetHumanEquipment(Pawn pawn, HumanFile humanFile)
    {
        try { pawn.equipment.DestroyAllEquipment(); }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }

        if (humanFile.Weapon != null)
        {
            try
            {
                ThingWithComps thing = (ThingWithComps)ItemScriber.StringToItem(humanFile.Weapon);
                pawn.equipment.AddEquipment(thing);
            }
            catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
        }
    }

    private static void SetHumanInventory(Pawn pawn, HumanFile humanFile)
    {
        if (humanFile.Items.Length > 0)
        {
            for (int i = 0; i < humanFile.Items.Length; i++)
            {
                try
                {
                    ItemComponent component = humanFile.Items[i];
                    Thing thing = ItemScriber.StringToItem(component.Thing);
                    pawn.inventory.TryAddAndUnforbid(thing);
                }
                catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
            }
        }
    }

    private static void SetHumanTransform(Pawn pawn, HumanFile humanFile)
    {
        try
        {
            pawn.Position = new IntVec3(humanFile.Transform.Position[0], humanFile.Transform.Position[1], humanFile.Transform.Position[2]);
            pawn.Rotation = new Rot4(humanFile.Transform.Rotation);
        }
        catch (Exception e) { Logger.Warning(e.ToString(), Logger.LogImportance.Verbose); }
    }
}