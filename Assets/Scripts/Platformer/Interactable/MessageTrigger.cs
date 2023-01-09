using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : Interactable
{
    public bool mIsFirstTimeInteracting = true;
    public string mMessage;
    public float mDuration;

    internal override void Initialize()
    {
        base.Initialize();
        mShowButton = false;
    }

    public override void Interact()
    {
        // Nothing
    }

    public override void DisplayFirstTimeHelp()
    {
        if (mIsFirstTimeInteracting)
        {
            GameManager.mUIManager.DisplayMessage(mMessage, mDuration);
            mIsFirstTimeInteracting = false;
        }
    }
}
