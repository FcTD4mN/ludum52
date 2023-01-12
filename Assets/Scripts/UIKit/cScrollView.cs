using UnityEngine;
using UnityEngine.UI;
using System;

class cScrollView :
    cView
{
    internal ScrollRect mScrollRect;
    internal GameObject mViewport;
    internal GameObject mContent;


    public cScrollView( GameObject parentView, string name ) : base(parentView, name)
    {
        GameObject.Destroy( mGameObject );
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/ScrollView");
        mGameObject = GameObject.Instantiate( prefab, Vector3.zero, Quaternion.Euler(0, 0, 0), parentView.transform );

        mViewport = mGameObject.transform.Find( "Viewport" ).gameObject;
        mContent = mViewport.transform.Find( "Content" ).gameObject;

        RectTransform rect = mGameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;

        mScrollRect = mGameObject.GetComponent<ScrollRect>();
    }

    public void AddViewToContent( cView view )
    {
        view.mGameObject.transform.SetParent( mContent.transform );
    }
}



class cBuildMenu :
    cScrollView
{
        public Action mOnClose;


        private cProductionBuildingPanel mPanel;

        public cBuildMenu( GameObject parentView, string name) : base(parentView, name)
        {
            mPanel = new cProductionBuildingPanel( mGameObject, "Panel" );
            mScrollRect.horizontal = false;

            AddViewToContent( mPanel );
        }


        override public void LayoutSubviews()
        {
            Rect frame = GetFrame();

            int panelRequiredHeight = mPanel.RequiredHeightForWidth( (int)frame.width );
            int panelHeight = (int)Math.Max( panelRequiredHeight, frame.height );

            mPanel.SetFrame(new Rect(0, 0, frame.width, panelHeight ));

            RectTransform contentRect = mContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, panelHeight);
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