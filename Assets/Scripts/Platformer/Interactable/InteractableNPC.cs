using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : Interactable
{
    public UnlockableAction unlockedAction;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isActive)
        {
            transform.position = new Vector3(transform.position.x + (0.001f * transform.localScale.x), transform.position.y, transform.position.z);
        }

    }

    public override void Interact()
    {
        // Drop content of the Chest
        if (isActive)
        {
            interactBtn.SetActive(false);

            // Unlock action (pour l'instant j'ai foutu ça dans le GM)
            GameManager.mInstance.UnlockAction(unlockedAction);

            // Show message 
            if (mIsFirstTimeInteracting)
            {
                GameManager.mUIManager.DisplayMessage(mMessage, mDuration);
                mIsFirstTimeInteracting = false;
            }

            // Set animation for leaving
            animator.SetBool("isRunning", true);
            isActive = false;
        }
    }
}