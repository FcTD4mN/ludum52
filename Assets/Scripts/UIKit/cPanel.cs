using UnityEngine;
using UnityEngine.UI;
using System;

class cPanel :
    cView
{
    public cPanel(GameObject parentView, string name) : base(parentView, name)
    {
        Image image = mGameObject.AddComponent<Image>();
        image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        image.type = Image.Type.Sliced;
        image.fillCenter = true;

        SetColor( new Color( 0.2f, 0.2f, 0.2f, 0.8f ) );
    }
}