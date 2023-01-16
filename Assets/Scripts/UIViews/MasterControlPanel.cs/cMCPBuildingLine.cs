using UnityEngine;
using System.Collections.Generic;
using System;

class cBuildingLine :
    cPanel
{
    private cImage mDiode;
    private Material mDiodeMat;
    private cLabel mTitle;

    private ProductionBuilding mAssociatedBuilding;

    private List<cResourceView> mInputViews;
    private List<cResourceView> mOutputViews;


    static public float mBuildingLineSpacing = 10;
    static public float mBuildingLineNameWidth = 200f;
    static public float mBuildingLineResourceWidth = 250;

    public cBuildingLine(GameObject parentView, string name, ProductionBuilding building) : base(parentView, name)
    {
        mTitle = new cLabel(mGameObject, "title");
        mTitle.mText.text = building.GetDisplayName();
        mTitle.mText.fontStyle = TMPro.FontStyles.Bold;
        mTitle.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mDiode = new cImage(mGameObject, "diode");
        mDiode.SetImageFromUnityResources("Knob");

        Shader diodeShader = Resources.Load<Shader>( "Shaders/DiodeShader" );
        mDiodeMat = new Material( diodeShader );
        mDiodeMat.mainTexture = mDiode.mImage.sprite.texture;
        mDiode.mImage.material = mDiodeMat;

        mAssociatedBuilding = building;

        SetColor(Color.clear);

        BuildInputs();
        BuildOutputs();
    }


    public override void LayoutSubviews()
    {
        var frame = GetFrame();

        mDiode.SetFrame(new Rect(0,
                                    0,
                                    frame.height,
                                    frame.height));

        Rect titleFrame = new Rect(mDiode.GetFrame().xMax + mBuildingLineSpacing,
                                    0,
                                    mBuildingLineNameWidth,
                                    frame.height);
        mTitle.SetFrame(titleFrame);

        var resourceWidth = 40f;
        var inputFrame = new Rect(  titleFrame.xMax + mBuildingLineSpacing,
                                        0,
                                        resourceWidth,
                                        frame.height );
        foreach( var inputView in mInputViews )
        {
            inputView.SetFrame( inputFrame );
            inputFrame = Utilities.OffsetRectBy( inputFrame, new Vector2( resourceWidth + mBuildingLineSpacing, 0 ) );
        }

        var outputFrame = new Rect(titleFrame.xMax + mBuildingLineSpacing + mBuildingLineResourceWidth,
                                        0,
                                        resourceWidth,
                                        frame.height);
        foreach( var output in mOutputViews )
        {
            output.SetFrame( outputFrame );
            outputFrame = Utilities.OffsetRectBy( outputFrame, new Vector2( resourceWidth + mBuildingLineSpacing, 0 ) );
        }
    }


    public void Update()
    {
        float prodRatio = mAssociatedBuilding.GetProductionRatio();

        foreach (var inputView in mInputViews)
        {
            inputView.Update();
        }

        foreach (var output in mOutputViews)
        {
            output.Update();
        }

        if (mAssociatedBuilding.IsPaused())
        {
            mDiodeMat.SetColor("_ColorA", new Color(1, 0.5f, 0, 1));
            mDiodeMat.SetColor("_ColorB", new Color(1, 0.6f, 0.1f, 1));
        }
        else if (prodRatio < 1)
        {
            mDiodeMat.SetColor("_ColorA", Color.red);
            mDiodeMat.SetColor("_ColorB", Color.black);
        }
        else
        {
            mDiodeMat.SetColor("_ColorA", Color.green);
            mDiodeMat.SetColor("_ColorB", new Color(0.1f, 1, 0.1f, 1));
        }
    }


    private void BuildInputs()
    {
        mInputViews = new List<cResourceView>();
        var buildingResourceDescriptor = mAssociatedBuilding.GetResourceDescriptor();
        foreach( var resource in buildingResourceDescriptor.mInputRates )
        {
            if( resource.Value == 0 ) continue;

            var newResourceView = new cResourceView( mGameObject,
                                                        "resInputView",
                                                        mAssociatedBuilding,
                                                        resource.Key,
                                                        cResourceDescriptor.eResourceType.kInput );

            mInputViews.Add( newResourceView );
        }
    }


    private void BuildOutputs()
    {
        mOutputViews = new List<cResourceView>();
        var buildingResourceDescriptor = mAssociatedBuilding.GetResourceDescriptor();
        foreach (var resource in buildingResourceDescriptor.mOutputRates)
        {
            if (resource.Value == 0) continue;

            var newResourceView = new cResourceView(mGameObject,
                                                        "resOutputView",
                                                        mAssociatedBuilding,
                                                        resource.Key,
                                                        cResourceDescriptor.eResourceType.kOutput);

            mOutputViews.Add(newResourceView);
        }
    }
}


class cResourceView :
    cPanel
{
    private cLabel mLabelName;
    private cLabel mLabelValue;

    private ProductionBuilding mBuilding;
    private string mResourceName;
    private cResourceDescriptor.eResourceType mResourceType;

    public cResourceView(GameObject parentView, string name, ProductionBuilding building, string resourceName, cResourceDescriptor.eResourceType type )
                            : base(parentView, name)
    {
        mBuilding = building;
        mResourceName = resourceName;
        mResourceType = type;

        SetColor( Color.clear );

        mLabelName = new cLabel( mGameObject, "labelName" );
        mLabelName.mText.text = "" + mResourceName[0];

        mLabelValue = new cLabel( mGameObject, "labelValue" );
        mLabelValue.mText.text = GetResourceValue().ToString();
    }


    public void Update()
    {
        float buildingRatio = mBuilding.GetProductionRatio();

        mLabelName.mText.text = "" + mResourceName[0];
        mLabelValue.mText.text = mBuilding.IsPaused() ? "0" : (GetResourceValue() * buildingRatio).ToString();
    }


    public override void LayoutSubviews()
    {
        var frame = GetFrame();

        mLabelName.SetFrame( new Rect( 0,
                                        0,
                                        frame.height,
                                        frame.height ));

        mLabelValue.SetFrame( new Rect( mLabelName.GetFrame().xMax,
                                        0,
                                        frame.height,
                                        frame.height ));
    }

    private float GetResourceValue()
    {
        var resourceDescriptor = mBuilding.GetResourceDescriptor();
        switch ( mResourceType )
        {
            case cResourceDescriptor.eResourceType.kBuildCost:
                return  resourceDescriptor.mBuildCosts[mResourceName];
            case cResourceDescriptor.eResourceType.kInput:
                return  resourceDescriptor.mInputRates[mResourceName];
            case cResourceDescriptor.eResourceType.kOutput:
                return  resourceDescriptor.mOutputRates[mResourceName];
        }

        return  -1f;
    }
}
