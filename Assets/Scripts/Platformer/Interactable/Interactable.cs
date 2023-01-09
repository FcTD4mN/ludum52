using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public bool isActive;
    protected GameObject interactBtn;
    public GameObject interactablePrefab;
    internal bool mShowButton = true;

    // Message part
    public bool mIsFirstTimeInteracting = true;
    public string mMessage;
    public float mDuration;

    void OnEnable()
    {
        Initialize();
    }

    virtual internal void Initialize()
    {
        isActive = true;
        interactBtn = transform.Find("InteractBtn").gameObject;
        interactBtn.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "Character" && isActive)
        {
            // Display 'E' keyboard + action text
            interactBtn.SetActive(mShowButton);
            GameManager.mInstance.playerCtrler.currentInteractable = this;
            DisplayFirstTimeHelp();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Hide interaction
        interactBtn.SetActive(false);
    }

    public abstract void Interact();

    public virtual void DisplayFirstTimeHelp()
    {
    }
}
