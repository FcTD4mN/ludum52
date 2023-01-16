using UnityEngine;
using System.Collections.Generic;
using System;

class cMCPContentPanel :
    cPanel
{
    private cMCPHarvesterPanel mHarvesterPanel;


    // UI variables
    public float mPadding = 32;

    private List<cBuildingLine> mAllHarvestersLines;


    public cMCPContentPanel(GameObject parentView, string name) : base(parentView, name)
    {
        mHarvesterPanel = new cMCPHarvesterPanel(mGameObject, "harvesterPanel");
        SetColor(Color.clear);
    }


    public void Update()
    {
        mHarvesterPanel.Update();
    }


    override public void LayoutSubviews()
    {
        var frame = GetFrame();

        var harvPanelHeight = mHarvesterPanel.RequiredHeightForWidth( frame.width - mPadding * 2 );
        Rect harvPanelFrame = new Rect( mPadding,
                                        mPadding,
                                        frame.width - mPadding * 2,
                                        harvPanelHeight );
        mHarvesterPanel.SetFrame(harvPanelFrame);
    }


    public float RequiredHeightForWidth(float forWidth)
    {
        var frame = GetFrame();
        return  mPadding + mHarvesterPanel.RequiredHeightForWidth(frame.width - mPadding * 2) + mPadding;
    }
}
