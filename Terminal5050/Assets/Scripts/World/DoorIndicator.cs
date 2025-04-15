using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorIndicator : Interactable
{
    [SerializeField] private Door sourceDoor;
    [SerializeField] private TextMeshPro text;

    private void Awake()
    {
        text.text = sourceDoor.id;
    }

    private void Update()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    public override void Interact()
    {
        CMDManager.Instance.tBehaviour.PlayerPingedDoor(sourceDoor.id);
    }
}
