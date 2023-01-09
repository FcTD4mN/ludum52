using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGoldChest : Interactable
{
    [HideInInspector]
    public enum LootType { Small, Medium, Huge };

    public LootType lootType;
    public int minSmallLoot = 10;
    public int maxSmallLoot = 100;
    public int minMediumLoot = 100;
    public int maxMediumLoot = 400;
    public int limit = 1000;
    private List<int> loot = new List<int>();

    void Awake()
    {
        int cMin = 0, cMax = 0, nbGold = 0;

        if (lootType == LootType.Small)
        {
            nbGold = Random.Range(3, 7);
            cMin = minSmallLoot;
            cMax = maxSmallLoot;
        }
        else if (lootType == LootType.Medium)
        {
            nbGold = Random.Range(2, 3);
            cMin = minMediumLoot;
            cMax = maxMediumLoot;
        }
        else if (lootType == LootType.Huge)
        {
            nbGold = 1;
            cMin = maxMediumLoot;
            cMax = limit;
        }

        for (int i = 0; i < nbGold; i++)
        {
            loot.Add(Random.Range(minSmallLoot, maxSmallLoot));
        }
    }

    public override void Interact()
    {
        // Drop content of the Chest
        if (isActive)
        {
            interactBtn.SetActive(false);
            Vector2 launchPoint = new Vector2(transform.position.x + 0.1f, transform.position.y);
            float spacingRatio = 0f;

            for (int i = 0; i < loot.Count; i++)
            {
                GameObject gold = Instantiate(
                    interactablePrefab,
                    new Vector2(launchPoint.x + spacingRatio, launchPoint.y + spacingRatio),
                    transform.rotation
                );

                Rigidbody2D rb = gold.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(3f, 1f);
                spacingRatio += 0.3f;
            }

            isActive = false;
        }
    }
}
