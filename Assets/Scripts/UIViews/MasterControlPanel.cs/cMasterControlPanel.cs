using UnityEngine;
using System;

class cMasterControlPanel :
    cPanel
{
    private cButton mCloseButton;
    private cLabel mTitle;

    private cScrollView mScrollAreaLeft;
    private cMCPResourcesPanel mResourcePanel;

    private cScrollView mScrollArea;
    private cMCPContentPanel mContent;

    private int mPadding = 10;
    private int mTitleSize = 24;

    private int mLeftPanelWidth = 500;


    public Action mCloseAction;


    ~cMasterControlPanel()
    {
        Debug.Log("cMasterControlPanel");
    }
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
            mCloseAction?.Invoke();
        });

        mScrollAreaLeft = new cScrollView(mGameObject, "ScrollAreaLeft");
        mResourcePanel = new cMCPResourcesPanel( mGameObject, "resourcePanel" );
        mScrollAreaLeft.AddViewToContent( mResourcePanel );


        mScrollArea = new cScrollView(mGameObject, "ScrollArea");
        mContent = new cMCPContentPanel( mGameObject, "Content", this );
        mScrollArea.AddViewToContent( mContent );
    }


    public void Update()
    {
        if( mGameObject == null ) return;

        mContent.Update();
        mResourcePanel.Update();
    }


    override public void LayoutSubviews()
    {
        Rect frame = GetFrame();

        float leftPanelWidth = mLeftPanelWidth - mPadding;

        mCloseButton.SetFrame(new Rect(0,
                                         0,
                                         40,
                                         40));
        mCloseButton.SetCenter(new Vector2(frame.width, 0));

        int titleHeight = mTitleSize;
        Rect titleFrame = new Rect( mPadding,
                                    mPadding,
                                    frame.width - mPadding * 2,
                                    titleHeight);
        mTitle.SetFrame(titleFrame);

        float resHeight = mResourcePanel.RequiredHeightForWidth( leftPanelWidth );
        mResourcePanel.SetFrame(new Rect(0, 0, leftPanelWidth, resHeight));

        float scrollHeight = frame.height - titleFrame.height - mPadding * 3;
        mScrollAreaLeft.SetFrame(new Rect(mPadding, titleFrame.yMax + mPadding, leftPanelWidth, scrollHeight));
        mScrollAreaLeft.SetContentSize(new Vector2(0, mResourcePanel.GetFrame().height));


        float smallerPadding = mPadding * 0.2f;
        float rightPartWidth = frame.width - mLeftPanelWidth - mPadding * 1.2f; // small padding between panels
        float contentHeight = mContent.RequiredHeightForWidth(rightPartWidth);
        mContent.SetFrame(new Rect(0, 0, rightPartWidth, contentHeight));

        mScrollArea.SetFrame(new Rect( mLeftPanelWidth + smallerPadding, titleFrame.yMax + mPadding, rightPartWidth, scrollHeight));
        mScrollArea.SetContentSize(new Vector2(0, mContent.GetFrame().height));
    }
}