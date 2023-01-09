using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBPChest : Interactable
{
    public BluePrint bluePrintChest;

    public override void Interact()
    {
        // Drop content of the Chest
        if (isActive)
        {
            interactBtn.SetActive(false);
            Vector2 launchPoint = new Vector2(transform.position.x + 0.5f, transform.position.y);

            GameObject bpPrefab = Instantiate(
                interactablePrefab,
                launchPoint,
                transform.rotation
            );

            Rigidbody2D rb = bpPrefab.GetComponent<Rigidbody2D>();
            BPCollectable bp = bpPrefab.GetComponent<BPCollectable>();
            bp.bluePrint = bluePrintChest;
            rb.velocity = new Vector2(2f, 4f);
            isActive = false;
        }
    }
}