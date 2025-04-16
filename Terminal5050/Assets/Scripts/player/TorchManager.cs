using UnityEngine;

public class TorchManager : MonoBehaviour
{
    [SerializeField] private Light torchLight;

    private Inventory _inventory;
    private PersonalPowerManager _pManager;

    private bool _on;

    private Torch _torch;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
        _pManager = GetComponent<PersonalPowerManager>();
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
        
        torchLight.gameObject.SetActive(_on);

        if (_torch == null)
        {
            _on = false;
            return;
        }

        if (Input.GetMouseButtonDown(0) && _pManager.charge >= 0)
        {
            _on = !_on;
            _inventory.Click.Play();
        }

        if (_on)
        {
            _pManager.charge -= _torch.drainPerSecond * Time.deltaTime;
        }
        
        torchLight.gameObject.SetActive(_on);

        if (_on)
        {
            _pManager.charge -= _torch.drainPerSecond * Time.deltaTime;
        }

        if (_pManager.charge <= 0)
        {
            _on = false;
            _pManager.charge = 0;
        }
    }
}
