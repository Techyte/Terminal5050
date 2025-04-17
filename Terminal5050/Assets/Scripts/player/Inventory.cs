using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform itemDisplayLocation;
    [SerializeField] private Transform itemThrowSpawnLocation;
    [SerializeField] private Transform cam;
    [SerializeField] private WorldItem worldItemPreset;
    [SerializeField] private float throwForce = 1;

    [SerializeField] private ItemTemplate torchTemplate;
    [SerializeField] private ItemTemplate scannerTemplate;
    [SerializeField] private CameraController controller;
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioSource click;
    [SerializeField] private int smallCapacity = 5;

    public AudioSource Beep => beep;
    public AudioSource Click => click;
    
    public int selectedIndex = 0;
    public Item largeItem;
    public Item[] smallItems = new Item[5];

    private GameObject _currentItemDisplay;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        
        smallItems = new Item[smallCapacity];

        smallItems[0] = new Torch(1, torchTemplate);
        smallItems[1] = new Scanner(5, scannerTemplate);
        
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

        bool wantToDrop = false;
        bool dropLarge = false;

        wantToDrop = Input.GetKeyDown(KeyCode.Q) && _player.local;
        dropLarge = Input.GetKeyDown(KeyCode.LeftControl) && _player.local;

        if (wantToDrop && !dropLarge && smallItems[selectedIndex] != null)
        {
            ThrowItem(false);
        }

        if (wantToDrop && dropLarge && largeItem != null)
        {
            ThrowItem(true);
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

    private void ThrowItem(bool big)
    {
        Vector3 pos = itemThrowSpawnLocation.position;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 1.255f))
        {
            pos = hit.point;
        }
        
        WorldItem worldItem = Instantiate(worldItemPreset, pos, Quaternion.identity);
        if (big)
        {
            worldItem.Init(largeItem);
        }
        else
        {
            worldItem.Init(smallItems[selectedIndex]);
        }
        smallItems[selectedIndex] = null;
        
        worldItem.GetComponent<Rigidbody>().AddForce(itemThrowSpawnLocation.forward * throwForce);
        
        UpdateSelected();
    }

    public int CanGainSmallItem()
    {
        int capacity = smallCapacity;
        
        for (int i = 0; i < smallItems.Length; i++)
        {
            if (smallItems[i] != null)
            {
                capacity--;
            }
        }

        return capacity;
    }

    public bool TryGainItem(Item item)
    {
        bool gained = false;
        
        switch (item.template.type)
        {
            case Type.Large:
                if (largeItem == null)
                {
                    largeItem = item;
                    gained = true;
                }
                break;
            case Type.Small:
                int remainingSpace = CanGainSmallItem();
                if (remainingSpace != 0)
                {
                    for (int i = 0; i < smallItems.Length; i++)
                    {
                        if (smallItems[i] == null)
                        {
                            smallItems[i] = item;
                            gained = true;
                            break;
                        }
                    }
                }
                break;
        }
        
        UpdateSelected();

        return gained;
    }
}
