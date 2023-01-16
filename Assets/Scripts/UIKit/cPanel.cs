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

        Outline outline = mGameObject.AddComponent<Outline>();
        outline.effectDistance = new Vector2( 2, 2 );
        outline.effectColor = new Color( 0.6f, 0.8f, 0.8f, 1f );

        SetColor( new Color( 0.13f, 0.2f, 0.2f, 1f ) );
    }
}