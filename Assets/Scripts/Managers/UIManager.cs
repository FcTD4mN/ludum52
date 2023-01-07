using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private ResourceManager mResourceManager;
    private Canvas mCanvas;

    private TextMeshProUGUI mLabelGold;
    private TextMeshProUGUI mLabelIron;
    private TextMeshProUGUI mLabelArrows;


    private GameObject mBuildMenu;
    private Button mButtonIronHarvester;
    private Button mButtonForge;
    private Button mButtonFireMaker;


    private GameObject mObjectToBuildTo;
    private GameObject mBuildButtonClicked;

    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mResourceManager = GameManager.mResourceManager;

        mLabelGold = GameObject.Find("LabelGold")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelIron = GameObject.Find("LabelIron")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelArrows = GameObject.Find("LabelArrows")?.gameObject.GetComponent<TextMeshProUGUI>();

        mCanvas = GameObject.Find("UI-RTS")?.gameObject.GetComponent<Canvas>();

        // Build Menu
        mBuildMenu = mCanvas.transform.Find("BuildMenu")?.gameObject;
        mButtonIronHarvester = GameObject.Find("ButtonIronHarvester")?.gameObject.GetComponent<Button>();
        mButtonForge = GameObject.Find("ButtonForge")?.gameObject.GetComponent<Button>();
        mButtonFireMaker = GameObject.Find("ButtonFireMaker")?.gameObject.GetComponent<Button>();
        BuildBuildMenu();

        CreateBuildButtonOnEveryBuildableObject();
    }


    // ===================================
    // Update
    // ===================================
    public void UpdateUI()
    {
        mLabelGold.text = mResourceManager.GetGold().ToString();
        mLabelIron.text = mResourceManager.GetIron().ToString();
        mLabelArrows.text = mResourceManager.GetArrows().ToString();

        UpdateBuildMenu();
    }

    // ===================================
    // UI Stuff
    // ===================================


    public void CreateBuildButtonOnEveryBuildableObject()
    {
        foreach( Transform child in GameManager.mRTSManager.mRTSWorld.transform )
        {
            if( child.name == "Tower" )
            {
                foreach( Transform buildingArea in child )
                {
                    if( buildingArea.tag != "Buildable" ) {
                        continue;
                    }

                    CreateBuildButtonOverObject( buildingArea.gameObject );
                }
            }

            if( child.tag != "Buildable" ) {
                continue;
            }

            CreateBuildButtonOverObject( child.gameObject );
        }
    }


    public void CreateBuildButtonOverObject( GameObject obj )
    {
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/RTS/UI/ButtonBuild");

        Rect screenOjbBBox = Utilities.WorldToScreenRect( Utilities.GetBBoxFromTransform(obj) );
        GameObject newButton = Instantiate( buttonPrefab,
                                            new Vector3( screenOjbBBox.center.x, screenOjbBBox.center.y, 0 ),
                                            Quaternion.Euler(0, 0, 0) );
        newButton.transform.SetParent( mCanvas.transform );
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2( screenOjbBBox.width, screenOjbBBox.height );

        newButton.GetComponent<Button>().onClick.AddListener( () => {

            mObjectToBuildTo = obj;
            mBuildButtonClicked = newButton;
            ShowBuildMenu();

        });
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

        mButtonIronHarvester.interactable = GameManager.mResourceManager.mGoldF >= IronHarvester.mGoldCost && isLocationAnIronMine;
        mButtonForge.interactable = GameManager.mResourceManager.mGoldF >= Forge.mGoldCost && !isLocationAnIronMine;
        mButtonFireMaker.interactable = !isLocationAnIronMine;
    }

    private void BuildBuildMenu()
    {
        mBuildMenu.SetActive( false );

        mBuildMenu.transform.Find("Close")?.gameObject.GetComponent<Button>().onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
        });

        TextMeshProUGUI labelIron = mButtonIronHarvester.gameObject.transform.Find("text")?.GetComponent<TextMeshProUGUI>();
        labelIron.text = "Iron Harvester" +
                        "\n" +
                        "\nCost: " + IronHarvester.mGoldCost.ToString() +
                        "\nProduction: " + IronHarvester.mRatePerSecond.ToString();

        mButtonIronHarvester.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingIronHarvester", mObjectToBuildTo.transform.position );
            GameObject.Destroy( mBuildButtonClicked );
        });

        TextMeshProUGUI labelForge = mButtonForge.gameObject.transform.Find("text")?.GetComponent<TextMeshProUGUI>();
        labelForge.text = "Forge" +
                        "\n" +
                        "\nCost: " + Forge.mGoldCost.ToString() +
                        "\nProduction: " + Forge.mRatePerSecond.ToString();

        mButtonForge.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingForge", mObjectToBuildTo.transform.position );
            GameObject.Destroy( mBuildButtonClicked );
            GameObject.Destroy( mObjectToBuildTo );
        });

    }
}
