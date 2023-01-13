using UnityEngine;
using UnityEngine.UI;
using System;

class cScrollView :
    cView
{
    public ScrollRect mScrollRect;
    public GameObject mViewport;
    public GameObject mContent;

    public GameObject mHScrollBar;
    public GameObject mVScrollBar;


    public cScrollView( GameObject parentView, string name ) : base(parentView, name)
    {
        GameObject.Destroy( mGameObject );
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/ScrollView");
        mGameObject = GameObject.Instantiate( prefab, Vector3.zero, Quaternion.Euler(0, 0, 0), parentView.transform );

        RectTransform rect = mGameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = new Vector2( 0, 1 );
        rect.anchorMax = new Vector2( 0, 1 );
        rect.pivot = new Vector2( 0, 1 );

        mScrollRect = mGameObject.GetComponent<ScrollRect>();

        mViewport = mGameObject.transform.Find("Viewport").gameObject;

        mContent = mViewport.transform.Find("Content").gameObject;
        RectTransform contentRect = mContent.GetComponent<RectTransform>();
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.anchorMax = new Vector2( 0, 1 );
        contentRect.anchorMin = new Vector2( 0, 1 );
        contentRect.pivot = new Vector2(0, 1);

        mHScrollBar = mGameObject.transform.Find("Scrollbar Horizontal").gameObject;
        mVScrollBar = mGameObject.transform.Find("Scrollbar Vertical").gameObject;


        SetColor(new Color(0.2f, 0.2f, 0.2f, 0.8f));
    }


    public void AddViewToContent( cView view )
    {
        view.mGameObject.transform.SetParent( mContent.transform );
    }

    public void SetContentSize( Vector2 size )
    {
        RectTransform contentRect = mContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = size;
    }
}





// This is the beginning of making scroll views all from code
// Need to add scrollView component + scrollBars, but is this usefull compared to baseprefab?

// // VIEWPORT
// cView viewport = new cView(mGameObject, "Viewport");
// RectTransform viewportRect = viewport.mGameObject.GetComponent<RectTransform>();
// viewportRect.anchoredPosition = Vector2.zero;
// viewportRect.anchorMin = new Vector2(0, 1);
// viewportRect.anchorMax = new Vector2(0, 1);
// viewportRect.pivot = Vector2.zero;
// viewportRect.position = new Vector2(0, 0);


// Image viewportImage = viewport.mGameObject.AddComponent<Image>();
// viewportImage.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
// viewportImage.type = Image.Type.Sliced;
// viewportImage.fillCenter = true;

// viewport.mGameObject.AddComponent<Mask>();

// // CONTENT
// GameObject content = new GameObject("Content");
// content.transform.SetParent( viewport.mGameObject.transform );
// RectTransform contentRect = content.AddComponent<RectTransform>();
// contentRect.anchoredPosition = Vector2.zero;
// contentRect.anchorMin = new Vector2( 0, 1 );
// contentRect.anchorMax = new Vector2(0, 1);
// contentRect.pivot = Vector2.zero;
// contentRect.position = new Vector2(0, 0);

// Image image = mGameObject.AddComponent<Image>();
// image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
// image.type = Image.Type.Sliced;
// image.fillCenter = true;

// mGameObject.AddComponent<Button>();