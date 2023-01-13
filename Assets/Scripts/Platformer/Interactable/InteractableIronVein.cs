public class InteractableIronVein : Interactable
{
    public static bool mIsFirstTimeInteractingLocal = true;

    override public void Interact()
    {
        if( isActive )
        {
            ProductionBuilding ironHarvester = GameManager.mRTSManager.GetPrefabByType(RTSManager.eBuildingList.FireHarvester);
            if (!ironHarvester.IsBuildable())
            {
                RTSManager.eBuildingErrors error = ironHarvester.GetBuildingError();
                GameManager.mUIManager.DisplayMessage(error.ToString(), 2);
                return;
            }

            GameManager.mRTSManager.BuildObjectAtLocation( RTSManager.eBuildingList.IronHarvester.ToString(), gameObject );
            interactBtn.SetActive( false );
            isActive = false;
        }
    }

    public override void DisplayFirstTimeHelp()
    {
        if (mIsFirstTimeInteractingLocal)
        {
            GameManager.mUIManager.DisplayMessage("This is an iron source !\nThis will bring you iron if you build an iron harvester over it !", 5);
            mIsFirstTimeInteractingLocal = false;
        }
    }
}
