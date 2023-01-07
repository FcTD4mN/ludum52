using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
        // Singleton
    public static GameManager mInstance;
    public static ResourceManager mResourceManager;
    public static UIManager mUIManager;
    public static RTSManager mRTSManager;


    // Start is called before the first frame update

    void OnEnable()
    {
        // If there is already an gamemanager instance
        if (GameManager.mInstance != null) { return; }

        mInstance = this;
        mResourceManager = GameObject.Find("ResourceManager")?.gameObject.GetComponent<ResourceManager>();
        mResourceManager.Initialize();

        mRTSManager = GameObject.Find("RTSManager")?.gameObject.GetComponent<RTSManager>();
        mRTSManager.Initialize();

        mUIManager = GameObject.Find("UIManager")?.gameObject.GetComponent<UIManager>();
        mUIManager.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        // Faire les appels ici pour garantir l'ordre, les resources doivent être updaté avant le reste du jeu
        mResourceManager.UpdateResources();


        // Surement en dernier, l'ui s'update
        mUIManager.UpdateUI();
    }
}
