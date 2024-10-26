using RimWorld;
using UnityEngine;
using Verse;

public class MessageWindow : Window
{
    public override Vector2 InitialSize => new Vector2(500f, 150f);

    private readonly string title = "MESSAGE";

    private readonly string description = "";

    private readonly float buttonX = 150f;

    private readonly float buttonY = 38f;

    public MessageWindow(string description)
    {
        this.description = description;

        forcePause = true;
        closeOnCancel = true;
        closeOnAccept = false;
        absorbInputAroundWindow = true;
        soundAppear = SoundDefOf.CommsWindow_Open;
    }

    public override void DoWindowContents(Rect rect)
    {
        float centeredX = rect.width / 2;
        float horizontalLineDif = Text.CalcSize(description).y + StandardMargin / 2;
        float windowDescriptionDif = Text.CalcSize(description).y + StandardMargin;

        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(centeredX - Text.CalcSize(title).x / 2, rect.y, Text.CalcSize(title).x, Text.CalcSize(title).y), title);

        Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

        Text.Font = GameFont.Small;
        Widgets.Label(new Rect(centeredX - Text.CalcSize(description).x / 2, windowDescriptionDif, Text.CalcSize(description).x, Text.CalcSize(description).y), description);

        if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "OK"))
        {
            Close();
        }
    }
}