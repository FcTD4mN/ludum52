using UnityEngine;

public class InteractableFireVein : Interactable
{
    public static bool mIsFirstTimeInteractingLocal = true;

    override public void Interact()
    {
        if (isActive)
        {
            ProductionBuilding fireHarvester = GameManager.mRTSManager.GetPrefabByType( RTSManager.eBuildingList.FireHarvester );
            if( !fireHarvester.IsBuildable() )
            {
                RTSManager.eBuildingErrors error = fireHarvester.GetBuildingError();
                GameManager.mUIManager.DisplayMessage( error.ToString(), 2 );
                return;
            }

            GameManager.mRTSManager.BuildObjectAtLocation( RTSManager.eBuildingList.FireHarvester.ToString(), gameObject);
            interactBtn.SetActive(false);
            isActive = false;
        }
    }

    public override void DisplayFirstTimeHelp()
    {
    }
}
