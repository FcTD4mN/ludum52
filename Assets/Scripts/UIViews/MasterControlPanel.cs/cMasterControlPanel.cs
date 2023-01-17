using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

class cMasterControlPanel :
    cPanel
{
    private cButton mCloseButton;
    private cLabel mTitle;

    private cScrollView mScrollArea;
    private cMCPContentPanel mContent;

    private int mPadding = 24;
    private int mTitleSize = 24;


    public cMasterControlPanel(GameObject parentView, string name) : base(parentView, name)
    {
        mTitle = new cLabel(mGameObject, "title");
        mTitle.mText.text = "MasterControlPanel";
        mTitle.mText.fontStyle = TMPro.FontStyles.Bold;
        mTitle.mText.fontSize = mTitleSize;
        mTitle.mText.alignment = TMPro.TextAlignmentOptions.Center;

        mCloseButton = new cButton(mGameObject, "Close");
        mCloseButton.SetColor(Color.black);
        mCloseButton.SetText("X");
        mCloseButton.AddOnClickAction(() =>
        {
            GameObject.Destroy( mGameObject );
        });

        mScrollArea = new cScrollView(mGameObject, "ScrollArea");

        mContent = new cMCPContentPanel( mGameObject, "Content" );
        mScrollArea.AddViewToContent( mContent );
    }


    public void Update()
    {
        if( mGameObject == null ) return;

        mContent.Update();
    }


    override public void LayoutSubviews()
    {
        Rect frame = GetFrame();

        mCloseButton.SetFrame(new Rect(0,
                                         0,
                                         40,
                                         40));
        mCloseButton.SetCenter(new Vector2(frame.xMax, frame.yMin));


        int titleHeight = mTitleSize;
        Rect titleFrame = new Rect(mPadding,
                                    mPadding,
                                    frame.width - mPadding * 2,
                                    titleHeight);
        mTitle.SetFrame(titleFrame);

        float scrollHeight = frame.height - titleFrame.height - mPadding * 2;
        mScrollArea.SetFrame(new Rect(0, titleFrame.yMax + mPadding, frame.width, scrollHeight));

        float contentHeight = mContent.RequiredHeightForWidth(frame.width);
        mContent.SetFrame(new Rect(0, 0, frame.width, contentHeight));


        mScrollArea.SetContentSize(new Vector2(0, mContent.GetFrame().height));
    }
}