using Riptide;
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
    [SerializeField] private Item[] defaultItems;

    public AudioSource Beep => beep;
    public AudioSource Click => click;
    
    public int selectedIndex = 0;
    [HideInInspector] public Item largeItem;
    public Item[] smallItems;

    private GameObject _currentItemDisplay;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();

        smallItems = new Item[smallCapacity];
        largeItem = null;

        for (int i = 0; i < defaultItems.Length; i++)
        {
            smallItems[i] = defaultItems[i];
        }
        
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
            SendSwapItemMessage(Player.LocalPlayer.id, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SendSwapItemMessage(Player.LocalPlayer.id, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SendSwapItemMessage(Player.LocalPlayer.id, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SendSwapItemMessage(Player.LocalPlayer.id, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SendSwapItemMessage(Player.LocalPlayer.id, 4);
        }

        bool wantToDrop = false;
        bool dropLarge = false;

        wantToDrop = Input.GetKeyDown(KeyCode.Q) && _player.local;
        dropLarge = Input.GetKey(KeyCode.LeftControl) && _player.local;

        if (wantToDrop && !dropLarge && smallItems[selectedIndex] != null)
        {
            SendDropItemMessage(Player.LocalPlayer.id, false, selectedIndex);
        }

        if (wantToDrop && dropLarge && largeItem != null)
        {
            SendDropItemMessage(Player.LocalPlayer.id, true, selectedIndex);
        }
    }

    private void ChangeItem(int index)
    {
        selectedIndex = index;
        UpdateSelected();
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

    private string ThrowItem(bool big, int index, string id = "")
    {
        Vector3 pos = itemThrowSpawnLocation.position;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 1.255f))
        {
            pos = hit.point;
        }
        
        WorldItem worldItem = Instantiate(worldItemPreset, pos, Quaternion.identity);
        worldItem.id = id;
        
        if (big)
        {
            worldItem.Init(largeItem);
        }
        else
        {
            worldItem.Init(smallItems[index]);
        }
        smallItems[index] = null;
        
        worldItem.GetComponent<Rigidbody>().AddForce(itemThrowSpawnLocation.forward * throwForce);
        
        UpdateSelected();

        return worldItem.id;
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

    public bool TryGainItem(WorldItem worldItem)
    {
        bool gained = false;

        if (!CanGainItem(worldItem.Item))
        {
            return false;
        }
        
        switch (worldItem.Item.template.type)
        {
            case Type.Large:
                largeItem = worldItem.Item;
                break;
            case Type.Small:
                for (int i = 0; i < smallItems.Length; i++)
                {
                    if (smallItems[i] == null)
                    {
                        smallItems[i] = worldItem.Item;
                        break;
                    }
                }
                break;
        }
        
        worldItem.PickedUp();
        
        UpdateSelected();

        return true;
    }

    private bool CanGainItem(Item item)
    {
        bool canGain = false;
        
        switch (item.template.type)
        {
            case Type.Large:
                if (largeItem == null)
                {
                    canGain = true;
                }
                break;
            case Type.Small:
                int remainingSpace = CanGainSmallItem();
                if (remainingSpace != 0)
                {
                    canGain = true;
                }
                break;
        }

        return canGain;
    }

    public void ItemWantToBePickedUp(WorldItem worldItem)
    {
        if (CanGainItem(worldItem.Item))
        {
            SendGainItemMessage(Player.LocalPlayer.id, worldItem);
        }
    }

    #region GainItem
    
    private static void SendGainItemMessage(ushort id, WorldItem worldItem)
    {
        if (NetworkManager.Instance.Server != null)
        {
            if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
            {
                Inventory inventory = player.inventory;
                
                if (inventory.TryGainItem(worldItem))
                {
                    if (player.local)
                        ActionBar.Instance.NewOutput($"+1 {worldItem.Item.template.name}");
                }
                else
                {
                    if (player.local)
                        ActionBar.Instance.NewOutput("Not enough space to pick up item", Color.red);
                }
            }
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.ItemPickedUp);
            message.AddUShort(id);
            message.AddString(worldItem.id);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.PickUpItem);
            message.AddString(worldItem.id);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static void ServerGainItem(ushort client, string id)
    {
        SendGainItemMessage(client, WorldItem.GetWorldItemFromId(id));
    }

    public static void ClientGainItem(ushort client, string worldItemId)
    {
        Debug.Log($"received player gained item message");
        if (NetworkManager.Instance.players.TryGetValue(client, out Player player))
        {
            Debug.Log($"Player {client} gained item of id {worldItemId}");
            
            Inventory inventory = player.inventory;
            WorldItem worldItem = WorldItem.GetWorldItemFromId(worldItemId);
            
            if (inventory.TryGainItem(worldItem))
            {
                if (player.local)
                    ActionBar.Instance.NewOutput($"+1 {worldItem.Item.template.name}");
            }
            else
            {
                if (player.local)
                    ActionBar.Instance.NewOutput("Not enough space to pick up item", Color.red);
            }
        }
    }

    #endregion

    #region DropItem
    
    private static void SendDropItemMessage(ushort id, bool big, int index)
    {
        if (NetworkManager.Instance.Server != null)
        {
            string itemId = "";
            
            if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
            {
                itemId = player.inventory.ThrowItem(big, index);
            }
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.ItemDropped);
            message.AddUShort(id);
            message.AddBool(big);
            message.AddInt(index);
            message.AddString(itemId);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.DropItem);
            message.AddBool(big);
            message.AddInt(index);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static void ServerDropItem(ushort client, bool big, int index)
    {
        SendDropItemMessage(client, big, index);
    }

    public static void ClientDropItem(ushort client, bool big, int index, string id)
    {
        if (NetworkManager.Instance.players.TryGetValue(client, out Player player))
        {
            player.inventory.ThrowItem(big, index, id);
        }
    }

    #endregion

    #region SwapItem

    private static void SendSwapItemMessage(ushort id, int index)
    {
        if (NetworkManager.Instance.Server != null)
        {
            if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
            {
                player.inventory.ChangeItem(index);
            }
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.ItemSwapped);
            message.AddUShort(id);
            message.AddInt(index);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.SwapItem);
            message.AddInt(index);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static void ServerSwapItem(ushort client, int index)
    {
        SendSwapItemMessage(client, index);
    }

    public static void ClientSwapItem(ushort client, int index)
    {
        if (NetworkManager.Instance.players.TryGetValue(client, out Player player))
        {
            player.inventory.ChangeItem(index);
        }
    }

    #endregion
}
