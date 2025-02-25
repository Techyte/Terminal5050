using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform itemDisplayLocation;

    [SerializeField] private ItemTemplate torchTemplate;
    [SerializeField] private CameraController controller;
    
    public int selectedIndex = 0;
    public Item largeItem;
    public Item[] smallItems = new Item[5];

    private GameObject _currentItemDisplay;

    private void Awake()
    {
        Torch torch = new Torch(50, 50);
        torch.template = torchTemplate;
        smallItems[0] = torch;
        UpdateSelected();
    }

    private void Update()
    {
        if (controller.cancel)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedIndex = 0;
            UpdateSelected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedIndex = 1;
            UpdateSelected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedIndex = 2;
            UpdateSelected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedIndex = 3;
            UpdateSelected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedIndex = 4;
            UpdateSelected();
        }
    }

    private void UpdateSelected()
    {
        if (_currentItemDisplay)
        {
            Destroy(_currentItemDisplay);
        }

        if (smallItems[selectedIndex] == null)
            return;

        _currentItemDisplay = Instantiate(smallItems[selectedIndex].template.model, itemDisplayLocation.position,
            Quaternion.identity, itemDisplayLocation);
        _currentItemDisplay.transform.localRotation = Quaternion.identity;
    }
}
