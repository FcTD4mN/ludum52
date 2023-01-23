using UnityEngine;
using System.Collections.Generic;
using System;

class cUIUtilities
{
    static string GetColoredString( string text, Color color )
    {
        var colorHex = ColorUtility.ToHtmlStringRGB(color);
        return  "<color=#" + colorHex + ">" + text + "</color>";
    }
    static string GetColoredString( string text, string hexColor )
    {
        return "<color=#" + hexColor + ">" + text + "</color>";
    }
}