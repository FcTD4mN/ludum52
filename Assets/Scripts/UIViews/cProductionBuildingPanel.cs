using UnityEngine;
using System.Collections.Generic;
using System;

class cProductionBuildingPanel:
    cPanel
{
    private cLabel mTitle;
    private List<RTSManager.eBuildingList> mBuildingToShow;

    public List<(string, cButton)> mButtons;
    public Action<RTSManager.eBuildingList> mOnBuildingClicked;

    // UI variables
    public int mPadding = 32;
    public int mTitleHeight = 24;
    public int mButtonSize = 128;
    public int mSpacing = 12;


    public cProductionBuildingPanel(GameObject parentView, string name) : base(parentView, name)
    {
        mTitle = new cLabel(mGameObject, "Title");
        mTitle.mText.text = "Production Buildings:";
        mTitle.mText.fontSize = 24;
        mTitle.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mButtons = new List<(string, cButton)>();
        mBuildingToShow = new List<RTSManager.eBuildingList>();
        mBuildingToShow.Add( RTSManager.eBuildingList.Forge );
        mBuildingToShow.Add( RTSManager.eBuildingList.BombFactory );
        mBuildingToShow.Add( RTSManager.eBuildingList.Workshop );

        BuildButtons();
    }


    override public void LayoutSubviews()
    {
        Rect frame = GetFrame();

        mTitle.SetFrame( new Rect( mPadding,
                                    frame.height - mTitleHeight - mPadding,
                                    frame.width - mPadding*2,
                                    mTitleHeight) );


        int numberOfButtonPerRow = (int)((frame.width - mPadding*2) / (float)(mButtonSize + mSpacing));
        if (numberOfButtonPerRow == 0) numberOfButtonPerRow = 1;

        int rowWidth = numberOfButtonPerRow*mButtonSize + (numberOfButtonPerRow-1)*mSpacing;

        int i = 0;
        Rect buttonFrame = new Rect( (frame.width - rowWidth) / 2f,
                                        mTitle.GetFrame().yMin - mSpacing - mButtonSize,
                                        mButtonSize,
                                        mButtonSize );
        foreach( (string, cButton) pair in mButtons )
        {
            int rowIndex = i % numberOfButtonPerRow;
            int columnIndex = i / numberOfButtonPerRow;
            Vector2 offset = new Vector2( rowIndex * (mButtonSize + mSpacing),
                                          -columnIndex * (mButtonSize + mSpacing) );

            cButton button = pair.Item2;
            button.SetFrame( Utilities.OffsetRectBy(buttonFrame, offset) );

            i++;
        }
    }


    public int RequiredHeightForWidth( int forWidth )
    {
        int numberOfButtonPerRow = (int)((forWidth - mPadding * 2) / (float)(mButtonSize + mSpacing));
        if (numberOfButtonPerRow == 0) numberOfButtonPerRow = 1;
        int totalButtonCount = mButtons.Count;

        int numberOfRows = Mathf.CeilToInt((float)totalButtonCount / (float)numberOfButtonPerRow);
        int contentSize = numberOfRows * mButtonSize + (numberOfRows-1) * mSpacing;

        return mPadding + mTitleHeight + mSpacing + contentSize + mPadding;
    }


    private void BuildButtons()
    {
        foreach( RTSManager.eBuildingList building in mBuildingToShow )
        {
            string buildingName = building.ToString();
            cButton button = new cButton( mGameObject, buildingName );
            button.AddText( buildingName );
            button.AddOnClickAction( () => {

                Debug.Log("Clicked: " + button.mGameObject.name);
                mOnBuildingClicked?.Invoke(building);

            });

            mButtons.Add( (buildingName, button) );

            // Temporary bg color before png images of buildings
            switch( building )
            {
                case RTSManager.eBuildingList.Forge:
                    button.SetColor( new Color( 0.63f, 0.38f, 0.65f, 1f ) );
                    break;
                case RTSManager.eBuildingList.BombFactory:
                    button.SetColor(new Color(0.65f, 0.38f, 0.38f, 1f));
                    break;
                case RTSManager.eBuildingList.Workshop:
                    button.SetColor(new Color(0.38f, 0.39f, 0.65f, 1f));
                    break;
            }
        }

        LayoutSubviews();
    }
}

