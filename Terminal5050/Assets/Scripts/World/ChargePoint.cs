using System;
using Riptide;
using UnityEngine;

public class ChargePoint : Interactable
{
    private static ChargePoint Instance;
    
    [SerializeField] private Transform battery;
    [SerializeField] private Transform camViewLoc;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float zoomTime = 2;
    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource humming;
    [SerializeField] private AudioSource error;

    private bool _interacting = false;

    private Player _currentPlayer;

    private PersonalPowerManager _pManager;
    private PlayerMovement _movement;
    private CameraController _cam;

    private void Awake()
    {
        Instance = this;
        humming.volume = 0;
    }

    public override void Interact(Player player)
    {
        if (player.local)
        {
            SendChargePointMessage(player.id);
        }
    }

    private void Interacted(Player player)
    {
        if (_currentPlayer != null && _currentPlayer != player)
        {
            if (player.local)
            {
                ActionBar.NewOutput("Charge Point Occupied");
                error.Play();
            }
            return;
        }
        
        PersonalPowerManager pManager = player.powerManager;
        
        if (_interacting)
        {
            GoBack();
            initTime = DateTime.Now;
        }
        else
        {
            if (Mathf.Approximately(pManager.charge, pManager.maxCharge))
            {
                if (player.local)
                {
                    ActionBar.NewOutput("Power Pack Full");
                    error.Play();
                }
                return;
            }
            
            _cam = pManager.transform.parent.GetComponentInChildren<CameraController>();
            _movement = player.movement;
            battery = player.powerManager.batteryLocation.GetChild(0);
            _originScale = battery.localScale;
            
            PowerManager.Instance.NewDrain("Charging", chargeSpeed);
            _interacting = true;
            initTime = DateTime.Now;
            
            _currentPlayer = player;
            _pManager = pManager;
            _clicked = false;
        }
    }

    private void Update()
    {
        if (_interacting)
        {
            Slotted();
            
            float amountToGain = chargeSpeed * Time.deltaTime;

            bool local = _currentPlayer.local;
            
            if (_pManager.charge + amountToGain >= _pManager.maxCharge)
            {
                if (_currentPlayer.local)
                {
                    _pManager.charge = _pManager.maxCharge;
                }

                if (NetworkManager.Instance.Server != null)
                {
                    PowerManager.Instance.ChangeCharge(-(_pManager.maxCharge - _pManager.charge));
                }
                
                GoBack();
                if (local)
                    ActionBar.NewOutput("Finished Charging");
            }
            else if (PowerManager.Instance.CurrentCharge - amountToGain <= 0)
            {
                GoBack();
                if (local)
                    ActionBar.NewOutput("Ran out of power", Color.red);
            }
            else
            {
                if (_currentPlayer.local)
                {
                    _pManager.charge += amountToGain;
                }
                
                if (NetworkManager.Instance.Server != null)
                {
                    PowerManager.Instance.ChangeCharge(-amountToGain);
                }
            }
        }
    }
    
    private DateTime initTime;
    private bool _clicked;
    private Vector3 _originScale;

    private void Slotted()
    {
        _movement.cancel = true;
        _cam.cancel = true;
        
        battery.transform.position = Vector3.Lerp(_pManager.batteryLocation.position, camViewLoc.position,
            ((float)(DateTime.Now - initTime).TotalSeconds / zoomTime));
        battery.transform.rotation = Quaternion.Lerp(_pManager.batteryLocation.rotation, camViewLoc.rotation,
            (float)(DateTime.Now - initTime).TotalSeconds / zoomTime);
        battery.transform.localScale = Vector3.Lerp(_originScale, camViewLoc.localScale,
            (float)(DateTime.Now - initTime).TotalSeconds / zoomTime);

        if ((DateTime.Now - initTime).TotalSeconds >= zoomTime && !_clicked)
        {
            click.Play();
            humming.volume = 1;
            _clicked = true;
        }
    }
    
    private void Return()
    {
        battery.transform.position = _pManager.batteryLocation.position;
        battery.transform.rotation = _pManager.batteryLocation.rotation;
        battery.transform.localScale = _originScale;
        click.Play();
    }

    private void GoBack()
    {
        Return();
        _interacting = false;
        _movement.cancel = false;
        _cam.cancel = false;
        humming.volume = 0;
        PowerManager.Instance.RemoveDrain("Charging");
        _pManager = null;
        _currentPlayer = null;
    }

    public static void SendChargePointMessage(ushort id)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
            {
                Instance.Interacted(player);
            }
            
            Debug.Log("Server sending charge point message");
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.ChargePoint);
            message.AddUShort(id);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Debug.Log("Client sending charge point message");
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.ChargePoint);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static void ServerReceivedChargePoint(ushort id)
    {
        Debug.Log("Server received charge point message");
        
        SendChargePointMessage(id);
    }

    public static void ClientReceivedChargePoint(ushort id)
    {
        Debug.Log("Client received charge point message");
        
        if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
        {
            Instance.Interacted(player);
        }
    }
}