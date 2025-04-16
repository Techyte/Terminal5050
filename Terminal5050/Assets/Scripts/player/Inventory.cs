using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform itemDisplayLocation;

    [SerializeField] private ItemTemplate torchTemplate;
    [SerializeField] private ItemTemplate scannerTemplate;
    [SerializeField] private CameraController controller;
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioSource click;

    public AudioSource Beep => beep;
    public AudioSource Click => click;
    
    public int selectedIndex = 0;
    public Item largeItem;
    public Item[] smallItems = new Item[5];

    private GameObject _currentItemDisplay;

    private void Awake()
    {
        Torch torch = new Torch(1);
        torch.template = torchTemplate;
        smallItems[0] = torch;
        
        Scanner scanner = new Scanner(1);
        scanner.template = scannerTemplate;
        smallItems[1] = scanner;
        
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
        _currentItemDisplay.transform.localRotation = smallItems[selectedIndex].template.model.transform.rotation;
    }
}
