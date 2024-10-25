using System;
using RimWorld;
using UnityEngine;
using Verse;

public class PromptWindow : Window
{
    public override Vector2 InitialSize => new Vector2(400f, 200f);

    private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

    private readonly int startAcceptingInputAtFrame;

    private readonly string title;

    private readonly Vector2 buttonSize = new Vector2(150f, 38f);

    private readonly Action actionYes;

    private readonly Action actionNo;

    private readonly string inputOneLabel;

    public static string windowAnswer;

    public PromptWindow(string title, string inputOneLabel, Action actionYes = null, Action actionNo = null)
    {
        this.title = title;
        this.actionYes = actionYes;
        this.actionNo = actionNo;
        this.inputOneLabel = inputOneLabel;

        windowAnswer = string.Empty;

        forcePause = true;
        closeOnCancel = true;
        closeOnAccept = false;
        absorbInputAroundWindow = true;
        soundAppear = SoundDefOf.CommsWindow_Open;
    }

    public override void DoWindowContents(Rect rect)
    {
        float centeredX = rect.width / 2;
        float horizontalLineDif = Text.CalcSize(title).y + StandardMargin / 2;

        float inputOneLabelDif = Text.CalcSize(inputOneLabel).y + StandardMargin;
        float inputOneDif = inputOneLabelDif + 28f;

        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(centeredX - Text.CalcSize(title).x / 2, rect.y, Text.CalcSize(title).x, Text.CalcSize(title).y), title);
        Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

        DrawInputOne(centeredX, inputOneLabelDif, inputOneDif);
        
        if (Widgets.ButtonText(new Rect(new Vector2(rect.xMin, rect.yMax - buttonSize.y), buttonSize), "Confirm"))
        {
            actionYes?.Invoke();
            Close();
        }

        if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonSize.x, rect.yMax - buttonSize.y), buttonSize), "Cancel"))
        {  
            actionNo?.Invoke();
            Close();
        }
    }

    private void DrawInputOne(float centeredX, float labelDif, float normalDif)
    {
        Text.Font = GameFont.Small;
        Widgets.Label(new Rect(centeredX - Text.CalcSize(inputOneLabel).x / 2, labelDif, Text.CalcSize(inputOneLabel).x, Text.CalcSize(inputOneLabel).y), inputOneLabel);

        string inputOne = Widgets.TextField(new Rect(centeredX - (200f / 2), normalDif + 10f, 200f, 30f), windowAnswer);
        if (AcceptsInput && inputOne.Length <= 32) windowAnswer = inputOne;
    }
}