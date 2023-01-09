using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePortal : Interactable
{
    public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        isActive = true;
        mShowButton = true;
    }

    public override void Interact()
    {
        if (isActive)
        {
            if (mIsFirstTimeInteracting)
            {
                // UNLOCK Portal on PortalManager
                Debug.Log("UNLOCK Portal");
                GameManager.mInstance.mPortalManager.UnlockPortal(this);
                GameManager.mInstance.mPortalManager.DisplayAvailablePortals();

                // Display message
                GameManager.mUIManager.DisplayMessage("Portal <color=\"green\">UNLOCK</color><br><size=80%>Press [F] again near a portal to travel quickly on the map.<br></size>", 3f);
                mIsFirstTimeInteracting = false;
            }
            else
            {
                // Open map to Teleport
                Debug.Log("On ouvre la map pour portals");

            }

        }
    }
}
