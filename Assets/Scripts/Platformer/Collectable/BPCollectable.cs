using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPCollectable : Collectable
{
    public BluePrint bluePrint;

    void OnEnable()
    {
        Initialize();
        label = "BluePrint";
    }

    public override void Collect()
    {
        base.Collect();
        Debug.Log("Unlock this blueprint : " + bluePrint);
        //GameManager.mResourceManager.AddResource(cResourceDescriptor.eResourceNames.Gold.ToString(), value, true);
    }
}
public enum BluePrint { Damage, Jump, Speed };