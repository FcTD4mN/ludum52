using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

class cBuildMenu :
    cPanel
{
    public Action mOnClose;
    public Action<RTSManager.eBuildingList> mOnBuildingClicked;

    private cButton mCloseButton;
    private cLabel mTitle;

    private cScrollView mScrollArea;
    private cProductionBuildingPanel mProdBuildings;
    private cBuffBuildingPanel mBuffBuildings;
    private cWeaponBuildingPanel mWeaponBuildings;


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
        mWeaponBuildings = new cWeaponBuildingPanel( mScrollArea.mGameObject, "WeaponPanel" );

        mProdBuildings.mOnBuildingClicked = ( building ) => {
            mOnBuildingClicked?.Invoke( building );
        };

        mBuffBuildings.mOnBuildingClicked = (building) =>
        {
            mOnBuildingClicked?.Invoke(building);
        };

        mWeaponBuildings.mOnBuildingClicked = (building) =>
        {
            mOnBuildingClicked?.Invoke(building);
        };

        mScrollArea.AddViewToContent( mProdBuildings );
        mScrollArea.AddViewToContent(mBuffBuildings);
        mScrollArea.AddViewToContent(mWeaponBuildings);

        mProdBuildings.mGameObject.SetActive( false );
        mBuffBuildings.mGameObject.SetActive( false );
        mWeaponBuildings.mGameObject.SetActive(false);
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

        float buffPanelHeight = mBuffBuildings.RequiredHeightForWidth(frame.width);
        mBuffBuildings.SetFrame(new Rect(0, 0, frame.width, buffPanelHeight));

        float weaponPanelHeight = mWeaponBuildings.RequiredHeightForWidth(frame.width);
        mWeaponBuildings.SetFrame(new Rect(0, 0, frame.width, weaponPanelHeight));
    }


    public void ShowProdBuildingPanel()
    {
        mProdBuildings.mGameObject.SetActive( true );
        mBuffBuildings.mGameObject.SetActive( false );
        mWeaponBuildings.mGameObject.SetActive(false);

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
        mWeaponBuildings.mGameObject.SetActive(false);

        // If content doesn't care about horizontal scroll, set width to 0
        // If you try to put the logical width, then if unity adds a vScroll, it'll reduce viewport, meaning content is too large
        // And then
        // Hello hScroll, what are you doing here ? Even if you set horizontal to false, it doesn't actually do anything...
        mScrollArea.SetContentSize(new Vector2(0, mBuffBuildings.GetFrame().height));
    }
    public void ShowWeaponBuildingPanel()
    {
        mProdBuildings.mGameObject.SetActive(false);
        mBuffBuildings.mGameObject.SetActive(false);
        mWeaponBuildings.mGameObject.SetActive(true);

        // If content doesn't care about horizontal scroll, set width to 0
        // If you try to put the logical width, then if unity adds a vScroll, it'll reduce viewport, meaning content is too large
        // And then
        // Hello hScroll, what are you doing here ? Even if you set horizontal to false, it doesn't actually do anything...
        mScrollArea.SetContentSize(new Vector2(0, mWeaponBuildings.GetFrame().height));
    }


    public void UpdateBuildMenu()
    {
        if( !mGameObject.activeSelf ) return;

        mProdBuildings.UpdateButtons();
        mBuffBuildings.UpdateButtons();
        mWeaponBuildings.UpdateButtons();
    }
}

