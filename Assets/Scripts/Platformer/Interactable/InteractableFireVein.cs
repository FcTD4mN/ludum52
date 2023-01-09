using UnityEngine;

public class InteractableFireVein : Interactable
{
    override public void Interact()
    {
        if (isActive)
        {
            if( !FireMaker.IsBuildable() )
            {
                RTSManager.eBuildingErrors error = FireMaker.GetBuildingError();
                GameManager.mUIManager.DisplayMessage( error.ToString(), 2 );
                return;
            }

            GameManager.mRTSManager.BuildObjectAtLocation("BuildingFireMine", gameObject);
            interactBtn.SetActive(false);
            isActive = false;
        }
    }
}
