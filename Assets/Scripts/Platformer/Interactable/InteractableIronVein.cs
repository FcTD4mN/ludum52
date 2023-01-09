public class InteractableIronVein : Interactable
{
    public static bool mIsFirstTimeInteracting = true;

    override public void Interact()
    {
        if( isActive )
        {
            if (!IronHarvester.IsBuildable())
            {
                RTSManager.eBuildingErrors error = IronHarvester.GetBuildingError();
                GameManager.mUIManager.DisplayMessage(error.ToString(), 2);
                return;
            }

            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingIronHarvester", gameObject );
            interactBtn.SetActive( false );
            isActive = false;
        }
    }

    public override void DisplayFirstTimeHelp()
    {
        if (mIsFirstTimeInteracting)
        {
            GameManager.mUIManager.DisplayMessage("This is an iron source !\nThis will bring you iron if you build an iron harvester over it !", 5);
            mIsFirstTimeInteracting = false;
        }
    }
}
