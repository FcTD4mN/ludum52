using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCollectable : Collectable
{
    void OnEnable()
    {
        Initialize();
        label = "Gold";
        value = Random.Range(0f, 100f);
    }
}
