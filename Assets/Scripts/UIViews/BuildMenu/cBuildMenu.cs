using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

class cBuildMenu :
    cPanel
{
    public Action mOnClose;
    public Action<RTSManager.eBuildingList> mOnBuildingClicked;
    public Action<RTSManager.eBuildingList?> mOnHover;

    private cButton mCloseButton;
    private cLabel mTitle;

    private cScrollView mScrollArea;
    private cProductionBuildingPanel mProdBuildings;
    private cBuffBuildingPanel mBuffBuildings;


    private int mPadding = 24;
    private int mTitleSize = 24;

    public cBuildMenu(GameObject parentView, string name) : base(parentView, name)
    {
        mTitle = new cLabel(mGameObject, "Title");
        mTitle.mText.text = "Buildings: ";
        mTitle.mText.fontStyle = TMPro.FontStyles.Bold;
        mTitle.mText.fontSize = mTitleSize;
        mTitle.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mCloseButton = new cButton( mGameObject, "Close" );
        mCloseButton.SetColor( Color.black );
        mCloseButton.SetText( "X" );
        mCloseButton.AddOnClickAction( () => {
            mOnClose?.Invoke();
        });

        mScrollArea = new cScrollView( mGameObject, "ScrollArea" );

        mProdBuildings = new cProductionBuildingPanel( mScrollArea.mGameObject, "ProdPanel" );
        mBuffBuildings = new cBuffBuildingPanel( mScrollArea.mGameObject, "BuffPanel" );

        mProdBuildings.mOnBuildingClicked = ( building ) => {
            mOnBuildingClicked( building );
        };
        mBuffBuildings.mOnBuildingClicked = (building) =>
        {
            mOnBuildingClicked(building);
        };

        mScrollArea.AddViewToContent( mProdBuildings );
        mScrollArea.AddViewToContent( mBuffBuildings );

        mProdBuildings.mGameObject.SetActive( false );
        mBuffBuildings.mGameObject.SetActive( false );
    }


    override public void LayoutSubviews()
    {
        Rect frame = GetFrame();

        mCloseButton.SetFrame ( new Rect(0,
                                         0,
                                         40,
                                         40));
        mCloseButton.SetCenter( new Vector2( frame.xMax, frame.yMin ) );


        int titleHeight = mTitleSize;
        Rect titleFrame = new Rect( mPadding,
                                    mPadding,
                                    frame.width - mPadding*2,
                                    titleHeight );
        mTitle.SetFrame( titleFrame );

        float scrollHeight = frame.height - titleFrame.height - mPadding*2;
        mScrollArea.SetFrame(new Rect(0, titleFrame.yMax + mPadding, frame.width, scrollHeight ));

        float prodPanelHeight = mProdBuildings.RequiredHeightForWidth( frame.width );
        mProdBuildings.SetFrame( new Rect(0, 0, frame.width, prodPanelHeight) );

        float buffPanelHeight = mBuffBuildings.RequiredHeightForWidth( frame.width );
        mBuffBuildings.SetFrame( new Rect(0, 0, frame.width, buffPanelHeight) );
    }


    public void ShowProdBuildingPanel()
    {
        mProdBuildings.mGameObject.SetActive( true );
        mBuffBuildings.mGameObject.SetActive( false );

        // If content doesn't care about horizontal scroll, set width to 0
        // If you try to put the logical width, then if unity adds a vScroll, it'll reduce viewport, meaning content is too large
        // And then
        // Hello hScroll, what are you doing here ? Even if you set horizontal to false, it doesn't actually do anything...
        mScrollArea.SetContentSize(new Vector2(0, mProdBuildings.GetFrame().height));

    }
    public void ShowBuffBuildingPanel()
    {
        mProdBuildings.mGameObject.SetActive( false );
        mBuffBuildings.mGameObject.SetActive( true );

        // If content doesn't care about horizontal scroll, set width to 0
        // If you try to put the logical width, then if unity adds a vScroll, it'll reduce viewport, meaning content is too large
        // And then
        // Hello hScroll, what are you doing here ? Even if you set horizontal to false, it doesn't actually do anything...
        mScrollArea.SetContentSize(new Vector2(0, mBuffBuildings.GetFrame().height));
    }




    public void UpdateBuildMenu()
    {
        bool isLocationOnTower = mProdBuildings.mGameObject.activeSelf;
        bool isLocationOnBuffTower = mBuffBuildings.mGameObject.activeSelf;

        mProdBuildings.UpdateButtons();
        mBuffBuildings.UpdateButtons();
    }


    public void UpdateMouse()
    {
        if( !mGameObject.activeSelf ) return;
        if ( !EventSystem.current.IsPointerOverGameObject() ) return;

        Rect screenRect = Camera.main.pixelRect;
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePosRegular = new Vector3( mousePosScreen.x, screenRect.height - mousePosScreen.y, mousePosScreen.z );

        List<(RTSManager.eBuildingList, cButton)> allButtons = new List<(RTSManager.eBuildingList, cButton)>();
        if (mProdBuildings.mGameObject.activeSelf)
        {
            allButtons = mProdBuildings.mButtons;
        }
        else if (mBuffBuildings.mGameObject.activeSelf)
        {
            allButtons = mBuffBuildings.mButtons;
        }

        foreach( (RTSManager.eBuildingList, cButton) pair in allButtons )
        {
            Rect buttonFrameScreen = pair.Item2.GetFrameRelativeTo( mGameObject );
            if( buttonFrameScreen.Contains( mousePosRegular ) )
            {
                mOnHover?.Invoke( pair.Item1 );
                return;
            }
        }

        mOnHover?.Invoke(null);
    }


}

