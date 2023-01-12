using UnityEngine;
using UnityEngine.UI;
using System;

class cView
{
    public GameObject mGameObject;

    public cView( GameObject parentView, string name ): this( name )
    {
        mGameObject.transform.SetParent(parentView.transform);
    }

    public cView(string name)
    {
        mGameObject = new GameObject(name);

        RectTransform rect = mGameObject.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;

        mGameObject.AddComponent<CanvasRenderer>();
    }



    // ===================================
    // Geometry
    // ===================================
    public void SetFrame(Rect frame)
    {
        RectTransform rect = mGameObject.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(frame.xMin, frame.yMin);
        rect.sizeDelta = new Vector2(frame.width, frame.height);

        LayoutSubviews();
    }


    public void SetCenter(Vector2 center)
    {
        RectTransform rect = mGameObject.GetComponent<RectTransform>();

        rect.position = new Vector2( center.x - rect.sizeDelta.x / 2f, center.y - rect.sizeDelta.y / 2f);
    }


    public Rect GetFrame()
    {
        RectTransform rect = mGameObject.GetComponent<RectTransform>();
        return  new Rect( rect.anchoredPosition.x, rect.anchoredPosition.y, rect.rect.width, rect.rect.height );
    }


    public virtual void LayoutSubviews()
    {
    }


    // ===================================
    // Color
    // ===================================
    public void SetColor( Color color )
    {
        Image image = mGameObject.GetComponent<Image>();
        if( image == null ) return;

        image.color = color;
    }

}