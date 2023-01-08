using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private ResourceManager mResourceManager;
    private Canvas mCanvas;

    private TextMeshProUGUI mLabelGold;
    private TextMeshProUGUI mLabelIron;
    private TextMeshProUGUI mLabelFire;
    private TextMeshProUGUI mLabelArrows;
    private TextMeshProUGUI mLabelBombs;

    // BuildMenu
    private GameObject mBuildMenu;

    private Button mButtonIronHarvester;
    private Button mButtonFireMaker;
    private Button mButtonForge;
    private Button mButtonBombFactory;

    // Info panel
    private GameObject mInfoPanel;
    private TextMeshProUGUI mInfoPanelText;



    private GameObject mObjectToBuildTo;
    private GameObject mBuildButtonClicked;
    private GameObject mHoverButton; // Button being visible due to hovering
    private GameObject mHoveredObject; // The object the mouse is currently hovering
    public List<GameObject> mBuildableObjects;
    private List<GameObject> mBuildButtons; // All UI buttons to click to build buildings
    private List<GameObject> mAllUIFloatingButtons; // All UI buttons to click to open build menu


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
        mBuildMenu = mCanvas.transform.Find("BuildMenu")?.gameObject;
        mButtonIronHarvester = GameObject.Find("ButtonIronHarvester")?.gameObject.GetComponent<Button>();
        mButtonFireMaker = GameObject.Find("ButtonFireMaker")?.gameObject.GetComponent<Button>();
        mButtonForge = GameObject.Find("ButtonForge")?.gameObject.GetComponent<Button>();
        mButtonBombFactory = GameObject.Find("ButtonBombFactory")?.gameObject.GetComponent<Button>();
        BuildBuildMenu();

        // Info Panel
        mInfoPanel = mCanvas.transform.Find("InfoPanel")?.gameObject;
        mInfoPanelText = GameObject.Find("InfoPanelText")?.gameObject.GetComponent<TextMeshProUGUI>();
        BuildInfoPanel();

        mBuildButtons = new List<GameObject>();
        mBuildButtons.Add( mButtonIronHarvester.gameObject );
        mBuildButtons.Add( mButtonFireMaker.gameObject );
        mBuildButtons.Add( mButtonForge.gameObject );
        mBuildButtons.Add( mButtonBombFactory.gameObject );

        mAllUIFloatingButtons = new List<GameObject>();
    }


    private void BuildBuildableList()
    {
        mBuildableObjects = new List<GameObject>();
        foreach( Transform child in GameManager.mRTSManager.mRTSWorld.transform )
        {
            if( child.name == "Tower" )
            {
                foreach( Transform buildingArea in child )
                {
                    if( buildingArea.tag != "Buildable" ) {
                        continue;
                    }

                    mBuildableObjects.Add( buildingArea.gameObject );
                }
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

        UpdateBuildMenu();
        UpdateMousePosition();
    }


    public void UpdateMousePosition()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePosWorld;
        RectTransformUtility.ScreenPointToWorldPointInRectangle( mCanvas.transform as RectTransform, mousePosScreen, Camera.main, out mousePosWorld );

        // PROD BUILDINGS
        foreach( ProductionBuilding building in GameManager.mRTSManager.mAllProductionBuildings )
        {
            if( building.gameObject.GetComponent<HarvestingBuilding>() != null ) { continue; }

            Rect bbox = Utilities.GetBBoxFromTransform( building.gameObject );
            if( bbox.Contains( mousePosWorld ) )
            {
                HoveringBuilding( building );
                return;
            }
        }

        // RECEIVERS
        List<(GameObject, int)> fullList = new List<(GameObject, int)>( GameManager.mRTSManager.mIronReceivers );
        fullList.AddRange(GameManager.mRTSManager.mFireReceivers);

        foreach ((GameObject, int) pack in fullList)
        {
            GameObject receiver = pack.Item1;
            Rect bbox = Utilities.GetBBoxFromTransform(receiver);
            if (bbox.Contains(mousePosWorld))
            {
                HoveringReceiver(receiver);
                return;
            }
        }

        // BUILDABLE
        foreach( GameObject buildable in mBuildableObjects )
        {
            Rect bbox = Utilities.GetBBoxFromTransform( buildable );
            if( bbox.Contains( mousePosWorld ) )
            {
                HoveringBuildable( buildable );
                return;
            }
        }

        // BUILDINGS IN BUILD MENU
        if( EventSystem.current.IsPointerOverGameObject() )
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(mousePosScreen.x, mousePosScreen.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if ( mBuildButtons.Contains( results[i].gameObject ) )
                {
                    HoveringBuildingDescription( results[i].gameObject );
                    return;
                }
            }
        }

        if( mHoverButton != null ) {
            DeleteUIButton( mHoverButton );
        }

        mInfoPanel.SetActive( false );
    }


    // ===================================
    // UI Stuff
    // ===================================
    public void ClearUIForSwitchingView()
    {
        DeleteAllUIFloatingButtons();
        mBuildMenu.SetActive(false);
        mInfoPanel.SetActive(false);

        if (mHoverButton != null)
        {
            DeleteUIButton(mHoverButton);
        }
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

        text.GetComponent<TextMeshProUGUI>().text = prodBuilding.mIsPaused ? "Resume" : "Pause";

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

            prodBuilding.mIsPaused = !prodBuilding.mIsPaused;
            UpdateInfoPanel();
            text.GetComponent<TextMeshProUGUI>().text = prodBuilding.mIsPaused ? "Resume" : "Pause";

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
        if (mHoveredObject == building.gameObject && mHoverButton != null)
        {
            return;
        }

        if (mHoveredObject != building.gameObject)
        {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = building.gameObject;
        CreatePauseButtonOverBuilding(building.gameObject);
        UpdateInfoPanel();
    }


    private void HoveringReceiver(GameObject receiver)
    {
        GameObject associatedReceiverObject = receiver.GetComponent<Receiver>().mAssociatedHarvester.gameObject;
        if (mHoveredObject == associatedReceiverObject && mHoverButton != null)
        {
            return;
        }

        if (mHoveredObject != associatedReceiverObject)
        {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = associatedReceiverObject;
        CreatePauseButtonOverBuilding( receiver );
        UpdateInfoPanel();
    }


    private void HoveringBuildable( GameObject resource )
    {
        if( mHoveredObject == resource && mHoverButton != null ) {
            return;
        }

        if( mHoveredObject != resource ) {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = resource;
    }


    private void HoveringBuildingDescription( GameObject button )
    {
        if( mHoveredObject == button && mHoverButton != null ) {
            return;
        }

        if( mHoveredObject != button ) {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = button;
        UpdateInfoPanel();
    }


    // ===================================
    // Build Menu
    // ===================================
    public void ShowBuildMenu()
    {
        UpdateBuildMenu();
        mBuildMenu.transform.SetAsLastSibling();
        mBuildMenu.SetActive( true );
    }


    private void UpdateBuildMenu()
    {
        if( !mBuildMenu.activeSelf ) { return; }

        bool isLocationAnIronMine = mObjectToBuildTo.gameObject.GetComponent<IronVein>() != null;
        bool isLocationAFireMine = mObjectToBuildTo.gameObject.GetComponent<FireVein>() != null;

        mButtonIronHarvester.interactable = IronHarvester.IsBuildable() && isLocationAnIronMine && GameManager.mRTSManager.CanBuildHarvester();
        mButtonFireMaker.interactable = FireMaker.IsBuildable() && isLocationAFireMine && GameManager.mRTSManager.CanBuildHarvester();

        mButtonForge.interactable = Forge.IsBuildable() && !isLocationAnIronMine && !isLocationAFireMine;
        mButtonBombFactory.interactable = BombFactory.IsBuildable() && !isLocationAnIronMine && !isLocationAFireMine;
    }


    private void BuildBuildMenu()
    {
        mBuildMenu.SetActive( false );

        mBuildMenu.transform.Find("Close")?.gameObject.GetComponent<Button>().onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
        });

        mButtonIronHarvester.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingIronHarvester", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonFireMaker.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingFireMine", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonForge.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingForge", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonBombFactory.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingBombFactory", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

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
        mInfoPanel.SetActive( false );
        if( mHoveredObject == null ) { return; }

        ProductionBuilding prodBuilding = mHoveredObject.GetComponent<ProductionBuilding>();
        if( prodBuilding != null )
        {
            mInfoPanelText.text = prodBuilding.mIsPaused ? "Paused" : prodBuilding.mResourceDescriptor.PrintProductionRates();
            mInfoPanel.SetActive( true );
        }

        if( mHoveredObject == mButtonIronHarvester.gameObject )
        {
            mInfoPanelText.text = IronHarvester.GetResourceDescriptor().PrintCompleteDescription( "Iron Harvester", "Harvests iron from iron veins" );
            mInfoPanel.SetActive( true );
        }
        else if( mHoveredObject == mButtonFireMaker.gameObject )
        {
            mInfoPanelText.text = FireMaker.GetResourceDescriptor().PrintCompleteDescription( "Fire Maker", "Harvests fire from fire veins" );
            mInfoPanel.SetActive( true );
        }
        else if( mHoveredObject == mButtonForge.gameObject )
        {
            mInfoPanelText.text = Forge.GetResourceDescriptor().PrintCompleteDescription( "Forge", "Builds arrows using iron" );
            mInfoPanel.SetActive( true );
        }
        else if( mHoveredObject == mButtonBombFactory.gameObject )
        {
            mInfoPanelText.text = BombFactory.GetResourceDescriptor().PrintCompleteDescription( "Bomb Factory", "Builds bombs using iron and fire" );
            mInfoPanel.SetActive( true );
        }
    }
}
