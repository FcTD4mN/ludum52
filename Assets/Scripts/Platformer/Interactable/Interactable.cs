using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public bool isActive;
    protected GameObject interactBtn;
    protected GameObject chestContent;
    public GameObject interactablePrefab;

    void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
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
            interactBtn.SetActive(true);
            GameManager.mInstance.playerCtrler.currentInteractable = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Hide interaction
        interactBtn.SetActive(false);
    }

    public abstract void Interact();
}
