using System.Collections;
using TMPro;
using UnityEngine;

public class DoorIndicator : Interactable
{
    [SerializeField] private Door sourceDoor;
    [SerializeField] private TextMeshPro text;

    private void Awake()
    {
        text.text = sourceDoor.id;
    }

    public override void Interact(PersonalPowerManager pManager)
    {
        Inventory inventory = pManager.GetComponent<Inventory>();
        if (inventory.smallItems[inventory.selectedIndex] is Scanner)
        {
            StartCoroutine(PingDoor(pManager));
        }
        else
        {
            ActionBar.Instance.NewOutput("Scanner needed");
        }
    }

    private IEnumerator PingDoor(PersonalPowerManager pManager)
    {
        ActionBar.Instance.NewOutput("Attempting to ping door back to base");
        yield return new WaitForSeconds(1);
        Inventory inventory = pManager.GetComponent<Inventory>();
        if (pManager.charge - ((Scanner)inventory.smallItems[inventory.selectedIndex]).powerUsageCost <= 0)
        {
            ActionBar.Instance.NewOutput("Insufficient power", Color.red);
            yield break;
        }
        pManager.charge -= ((Scanner)inventory.smallItems[inventory.selectedIndex]).powerUsageCost;
        CMDManager.Instance.tBehaviour.PlayerPingedDoor(sourceDoor.id);
        ActionBar.Instance.NewOutput("Ping successful");
        inventory.Beep.Play();
    }
}
