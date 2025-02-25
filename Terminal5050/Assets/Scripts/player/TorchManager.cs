using UnityEngine;
using UnityEngine.UI;

public class TorchManager : MonoBehaviour
{
    [SerializeField] private Light torchLight;
    [SerializeField] private float drainPerSecond;
    [SerializeField] private Image chargeDisplay;

    private Inventory _inventory;

    private bool _on;

    private Torch _torch;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (_inventory.smallItems[_inventory.selectedIndex] is Torch)
        {
            _torch = (Torch)_inventory.smallItems[_inventory.selectedIndex];
        }
        else
        {
            _torch = null;
        }
        
        if (_torch == null)
            return;
        
        if (Input.GetMouseButtonDown(0) && _torch.charge >= 0)
        {
            _on = !_on;
        }
        
        torchLight.gameObject.SetActive(_on);

        if (_on)
        {
            _torch.charge -= drainPerSecond * Time.deltaTime;
        }

        if (_torch.charge <= 0)
        {
            _on = false;
            _torch.charge = 0;
        }

        chargeDisplay.transform.localScale = new Vector2(_torch.charge / _torch.maxCharge, 1);
    }
}
