using System.IO;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

public class MapListingWindow : Window
{
    public override Vector2 InitialSize => new Vector2(500f, 400f);

    public readonly string title;

    public readonly string description;

    public readonly string[] elements;

    private Vector2 scrollPosition = Vector2.zero;

    private readonly Vector2 normalButton = new Vector2(100f, 37f);

    private readonly Vector2 selectButton = new Vector2(25f, 25f);

    public MapListingWindow(string title, string description, string[] elements)
    {
        this.title = title;
        this.description = description;
        this.elements = elements;

        forcePause = true;
        closeOnCancel = true;
        closeOnAccept = false;
        absorbInputAroundWindow = true;
        soundAppear = SoundDefOf.CommsWindow_Open;
    }

    public override void DoWindowContents(Rect rect)
    {
        float centeredX = rect.width / 2;

        float windowDescriptionDif = Text.CalcSize(description).y + StandardMargin;
        float descriptionLineDif1 = windowDescriptionDif - Text.CalcSize(description).y * 0.25f;
        float descriptionLineDif2 = windowDescriptionDif + Text.CalcSize(description).y * 1.1f;

        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(centeredX - Text.CalcSize(title).x / 2, rect.y, Text.CalcSize(title).x, Text.CalcSize(title).y), title);

        Widgets.DrawLineHorizontal(rect.x, descriptionLineDif1, rect.width);

        Text.Font = GameFont.Small;
        Widgets.Label(new Rect(centeredX - Text.CalcSize(description).x / 2, windowDescriptionDif, Text.CalcSize(description).x, Text.CalcSize(description).y), description);
        Text.Font = GameFont.Medium;

        Widgets.DrawLineHorizontal(rect.x, descriptionLineDif2, rect.width);
        
        FillMainRect(new Rect(0f, descriptionLineDif2 + 10f, rect.width, rect.height - normalButton.y - 85f));

        if (Widgets.ButtonText(new Rect(new Vector2(centeredX - normalButton.x / 2, rect.yMax - normalButton.y), normalButton), "Close"))
        {
            Close();
        }
    }

    private void FillMainRect(Rect mainRect)
    {
        float height = 6f + elements.Count() * 30f;
        Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
        Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);
        float num = 0;
        float num2 = scrollPosition.y - 30f;
        float num3 = scrollPosition.y + mainRect.height;
        int num4 = 0;

        for (int i = 0; i < elements.Count(); i++)
        {
            if (num > num2 && num < num3)
            {
                Rect rect = new Rect(0f, num, viewRect.width, 30f);
                DrawCustomRow(rect, elements[i], num4);
            }

            num += 30f;
            num4++;
        }

        Widgets.EndScrollView();
    }

    private void DrawCustomRow(Rect rect, string element, int index)
    {
        Text.Font = GameFont.Small;
        Rect fixedRect = new Rect(new Vector2(rect.x, rect.y + 5f), new Vector2(rect.width - 16f, rect.height - 5f));
        if (index % 2 == 0) Widgets.DrawHighlight(fixedRect);

        Widgets.Label(fixedRect, $"{element}");

        if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - selectButton.x, rect.yMax - selectButton.y), selectButton), "X"))
        {
            MapManager.OpenMapDeleter(index);
            Close();
        }

        if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - (selectButton.x * 2), rect.yMax - selectButton.y), selectButton), "R"))
        {
            MapManager.OpenMapRenamer(index);
            Close();
        }

        if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - (selectButton.x * 3), rect.yMax - selectButton.y), selectButton), "L"))
        {
            MapManager.LoadMap(Directory.GetFiles(Master.modFolderPath)[index]);
            Close();
        }
    }
}