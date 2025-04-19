using UnityEngine;

public class TorchManager : MonoBehaviour
{
    [SerializeField] private Light torchLight;
    [SerializeField] private float torchDrainPerSecond;
    public float scannerDrain;

    private Inventory _inventory;
    private PersonalPowerManager _pManager;

    private bool _on;

    private Item _torch;

    private Player _player;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
        _pManager = GetComponent<PersonalPowerManager>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_inventory.smallItems[_inventory.selectedIndex] != null)
        {
            if (_inventory.smallItems[_inventory.selectedIndex].template.name == "Torch")
            {
                _torch = _inventory.smallItems[_inventory.selectedIndex];
            }
            else
            {
                _torch = null;
            }
        }
        else
        {
            _torch = null;
        }

        if (_torch == null)
        {
            _on = false;
            torchLight.gameObject.SetActive(false);
            return;
        }

        bool wantToToggle = false;
        
        wantToToggle = Input.GetMouseButtonDown(0) && _player.local;

        if (wantToToggle && _pManager.charge >= 0)
        {
            _on = !_on;
            _inventory.Click.Play();
        }

        if (_on)
        {
            _pManager.charge -= torchDrainPerSecond * Time.deltaTime;
        }
        
        torchLight.gameObject.SetActive(_on);

        if (_pManager.charge <= 0)
        {
            _on = false;
            _pManager.charge = 0;
        }
    }
}
