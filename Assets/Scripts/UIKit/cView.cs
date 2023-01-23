using UnityEngine;
using UnityEngine.UI;
using System;

class cView
{
    public GameObject mGameObject;

    // ===================================
    // Constructors
    // ===================================
    public cView( GameObject parentView, string name ): this( name )
    {
        mGameObject.transform.SetParent(parentView.transform);
    }

    public cView(string name)
    {
        mGameObject = new GameObject(name);

        RectTransform rect = mGameObject.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = new Vector2( 0, 1 );
        rect.anchorMax = new Vector2( 0, 1 );
        rect.pivot = new Vector2( 0, 1 );

        mGameObject.AddComponent<CanvasRenderer>();
    }


    // ===================================
    // Geometry
    // ===================================
    public virtual void LayoutSubviews()
    {
    }


    // With anchors and pivot point set to topLeft, we need to negate y in order to pass in regular topleft origin based rect and geometry and have it work
    public void SetFrame(Rect frame)
    {
        RectTransform rect = mGameObject.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(frame.xMin, -frame.yMin);
        rect.sizeDelta = new Vector2(frame.width, frame.height);

        LayoutSubviews();
    }


    public void SetCenter(Vector2 center)
    {
        RectTransform rect = mGameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2( center.x - rect.sizeDelta.x / 2f, -(center.y - rect.sizeDelta.y / 2f));
    }


    public Rect GetFrame()
    {
        return  GetFrame( mGameObject );
    }


    // This gives frame as if other was its direct parent, with 0,0 being other's top left
    public Rect GetFrameRelativeTo( GameObject other )
    {
        Rect frame = GetFrame();

        Transform parent = mGameObject.transform.parent;
        while( parent != null && parent != other.transform )
        {
            Rect parentFrame = GetFrame( parent.gameObject );
            frame = Utilities.OffsetRectBy( frame, new Vector2( parentFrame.xMin, parentFrame.yMin ) );

            parent = parent.parent;
        }

        return  frame;
    }


    private Rect GetFrame( GameObject ofObject )
    {
        RectTransform rect = ofObject.GetComponent<RectTransform>();
        return new Rect(rect.anchoredPosition.x, -rect.anchoredPosition.y, rect.rect.width, rect.rect.height);
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




class Hoverable : MonoBehaviour
{
    public Action mOnHoverAction;
    public Action mOnHoverEndedAction;
}