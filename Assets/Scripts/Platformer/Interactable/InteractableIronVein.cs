public class InteractableIronVein : Interactable
{
    override public void Interact()
    {
        if( isActive && IronHarvester.IsBuildable() )
        {
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingIronHarvester", gameObject );
            interactBtn.SetActive( false );
            isActive = false;
        }
    }
}
