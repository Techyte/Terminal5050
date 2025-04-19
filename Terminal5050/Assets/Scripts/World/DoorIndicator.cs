using System.Collections;
using TMPro;
using UnityEngine;

public class DoorIndicator : Interactable
{
    [SerializeField] private Door sourceDoor;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float powerCost;

    private void Awake()
    {
        text.text = sourceDoor.id;
    }

    public override void Interact(PersonalPowerManager pManager)
    {
        Inventory inventory = pManager.GetComponent<Inventory>();
        if (inventory.smallItems[inventory.selectedIndex].template.name == "Scanner")
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
        bool local = pManager.GetComponent<Player>().local;

        if (local)
            ActionBar.Instance.NewOutput("Attempting to ping door back to base");
        
        Inventory inventory = pManager.GetComponent<Inventory>();
        TorchManager tManager = pManager.GetComponent<TorchManager>();
        
        yield return new WaitForSeconds(1);
        
        if (pManager.charge - tManager.scannerDrain <= 0)
        {
            if (local)
                ActionBar.Instance.NewOutput("Insufficient power", Color.red);
            yield break;
        }
        
        pManager.charge -= tManager.scannerDrain;
        
        CMDManager.Instance.tBehaviour.PlayerPingedDoor(sourceDoor.id);
        if (local)
            ActionBar.Instance.NewOutput("Ping successful");
        
        inventory.Beep.Play();
    }
}
