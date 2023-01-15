using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private ResourceManager mResourceManager;
    public Canvas mCanvas;

    private TextMeshProUGUI mLabelGold;
    private TextMeshProUGUI mLabelIron;
    private TextMeshProUGUI mLabelFire;
    private TextMeshProUGUI mLabelArrows;
    private TextMeshProUGUI mLabelBombs;

    // BuildMenu
    private cBuildMenu mBuildMenu;

    // Info panel
    private GameObject mInfoPanel;
    private TextMeshProUGUI mInfoPanelText;

    // Tooltip panel
    private GameObject mTooltipPanel;
    private TextMeshProUGUI mTooltipPanelText;
    private Coroutine mTooltipCloseCoroutine;

    private GameObject mObjectToBuildTo;
    private GameObject mBuildButtonClicked;
    private GameObject mHoverButton; // Button being visible due to hovering
    private GameObject mHoveredObject; // The object the mouse is currently hovering
    private bool mDidFindHoveredButton = false;


    public List<GameObject> mBuildableObjects;
    private List<GameObject> mBuildButtons; // All UI buttons to click to build buildings
    private List<GameObject> mAllUIFloatingButtons; // All UI buttons to click to open build menu


    private enum eInfoPanelDisplayType
    {
        kShowStatsOfExistingBuilding,
        kShowPrefabStats
    }
    private eInfoPanelDisplayType mInfoPanelDisplay;

    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mResourceManager = GameManager.mResourceManager;

        mLabelGold = GameObject.Find("LabelGold")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelIron = GameObject.Find("LabelIron")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelFire = GameObject.Find("LabelFire")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelArrows = GameObject.Find("LabelArrows")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelBombs = GameObject.Find("LabelBombs")?.gameObject.GetComponent<TextMeshProUGUI>();

        mCanvas = GameObject.Find("UI-RTS")?.gameObject.GetComponent<Canvas>();
        BuildBuildableList();

        // Build Menu
        mBuildMenu = new cBuildMenu( mCanvas.gameObject, "BuildMenu" );
        Rect screenRect = Camera.main.pixelRect;
        mBuildMenu.SetFrame(new Rect(0, 0, 500, 500));
        mBuildMenu.SetCenter(screenRect.center);
        mBuildMenu.mGameObject.SetActive( false );
        BuildBuildMenu();

        // Info Panel
        mInfoPanel = mCanvas.transform.Find("InfoPanel")?.gameObject;
        mInfoPanelText = GameObject.Find("InfoPanelText")?.gameObject.GetComponent<TextMeshProUGUI>();
        BuildInfoPanel();

        // Info Panel
        mTooltipPanel = mCanvas.transform.Find("ToolTipPanel")?.gameObject;
        mTooltipPanelText = mTooltipPanel.transform.Find("text")?.gameObject.GetComponent<TextMeshProUGUI>();
        mTooltipPanelText.text = "ok";
        mTooltipPanel.SetActive( false );

        mAllUIFloatingButtons = new List<GameObject>();
    }


    private void BuildBuildableList()
    {
        mBuildableObjects = new List<GameObject>();
        foreach( Transform child in GameManager.mRTSManager.mRTSWorld.transform )
        {
            foreach (Transform subChild in child )
            {
                if( subChild.tag != "Buildable" ) {
                    continue;
                }

                mBuildableObjects.Add( subChild.gameObject );
            }

            if( child.tag != "Buildable" ) {
                continue;
            }

            mBuildableObjects.Add( child.gameObject );
        }
    }


    // ===================================
    // Update
    // ===================================
    public void UpdateUI()
    {
        mLabelGold.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Gold)).ToString();
        mLabelIron.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Iron)).ToString();
        mLabelFire.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Fire)).ToString();
        mLabelArrows.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Arrows)).ToString();
        mLabelBombs.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Bombs)).ToString();

        mBuildMenu.UpdateBuildMenu();
        UpdateMousePosition();
    }


    public void UpdateMousePosition()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePosWorld;
        RectTransformUtility.ScreenPointToWorldPointInRectangle( mCanvas.transform as RectTransform, mousePosScreen, Camera.main, out mousePosWorld );

        GameObject previousHovered = mHoveredObject;
        mDidFindHoveredButton = false;

        // PROD BUILDINGS
        foreach( ProductionBuilding building in GameManager.mRTSManager.mAllProductionBuildings )
        {
            if( building.gameObject.GetComponent<HarvestingBuilding>() != null ) { continue; }

            Rect bbox = Utilities.GetBBoxFromTransform( building.gameObject );
            if( bbox.Contains( mousePosWorld ) )
            {
                HoveringBuilding( building );
                mDidFindHoveredButton = true;
                break;
            }
        }

        // RECEIVERS
        if( !mDidFindHoveredButton )
        {
            foreach ((GameObject, int) pack in GameManager.mRTSManager.mAllReceivers)
            {
                GameObject receiver = pack.Item1;
                Rect bbox = Utilities.GetBBoxFromTransform(receiver);
                if (bbox.Contains(mousePosWorld))
                {
                    HoveringReceiver(receiver);
                    mDidFindHoveredButton = true;
                    break;
                }
            }
        }

        if (!mDidFindHoveredButton) mBuildMenu.UpdateMouse();

        if( !mDidFindHoveredButton ) mHoveredObject = null;
        if( mHoveredObject == null && mHoverButton != null )
        {
            DeleteUIButton( mHoverButton );
            mHoverButton = null;
        }

        UpdateInfoPanel();
    }


    // ===================================
    // UI Stuff
    // ===================================
    public void ClearUIForSwitchingView()
    {
        DeleteAllUIFloatingButtons();
        mBuildMenu.mGameObject.SetActive(false);
        mInfoPanel.SetActive(false);

        if (mHoverButton != null) DeleteUIButton(mHoverButton);
    }


    public void CreateBuildButtonOnEveryBuildableObject()
    {
        foreach( GameObject buildable in mBuildableObjects )
        {
            if( !buildable.activeSelf ) { continue; }

            CreateBuildButtonOverObject( buildable );
        }
    }


    public void CreateBuildButtonOverObject( GameObject obj )
    {
        GameObject newButton = CreateButtonOverObject( "ButtonBuild", obj );
        mAllUIFloatingButtons.Add( newButton );
        newButton.GetComponent<Button>().onClick.AddListener( () => {

            mObjectToBuildTo = obj;
            mBuildButtonClicked = newButton;
            ShowBuildMenu();

        });
    }


    private void CreatePauseButtonOverBuilding( GameObject building )
    {
        mHoverButton = CreateButtonOverObject( "ButtonPause", building );
        GameObject text = mHoverButton.transform.Find("text")?.gameObject;

        bool isReceiver = false;
        ProductionBuilding prodBuilding = building.GetComponent<ProductionBuilding>();
        if( building.GetComponent<Receiver>() != null )
        {
            isReceiver = true;
            prodBuilding = building.GetComponent<Receiver>().mAssociatedHarvester.GetComponent<ProductionBuilding>();
        }

        text.GetComponent<TextMeshProUGUI>().text = prodBuilding.IsPaused() ? "Resume" : "Pause";

        GameObject deleteButton = mHoverButton.transform.Find("Delete")?.gameObject;
        deleteButton.GetComponent<Button>().onClick.AddListener(() =>
        {

            if( !isReceiver ) // We reactivate the buildable object + reshow a build button
            {
                foreach( GameObject obj in mBuildableObjects )
                {
                    if( obj.transform.position == prodBuilding.transform.position )
                    {
                        CreateBuildButtonOverObject( obj );
                        break;
                    }
                }
            }
            GameManager.mRTSManager.DestroyBuilding( building );
            DeleteUIButton( mHoverButton );

        });

        mHoverButton.GetComponent<Button>().onClick.AddListener( () => {

            prodBuilding.SetPause( !prodBuilding.IsPaused() );
            UpdateInfoPanel();
            text.GetComponent<TextMeshProUGUI>().text = prodBuilding.IsPaused() ? "Resume" : "Pause";

        });
    }


    private GameObject CreateButtonOverObject( string prefabName, GameObject obj )
    {
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/RTS/UI/" + prefabName);

        Rect objectBBox = Utilities.GetBBoxFromTransform(obj);
        Rect screenOjbBBox = Utilities.WorldToScreenRect( objectBBox );

        GameObject newButton = Instantiate( buttonPrefab,
                                            new Vector3( screenOjbBBox.center.x, screenOjbBBox.center.y, 0 ),
                                            Quaternion.Euler(0, 0, 0) );
        newButton.transform.SetParent( mCanvas.transform );
        float buttongPadding = 10;
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2( screenOjbBBox.width - buttongPadding, screenOjbBBox.height - buttongPadding );

        return  newButton;
    }


    public void DeleteAllUIFloatingButtons()
    {
        foreach( GameObject button in mAllUIFloatingButtons )
        {
            GameObject.Destroy( button );
        }

        mAllUIFloatingButtons.Clear();
    }


    public void DeleteUIButton( GameObject obj )
    {
        mAllUIFloatingButtons.Remove( obj );
        GameObject.Destroy( obj );
    }


    // ===================================
    // Mouse Hovering
    // ===================================
    private void HoveringBuilding(ProductionBuilding building)
    {
        if (mHoveredObject == building.gameObject && mHoverButton != null) return;
        if (mHoveredObject != building.gameObject) DeleteUIButton(mHoverButton);

        mHoveredObject = building.gameObject;
        mInfoPanelDisplay = eInfoPanelDisplayType.kShowStatsOfExistingBuilding;

        CreatePauseButtonOverBuilding(building.gameObject);
    }


    private void HoveringReceiver(GameObject receiver)
    {
        GameObject associatedReceiverObject = receiver.GetComponent<Receiver>().mAssociatedHarvester.gameObject;
        if (mHoveredObject == associatedReceiverObject && mHoverButton != null) return;
        if (mHoveredObject != associatedReceiverObject) DeleteUIButton(mHoverButton);

        mHoveredObject = associatedReceiverObject;
        mInfoPanelDisplay = eInfoPanelDisplayType.kShowStatsOfExistingBuilding;

        CreatePauseButtonOverBuilding( receiver );
    }


    // ===================================
    // Build Menu
    // ===================================
    public void ShowBuildMenu()
    {
        bool isLocationOnTower = GameManager.mRTSManager.mTowers.Contains(mObjectToBuildTo.transform.parent.gameObject);
        bool isLocationOnBuffTower = GameManager.mRTSManager.mBuffTower.Contains(mObjectToBuildTo.transform.parent.gameObject);

        if( isLocationOnTower ) mBuildMenu.ShowProdBuildingPanel();
        if( isLocationOnBuffTower ) mBuildMenu.ShowBuffBuildingPanel();
        mBuildMenu.UpdateBuildMenu();
        mBuildMenu.mGameObject.transform.SetAsLastSibling();
        mBuildMenu.mGameObject.SetActive( true );
    }


    private void BuildBuildMenu()
    {
        mBuildMenu.mOnClose = () =>
        {
            mBuildMenu.mGameObject.SetActive(false);
        };


        mBuildMenu.mOnBuildingClicked = (building)=> {

            mBuildMenu.mGameObject.SetActive(false);
            GameManager.mRTSManager.BuildBuffBuildingAtLocation( building.ToString(), mObjectToBuildTo);
            DeleteUIButton(mBuildButtonClicked);

        };

        mBuildMenu.mOnHover = (building) =>
        {
            if( building == null ) return;
            ProductionBuilding prod = GameManager.mRTSManager.GetPrefabByType( (RTSManager.eBuildingList)building );

            if (mHoveredObject == prod && mHoverButton != null) return;
            if (mHoveredObject != prod) DeleteUIButton(mHoverButton);

            mHoveredObject = prod.gameObject;
            mDidFindHoveredButton = true;
            mInfoPanelDisplay = eInfoPanelDisplayType.kShowPrefabStats;
        };
    }

    // ===================================
    // Info Panel
    // ===================================
    private void BuildInfoPanel()
    {
        mInfoPanel.SetActive( false );
    }

    private void UpdateInfoPanel()
    {
        mInfoPanel.SetActive(mHoveredObject != null);

        if( mHoveredObject != null )
        {
            // PROD BUILDING
            ProductionBuilding prodBuilding = mHoveredObject.GetComponent<ProductionBuilding>();
            if( prodBuilding == null ) return;

            switch (mInfoPanelDisplay)
            {
                case eInfoPanelDisplayType.kShowStatsOfExistingBuilding:
                    mInfoPanelText.text = prodBuilding.IsPaused() ? "Paused" : prodBuilding.mResourceDescriptor.PrintProductionRates();
                    break;

                case eInfoPanelDisplayType.kShowPrefabStats:
                    mInfoPanelText.text = prodBuilding.GetUIDescription( true );
                    break;

                default:
                    break;
            }
        }

    }



    public void DisplayMessage( string message, float duration )
    {
        if( mTooltipCloseCoroutine != null ){
            StopCoroutine( mTooltipCloseCoroutine );
        }
        mTooltipCloseCoroutine = null;

        mTooltipPanelText.text = message;
        mTooltipPanel.SetActive( true );

        mTooltipCloseCoroutine = StartCoroutine( Utilities.ExecuteAfter( duration, () => {
            mTooltipPanel.SetActive( false );
            mTooltipCloseCoroutine = null;
         } ));
    }

    public void FloatingMessage( string message, Color color, Vector3 position )
    {
        Animation anim = new Animation( this );
        anim.DisplayAnimatedText( position, message, color, 2, 30, mCanvas.transform );
    }
}
