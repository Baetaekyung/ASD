using UnityEngine;

public static class RichText
{
    public static string ColorToHTML(Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }

    public static string GetColorText(string text, Color color)
    {
        string colorText = $"<color=#{ColorToHTML(color)}>{text}</color>";

        return colorText;
    }
}
