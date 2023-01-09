public class InteractableFireVein : Interactable
{
    override public void Interact()
    {
        if (isActive && FireMaker.IsBuildable())
        {
            GameManager.mRTSManager.BuildObjectAtLocation("BuildingFireMine", gameObject);
            interactBtn.SetActive(false);
            isActive = false;
        }
    }
}
