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

        GameObject ironMineA = GameObject.Find("IronVein")?.gameObject;
        GameObject ironMineB = GameObject.Find("IronVein2")?.gameObject;
        CreateBuildButtonOverObject( ironMineA );
        CreateBuildButtonOverObject( ironMineB );
    }


    // ===================================
    // Update
    // ===================================
    public void UpdateUI()
    {
        mLabelGold.text = mResourceManager.GetGold().ToString();
        mLabelIron.text = mResourceManager.GetIron().ToString();
        mLabelArrows.text = mResourceManager.GetArrows().ToString();
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

            ButtonBuildClicked(obj);
            GameObject.Destroy( newButton );
        });
    }

    public void ButtonBuildClicked( GameObject obj )
    {
        GameManager.mRTSManager.BuildObjectAtLocation( "BuildingIronHarvester", obj.transform.position );
    }
}
