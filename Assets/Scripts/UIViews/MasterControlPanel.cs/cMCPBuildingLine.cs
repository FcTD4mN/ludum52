using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

class cBuildingLine :
    cPanel
{
    private cImage mDiode;
    private Material mDiodeMat;
    private cLabel mTitle;
    private cButton mEmptyButton;

    private WeakReference<cMasterControlPanel> mMaster;

    private cPanel mHoverInfos;
    private cLabel mHoverInfosText;

    private ProductionBuilding mAssociatedBuilding;
    private HarvestingBuilding  _mAssociatedBuildingHarvesterComp;  // Can be null, it's to avoid constant GetComponent calls
    private BuffBuilding        _mAssociatedBuildingBuffComp;       // Can be null, it's to avoid constant GetComponent calls

    private List<cResourceView> mInputViews;
    private List<cResourceView> mOutputViews;

    private cLabel mLabelProdRate;
    private cButton mButtonPause;

    public Action<cBuildingLine> mOnClickEmptyEvent;

    // Unity can't change material properties on UI elements, so we have to patch it by reinstancing a new material copy...
    // This is used to reduce the amount of material recreation
    private int mMaterialPatcher = -1;


    static public float mBuildingLineSpacing = 10;
    static public float mBuildingLineNameWidth = 200f;
    static public float mBuildingLineResourceWidth = 250;
    static public float mBuildingLineEfficiencyWidth = 70;
    static public float mBuildingLineButtonPauseWidth = 50;

    public cBuildingLine( GameObject parentView, string name, ProductionBuilding building, cMasterControlPanel master ) : base(parentView, name)
    {
        mMaster = new WeakReference<cMasterControlPanel>(master);

        mTitle = new cLabel(mGameObject, "title");
        mTitle.mText.fontStyle = TMPro.FontStyles.Bold;
        mTitle.mText.alignment = TMPro.TextAlignmentOptions.Left;
        var hoverable = mTitle.mGameObject.AddComponent<Hoverable>();
        hoverable.mOnHoverAction = Hover;
        hoverable.mOnHoverEndedAction = HoverEnded;

        mEmptyButton = new cButton( mGameObject, "emptyButton" );
        mEmptyButton.SetText( "<Empty>" );
        mEmptyButton.SetColor( new Color( 0.3f,0.3f,0.3f,0.3f ) );
        mEmptyButton.AddOnClickAction( ()=> {
            mOnClickEmptyEvent?.Invoke( this );
        });

        mLabelProdRate = new cLabel(mGameObject, "prodRate");
        mLabelProdRate.mText.fontStyle = TMPro.FontStyles.Bold;
        mLabelProdRate.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mButtonPause = new cButton( mGameObject, "buttonPause" );
        mButtonPause.SetText( "||" );
        mButtonPause.SetColor( Color.black );
        mButtonPause.AddOnClickAction( OnPauseClick );

        mInputViews = new List<cResourceView>();
        mOutputViews = new List<cResourceView>();

        SetColor(Color.clear);

        BuildUIForBuilding( building );
    }


    public void BuildUIForBuilding( ProductionBuilding building )
    {
        mAssociatedBuilding = building;
        _mAssociatedBuildingHarvesterComp = building?.GetComponent<HarvestingBuilding>();
        _mAssociatedBuildingBuffComp = building?.GetComponent<BuffBuilding>();

        mTitle.mText.text = building == null ? "<Empty>" : building.GetDisplayName();

        foreach (var view in mInputViews)
        {
            GameObject.Destroy(view.mGameObject);
        }
        mInputViews.Clear();

        foreach (var view in mOutputViews)
        {
            GameObject.Destroy(view.mGameObject);
        }
        mOutputViews.Clear();

        bool buildingIsNotNull = building != null;
        mButtonPause.mGameObject.SetActive( buildingIsNotNull );
        mLabelProdRate.mGameObject.SetActive( buildingIsNotNull );
        mDiode?.mGameObject.SetActive( buildingIsNotNull );

        mTitle.mGameObject.SetActive( buildingIsNotNull );
        mEmptyButton.mGameObject.SetActive( !buildingIsNotNull );

        mDiode = new cImage(mGameObject, "diode");
        mDiode.SetImageFromUnityResources("Knob");

        Shader diodeShader = Resources.Load<Shader>("Shaders/DiodeShader");
        mDiodeMat = new Material(diodeShader);
        mDiodeMat.mainTexture = mDiode.mImage.sprite.texture;
        mDiode.mImage.material = mDiodeMat;

        BuildInputs();
        BuildOutputs();

        LayoutSubviews();
    }


    public override void LayoutSubviews()
    {
        var frame = GetFrame();

        bool diodeIsThere = mDiode != null;
        float diodeSize = frame.height;

        if( diodeIsThere )
        {
            mDiode.SetFrame(new Rect(0,
                                        0,
                                        diodeSize,
                                        diodeSize));
        }

        // Always aligned to diode size, so that text will alin with any existing building
        Rect titleFrame = new Rect( diodeSize + mBuildingLineSpacing,
                                    0,
                                    mBuildingLineNameWidth,
                                    frame.height);
        mTitle.SetFrame(titleFrame);
        mEmptyButton.SetFrame( titleFrame );

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
        // Diode update
        DiodeUpdate();

        if( mAssociatedBuilding == null ) { return; }

        float prodRatio = mAssociatedBuilding.IsPaused() ? 0 : mAssociatedBuilding.GetProductionRatio();

        // Prod label update
        var prodColor = Color.Lerp( Color.red, Color.green, prodRatio );
        string hex = ColorUtility.ToHtmlStringRGB( prodColor );
        mLabelProdRate.mText.text = "<color=#" + hex + ">" + (int)(prodRatio*100f) + "%" + "</color>";

        // Input updates
        foreach (var inputView in mInputViews)
        {
            inputView.Update();
        }

        // Output updates
        foreach (var output in mOutputViews)
        {
            output.Update();
        }

        // Pause button update
        var pauseText = mAssociatedBuilding.IsPaused() ? ">" : "||";
        mButtonPause.SetText(pauseText);

        // Tooltips update
        if (_mAssociatedBuildingHarvesterComp != null) UpdateHarvesterToolTip();
        if (_mAssociatedBuildingBuffComp != null) UpdateBufferToolTip();
    }


    private void DiodeUpdate()
    {
        if (mAssociatedBuilding == null)
        {
            if (mMaterialPatcher == 5) return;
            mDiodeMat = new Material(mDiodeMat);
            mDiode.mImage.material = mDiodeMat;
            mDiodeMat.SetColor("_ColorA", new Color(0f, 0f, 0f, 1));
            mDiodeMat.SetColor("_ColorB", new Color(0f, 0f, 0f, 1));
            mMaterialPatcher = 5;
            return;
        }

        float prodRatio = mAssociatedBuilding.IsPaused() ? 0 : mAssociatedBuilding.GetProductionRatio();
        if (mAssociatedBuilding.IsPaused())
        {
            if (mMaterialPatcher == 0) return;
            mDiodeMat = new Material(mDiodeMat);
            mDiode.mImage.material = mDiodeMat;
            mDiodeMat.SetColor("_ColorA", new Color(1, 0.5f, 0, 1));
            mDiodeMat.SetColor("_ColorB", new Color(1, 0.6f, 0.1f, 1));
            mMaterialPatcher = 0;
        }
        else if (prodRatio < 1)
        {
            if (mMaterialPatcher == 1) return;
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
        if( mAssociatedBuilding == null ) { return; }

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
        if (mAssociatedBuilding == null) { return; }

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
        if( mAssociatedBuilding == null ) { return; }

        mAssociatedBuilding.SetPause(!mAssociatedBuilding.IsPaused());
    }


    private void Hover()
    {
        if( _mAssociatedBuildingHarvesterComp == null && _mAssociatedBuildingBuffComp == null ) return;
        if( mAssociatedBuilding.GetComponent<HarvesterTower>() != null ) return;
        if (mHoverInfos != null) return;

        cMasterControlPanel master;
        mMaster.TryGetTarget(out master);
        if( master == null ) return;

        mHoverInfos = new cPanel(master?.mGameObject, "hoverInfos");
        mHoverInfosText = new cLabel(mHoverInfos.mGameObject, "text");
        mHoverInfosText.mText.alignment = TMPro.TextAlignmentOptions.TopLeft;
    }

    private void HoverEnded()
    {
        if( mHoverInfos == null ) return;

        GameObject.Destroy(mHoverInfos.mGameObject);
        mHoverInfos = null;
    }


    private void UpdateHarvesterToolTip()
    {
        if( mHoverInfos == null ) return;
        if( _mAssociatedBuildingHarvesterComp == null ) return;

        string finalText = _mAssociatedBuildingHarvesterComp.GetBuildingType().ToString() + " source: \n";
        foreach( var resource in _mAssociatedBuildingHarvesterComp.mResourceVein.mResource.mAvailable )
        {
            // Uses input to figure which resources are inside the vein
            // This wouldn't work well for resources that would have to replenish capabilities
            // Best would be to not set all fields of dictionnaries to 0, but only the one relevants
            // so we would have nothing to do in order to not show every resource with a value of 0 here
            if( _mAssociatedBuildingHarvesterComp.mResourceVein.mResource.mInputRates[resource.Key] == 0 ) continue;

            finalText += " " + resource.Key.ToString() + ": " + ((int)resource.Value).ToString() + "\n";
        }

        mHoverInfosText.mText.text = finalText;

        UpdateHoverInfosFrameToFit();
    }


    private void UpdateBufferToolTip()
    {
        if (mHoverInfos == null) return;
        if (_mAssociatedBuildingBuffComp == null) return;

        string finalText = "Stats buffed: \n";
        foreach (var resource in _mAssociatedBuildingBuffComp.mStatsModifiers.mStatValues)
        {
            if (resource.Value == 0) continue;

            finalText += " " + resource.Key.ToString() + ": " + ((int)resource.Value).ToString() + "\n";
        }

        var text = mHoverInfos.mGameObject.transform.Find("text").GetComponent<TextMeshProUGUI>();
        text.text = finalText;

        UpdateHoverInfosFrameToFit();
    }


    private void UpdateHoverInfosFrameToFit()
    {
        cMasterControlPanel master;
        mMaster.TryGetTarget(out master);
        if (master == null) return;

        var frame = GetFrameRelativeTo(master?.mGameObject);
        mHoverInfosText.mText.ForceMeshUpdate();
        var textWidth = mHoverInfosText.mText.textBounds.size.x;
        var textHeight = mHoverInfosText.mText.textBounds.size.y;

        mHoverInfos.SetFrame(new Rect(frame.xMin, frame.yMin + frame.height, textWidth + 20, textHeight + 20));
        mHoverInfosText.SetFrame(new Rect(10, 10, textWidth, textHeight));
    }
}



























class cResourceView :
    cPanel
{
    private cLabel mLabelName;
    private cLabel mLabelValue;

    private ProductionBuilding mBuilding;
    private cResourceDescriptor.eResourceNames mResourceName;
    private cResourceDescriptor.eResourceType mResourceType;
    private string mColor;

    public cResourceView(GameObject parentView,
                            string name,
                            ProductionBuilding building,
                            cResourceDescriptor.eResourceNames resourceName,
                            cResourceDescriptor.eResourceType type,
                            string color ) : base(parentView, name)
    {
        mBuilding = building;
        mResourceName = resourceName;
        mResourceType = type;
        mColor = color;

        SetColor( Color.clear );

        mLabelName = new cLabel( mGameObject, "labelName" );
        mLabelName.mText.text = "" + mResourceName.ToString()[0];

        mLabelValue = new cLabel( mGameObject, "labelValue" );
        mLabelValue.mText.text = GetResourceValue().ToString();
    }


    public void Update()
    {
        float buildingRatio = mBuilding.GetProductionRatio();

        mLabelName.mText.text = "" + mResourceName.ToString()[0];

        float value = mBuilding.IsPaused() ? 0 : GetResourceValue() * buildingRatio;
        string colorTagIn = value == 0 ? "<color=#bbbbbb>" : "<color="+mColor+">";
        mLabelValue.mText.text = colorTagIn + ((int)value).ToString() + "</color>";
    }


    public override void LayoutSubviews()
    {
        var frame = GetFrame();

        mLabelName.mText.ForceMeshUpdate();
        mLabelName.SetFrame( new Rect( 0,
                                        0,
                                        mLabelName.mText.textBounds.size.x,
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
