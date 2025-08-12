using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

public static class DebugEx
{
    [Conditional("DbugEx")]
    public static void CLog(string text)
    {
        Debug.Log(RichText.GetColorText(text, Color.white));
    }

    [Conditional("DbugEx")]
    public static void CLog(string text, Color color)
    {
        Debug.Log(RichText.GetColorText(text, color));
    }

    [Conditional("DbugEx")]
    public static void CLog(string text, string colorText)
    {
        var pallette = Resources.Load<ColorPalletteDB>("Color/ColorPallette");
        Debug.Log(RichText.GetColorText(text, pallette.GetColor(colorText)));
    }
}
