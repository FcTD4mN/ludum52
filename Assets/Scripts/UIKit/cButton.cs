using UnityEngine;
using UnityEngine.UI;
using System;

class cButton:
    cView
{
    public cLabel mLabel;


    // ===================================
    // Constructors
    // ===================================
    public cButton( GameObject parentView, string name ): this( name )
    {
        mGameObject.transform.SetParent(parentView.transform);
    }
    public cButton(string name) : base(name)
    {
        Image image = mGameObject.AddComponent<Image>();
        image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.fillCenter = true;

        mGameObject.AddComponent<Button>();
        SetColor( Color.gray );
    }


    // ===================================
    // UI
    // ===================================
    public new void SetColor( Color color )
    {
        Button button = mGameObject.GetComponent<Button>();

        float h,s,v;
        Color.RGBToHSV( color, out h, out s, out v );

        ColorBlock colors = new ColorBlock();
        colors.normalColor = color;
        colors.highlightedColor = Color.HSVToRGB( h, s * 0.5f, (v + 1)* 0.5f );
        colors.pressedColor = Color.HSVToRGB(h, s * 0.5f, (v + 1) * 0.5f);
        colors.disabledColor = Color.HSVToRGB(h, s * 0.5f, v * 0.5f);
        colors.selectedColor = colors.normalColor;
        colors.colorMultiplier = 1;
        colors.fadeDuration = 0.1f;

        button.colors = colors;
    }


    public void AddOnClickAction( Action action )
    {
        Button button = mGameObject.GetComponent<Button>();

        button.onClick.AddListener( () => {
            action();
        });
    }


    public override void LayoutSubviews()
    {
        if( mLabel != null )
        {
            Rect myFrame = GetFrame();
            mLabel.SetFrame( new Rect( 0, 0, myFrame.width, myFrame.height ));
        }
    }


    // ===================================
    // Label
    // ===================================
    public void AddText( string text )
    {
        if( mLabel == null )
        {
            mLabel = new cLabel( mGameObject, "label" );
        }

        mLabel.mText.text = text;
        LayoutSubviews();
    }
}