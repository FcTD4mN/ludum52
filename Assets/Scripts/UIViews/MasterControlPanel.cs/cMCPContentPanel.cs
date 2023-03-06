using UnityEngine;
using System.Collections.Generic;
using System;

class cMCPContentPanel :
    cPanel
{
    private cMCPHarvesterPanel mHarvesterPanel;
    private cMCPProductionPanel mProducerPanel;
    private cMCPBuffersPanel mBuffersPanel;
    private cMCPWeaponPanel mWeaponsPanel;



    // UI variables
    public float mPadding = 32;
    public float mSpacing = 50;

    public cMCPContentPanel(GameObject parentView, string name, cMasterControlPanel master ) : base(parentView, name)
    {
        mHarvesterPanel = new cMCPHarvesterPanel(mGameObject, "harvesterPanel", master);
        mProducerPanel = new cMCPProductionPanel(mGameObject, "producerPanel", master);
        mBuffersPanel = new cMCPBuffersPanel(mGameObject, "bufferPanel", master);
        mWeaponsPanel = new cMCPWeaponPanel(mGameObject, "weaponPanel", master);
        SetColor(Color.clear);
    }


    public void Update()
    {
        mHarvesterPanel.Update();
        mProducerPanel.Update();
        mBuffersPanel.Update();
        mWeaponsPanel.Update();
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

        var prodPanelHeight = mProducerPanel.RequiredHeightForWidth( frame.width - mPadding * 2 );
        Rect prodPanelFrame = new Rect( mPadding,
                                        harvPanelFrame.yMax + mSpacing,
                                        frame.width - mPadding * 2,
                                        prodPanelHeight );
        mProducerPanel.SetFrame(prodPanelFrame);

        var buffPanelHeight = mBuffersPanel.RequiredHeightForWidth( frame.width - mPadding * 2 );
        Rect buffPanelFrame = new Rect( mPadding,
                                        prodPanelFrame.yMax + mSpacing,
                                        frame.width - mPadding * 2,
                                        buffPanelHeight );
        mBuffersPanel.SetFrame(buffPanelFrame);

        var weaponPanelHeight = mWeaponsPanel.RequiredHeightForWidth( frame.width - mPadding * 2 );
        Rect weaponPanelFrame = new Rect( mPadding,
                                        buffPanelFrame.yMax + mSpacing,
                                        frame.width - mPadding * 2,
                                        weaponPanelHeight );
        mWeaponsPanel.SetFrame(weaponPanelFrame);
    }


    public float RequiredHeightForWidth(float forWidth)
    {
        var frame = GetFrame();
        float harvesterHeight = mHarvesterPanel.RequiredHeightForWidth(frame.width - mPadding * 2);
        float producerHeight = mProducerPanel.RequiredHeightForWidth(frame.width - mPadding * 2);
        float buffHeight = mBuffersPanel.RequiredHeightForWidth(frame.width - mPadding * 2);
        float weaponHeight = mWeaponsPanel.RequiredHeightForWidth(frame.width - mPadding * 2);
        return  mPadding + harvesterHeight + mSpacing + producerHeight + mSpacing + buffHeight + mSpacing + weaponHeight + mPadding;
    }
}
