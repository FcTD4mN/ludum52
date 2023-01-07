using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
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
        }
    }
}
