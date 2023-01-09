using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : Interactable
{
    public UnlockableAction unlockedAction;

    public override void Interact()
    {
        // Drop content of the Chest
        if (isActive)
        {
            interactBtn.SetActive(false);

            // Unlock action (pour l'instant j'ai foutu ça dans le GM)
            GameManager.mInstance.UnlockAction(unlockedAction);

            // Set animation for leaving
            isActive = false;
        }
    }
}