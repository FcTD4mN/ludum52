using UnityEngine;
using System.Collections.Generic;
using System;

class cMCPResourcesPanel :
    cPanel
{
    // UI variables
    public float mPadding = 25;
    public float mSpacing = 25;
    public float mLineHeight = 30;

    private List<cMCPResourceLine> mResourcesLines;

    public cMCPResourcesPanel(GameObject parentView, string name) : base(parentView, name)
    {
        // SetColor(Color.gray);
        SetColor(Color.clear);
        BuildResourceList();
    }


    public void Update()
    {
        foreach( var line in mResourcesLines )
        {
            line.Update();
        }
    }


    override public void LayoutSubviews()
    {
        var frame = GetFrame();

        var framer = new Rect( mPadding,
                                mPadding,
                                frame.width - mPadding*2,
                                mLineHeight );
        foreach (var line in mResourcesLines)
        {
            line.SetFrame( framer );
            framer = Utilities.OffsetRectBy( framer, new Vector2( 0, mLineHeight + mSpacing ));
        }
    }


    public float RequiredHeightForWidth(float forWidth)
    {
        return mPadding + mResourcesLines.Count * mLineHeight + (mResourcesLines.Count - 1) * mSpacing + mPadding;
    }


    private void BuildResourceList()
    {
        mResourcesLines = new List<cMCPResourceLine>();
        foreach (cResourceDescriptor.eResourceNames resource in Enum.GetValues( typeof(cResourceDescriptor.eResourceNames) ) )
        {
            var resourceLine = new cMCPResourceLine( mGameObject, resource.ToString(), resource );
            mResourcesLines.Add( resourceLine );
        }

        LayoutSubviews();
    }
}





class cMCPResourceLine :
    cPanel
{
    private cImage mIcon;
    private cLabel mName;

    private cLabel mCurrentTotal;
    private cLabel mTotalIncoming;
    private cLabel mTotalConsumed;
    private cLabel mNetRatio;

    private cResourceDescriptor.eResourceNames mResourceName;


    public float mPadding = 0;
    public float mSpacing = 10;

    public cMCPResourceLine(GameObject parentView, string name, cResourceDescriptor.eResourceNames resourceName ) : base(parentView, name)
    {
        mResourceName = resourceName;

        mIcon = new cImage( mGameObject, "icon" );
        mIcon.SetImageFromUnityResources( "Knob" );

        mName = new cLabel( mGameObject, "label" );
        mName.mText.text = resourceName.ToString();
        mName.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mCurrentTotal = new cLabel( mGameObject, "current" );
        mTotalIncoming = new cLabel( mGameObject, "incoming" );
        mTotalConsumed = new cLabel( mGameObject, "consumed" );
        mNetRatio = new cLabel( mGameObject, "total" );

        SetColor(Color.clear);
    }


    override public void LayoutSubviews()
    {
        var frame = GetFrame();
        float valueWidth = 60;
        mIcon.SetFrame( new Rect( mPadding,
                                    0,
                                    frame.height,
                                    frame.height ));

        mName.SetFrame( new Rect( mIcon.GetFrame().xMax + mSpacing,
                                    0,
                                    100,
                                    frame.height ));

        mCurrentTotal.SetFrame( new Rect( mName.GetFrame().xMax + mSpacing*2,
                                        0,
                                        valueWidth,
                                        frame.height ));
        mTotalIncoming.SetFrame( new Rect( mCurrentTotal.GetFrame().xMax + mSpacing,
                                        0,
                                        valueWidth,
                                        frame.height ));
        mTotalConsumed.SetFrame( new Rect( mTotalIncoming.GetFrame().xMax + mSpacing,
                                        0,
                                        valueWidth,
                                        frame.height ));
        mNetRatio.SetFrame( new Rect( mTotalConsumed.GetFrame().xMax + mSpacing,
                                        0,
                                        valueWidth,
                                        frame.height ));
    }


    public void Update()
    {
        float inputValue = 0;
        float outputValue = 0;
        foreach( var building in GameManager.mRTSManager.mAllProductionBuildings )
        {
            inputValue += building.IsPaused() ? 0 : building.mResourceDescriptor.mInputRates[mResourceName] * building.GetProductionRatio();
            outputValue += building.IsPaused() ? 0 : building.mResourceDescriptor.mOutputRates[mResourceName] * building.GetProductionRatio();
        }
        float totalRate = outputValue - inputValue;

        mTotalIncoming.mText.text = GetColorForValue(outputValue ) + (int)outputValue + "</color>";
        mTotalConsumed.mText.text = GetColorForValue(-inputValue) + (int)inputValue + "</color>";
        mNetRatio.mText.text = GetColorForValue(totalRate) + (int)totalRate + "</color>";

        mCurrentTotal.mText.text = ((int)GameManager.mResourceManager.GetRessource( mResourceName )).ToString();
    }


    private string GetColorForValue( float value )
    {
        string color = value < 0 ? "<color=#ff2222>" : "<color=#22ff22>";
        if (value == 0) color = "<color=#bbbbbb>";

        return  color;
    }
}