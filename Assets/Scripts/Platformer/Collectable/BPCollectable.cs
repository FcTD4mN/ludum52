using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPCollectable : Collectable
{
    public BluePrint bluePrint;
    private bool mIsFirstTimeInteracting;

    void OnEnable()
    {
        Initialize();
        mIsFirstTimeInteracting = true;
        label = "BluePrint";
    }

    public override void Collect()
    {
        base.Collect();
        // Show message 
        if (mIsFirstTimeInteracting)
        {
            GameManager.mUIManager.DisplayMessage("<size=100%><color=#49b6ba>Blueprint UNLOCKED</color></size><br><size=80%><color=#BA4D49>" + bluePrint.ToString() + "</color> building unlocked !<br>Check your base [tab]</size>", 5f);
            mIsFirstTimeInteracting = false;
            // todo: actually unlock !!!
        }
    }
}
public enum BluePrint { Damage, Jump, Speed, BombFactory };