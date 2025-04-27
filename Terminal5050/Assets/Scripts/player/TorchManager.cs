using Riptide;
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
    private PlayerPauseManager _pause;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
        _pManager = GetComponent<PersonalPowerManager>();
        _player = GetComponent<Player>();
        _pause = GetComponent<PlayerPauseManager>();
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
        
        torchLight.gameObject.SetActive(_on);
        
        if (!_player.local)
        {
            return;
        }

        bool wantToToggle = false;
        
        wantToToggle = Input.GetMouseButtonDown(0) && !_pause.Paused;

        if (wantToToggle && _pManager.charge >= 0)
        {
            SendItemToggleMessage(Player.LocalPlayer.id, !_on);
        }

        if (_on)
        {
            _pManager.charge -= torchDrainPerSecond * Time.deltaTime;
        }

        if (_pManager.charge <= 0)
        {
            _on = false;
            _pManager.charge = 0;
        }
    }

    private void ChangeTorchState(bool newState)
    {
        _on = newState;
        _inventory.Click.Play();
    }

    public static void SendItemToggleMessage(ushort id, bool newState)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
            {
                player.tManager.ChangeTorchState(newState);
            }
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.ItemToggled);
            message.AddUShort(id);
            message.AddBool(newState);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.ToggleItem);
            message.AddBool(newState);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static  void ServerReceivedToggleItem(ushort id, bool newState)
    {
        SendItemToggleMessage(id, newState);
    }

    public static  void ClientReceivedItemToggled(ushort id, bool newState)
    {
        if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
        {
            player.tManager.ChangeTorchState(newState);
        }
    }
}
