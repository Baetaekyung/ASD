using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ColorPalletteDB")]
public class ColorPalletteDB : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, Color> colorPallet = new();

    public Color GetColor(string colorName)
        => colorPallet.TryGetValue(colorName, out var color) 
            ? color : Color.white;
}
