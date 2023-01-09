public class InteractableIronVein : Interactable
{
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
}
