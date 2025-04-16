public class CameraToggleButton : Interactable
{
    public override void Interact(PersonalPowerManager pManager)
    {
        CameraManager.Instance.ToggleCameras();
    }
}
