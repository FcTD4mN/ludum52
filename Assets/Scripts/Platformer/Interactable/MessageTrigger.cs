using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : Interactable
{
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
