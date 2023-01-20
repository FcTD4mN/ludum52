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

    private cLabel mLabelProdRate;
    private cButton mButtonPause;

    // Unity can't change material properties on UI elements, so we have to patch it by reinstancing a new material copy...
    // This is used to reduce the amount of material recreation
    private int mMaterialPatcher = -1;


    static public float mBuildingLineSpacing = 10;
    static public float mBuildingLineNameWidth = 200f;
    static public float mBuildingLineResourceWidth = 250;
    static public float mBuildingLineEfficiencyWidth = 70;
    static public float mBuildingLineButtonPauseWidth = 50;

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

        mLabelProdRate = new cLabel(mGameObject, "prodRate");
        mLabelProdRate.mText.fontStyle = TMPro.FontStyles.Bold;
        mLabelProdRate.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mButtonPause = new cButton( mGameObject, "buttonPause" );
        mButtonPause.SetText( "||" );
        mButtonPause.SetColor( Color.black );
        mButtonPause.AddOnClickAction( OnPauseClick );

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

        var labelProdFrame = new Rect( titleFrame.xMax + mBuildingLineSpacing + mBuildingLineResourceWidth * 2f + mBuildingLineSpacing * 2f,
                                    0,
                                    mBuildingLineEfficiencyWidth,
                                    frame.height );
        mLabelProdRate.SetFrame( labelProdFrame );

        var buttonPauseFrame = new Rect( frame.width - mBuildingLineSpacing - mBuildingLineButtonPauseWidth,
                                            0,
                                            mBuildingLineButtonPauseWidth,
                                            frame.height );
        mButtonPause.SetFrame( buttonPauseFrame );
    }


    public void Update()
    {
        float prodRatio = mAssociatedBuilding.IsPaused() ? 0 : mAssociatedBuilding.GetProductionRatio();

        mLabelProdRate.mText.text = (int)(prodRatio*100f) + "%";

        foreach (var inputView in mInputViews)
        {
            inputView.Update();
        }

        foreach (var output in mOutputViews)
        {
            output.Update();
        }

        var pauseText = mAssociatedBuilding.IsPaused() ? ">" : "||";
        mButtonPause.SetText(pauseText);

        if (mAssociatedBuilding.IsPaused() )
        {
            if (mMaterialPatcher == 0) return;
            mDiodeMat = new Material( mDiodeMat );
            mDiode.mImage.material = mDiodeMat;
            mDiodeMat.SetColor("_ColorA", new Color(1, 0.5f, 0, 1));
            mDiodeMat.SetColor("_ColorB", new Color(1, 0.6f, 0.1f, 1));
            mMaterialPatcher = 0;
        }
        else if (prodRatio < 1)
        {
            if( mMaterialPatcher == 1 ) return;
            mDiodeMat = new Material(mDiodeMat);
            mDiode.mImage.material = mDiodeMat;
            mDiodeMat.SetColor("_ColorA", Color.red);
            mDiodeMat.SetColor("_ColorB", Color.black);
            mMaterialPatcher = 1;
        }
        else
        {
            if (mMaterialPatcher == 2) return;
            mDiodeMat = new Material(mDiodeMat);
            mDiode.mImage.material = mDiodeMat;
            mDiodeMat.SetColor("_ColorA", Color.green);
            mDiodeMat.SetColor("_ColorB", new Color(0.1f, 1, 0.1f, 1));
            mMaterialPatcher = 2;
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
                                                        cResourceDescriptor.eResourceType.kInput,
                                                        "#ff2222" );

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
                                                        cResourceDescriptor.eResourceType.kOutput,
                                                        "#22ff22" );

            mOutputViews.Add(newResourceView);
        }
    }

    // This whole method is now the best way i feel like, pausing should be handled in SetPause() override, a harvester can't be paused, it will always
    // unplug, like, pausing means unpluging.
    // But then, for now, it means you shouldn't be able to hit pause on the receiver hover button, cause Pause => then object disappears is kinda weird
    private void OnPauseClick()
    {
        mAssociatedBuilding.SetPause(!mAssociatedBuilding.IsPaused());
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
    private string mColor;

    public cResourceView(GameObject parentView,
                            string name,
                            ProductionBuilding building,
                            string resourceName,
                            cResourceDescriptor.eResourceType type,
                            string color ) : base(parentView, name)
    {
        mBuilding = building;
        mResourceName = resourceName;
        mResourceType = type;
        mColor = color;

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

        float value = mBuilding.IsPaused() ? 0 : GetResourceValue() * buildingRatio;
        string colorTagIn = value == 0 ? "<color=#bbbbbb>" : "<color="+mColor+">";
        mLabelValue.mText.text = colorTagIn + ((int)value).ToString() + "</color>";
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
