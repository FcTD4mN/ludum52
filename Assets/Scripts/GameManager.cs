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

    // PlayerController Ref
    [HideInInspector]
    public PlayerController playerCtrler;

    // Start is called before the first frame update
    void OnEnable()
    {
        // If there is already an gamemanager instance
        if (GameManager.mInstance != null) { return; }

        mInstance = this;
        mResourceManager = GameObject.Find("ResourceManager")?.gameObject.GetComponent<ResourceManager>();
        if (mResourceManager != null)
            mResourceManager.Initialize();

        mRTSManager = GameObject.Find("RTSManager")?.gameObject.GetComponent<RTSManager>();
        if (mRTSManager != null)
            mRTSManager.Initialize();

        mUIManager = GameObject.Find("UIManager")?.gameObject.GetComponent<UIManager>();
        if (mUIManager != null)
            mUIManager.Initialize();

        GameObject player = GameObject.Find("Character")?.gameObject;
        playerCtrler = GameObject.Find("Character")?.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Faire les appels ici pour garantir l'ordre, les resources doivent être updaté avant le reste du jeu
        if (mResourceManager != null)
            mResourceManager.UpdateResources();

        // Surement en dernier, l'ui s'update
        if (mUIManager != null)
            mUIManager.UpdateUI();
    }
}
