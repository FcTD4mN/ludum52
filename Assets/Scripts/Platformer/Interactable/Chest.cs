using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public static bool mIsFirstTimeInteracting = true;
    public override void Interact()
    {
        // Drop a gold prefab
        if (isActive)
        {
            interactBtn.SetActive(false);
            Vector2 launchPoint = new Vector2(transform.position.x + 0.5f, transform.position.y);
            chestContent = Instantiate(interactablePrefab, launchPoint, transform.rotation);
            Rigidbody2D rb = chestContent.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(3f, 1f);
            isActive = false;
        }
    }

    public override void DisplayFirstTimeHelp()
    {
        if (mIsFirstTimeInteracting)
        {
            GameManager.mUIManager.DisplayMessage("This is a chest, it can contain gold !", 3);
            mIsFirstTimeInteracting = false;
        }
    }
}
