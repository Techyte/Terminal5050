using System;
using System.Collections.Generic;
using Riptide;
using UnityEngine;
using UnityEngine.SceneManagement;

enum ClientToServerMessageId : ushort
{
    BasicInfo,
    PosRot,
    PersonalPower,
    ChangedCamera,
    ToggleCamera,
    ActivateCamera,
    PickUpItem,
    DropItem,
    SwapItem,
    ToggleItem,
    PingDoor,
    ToggleDoor,
    StartSpeakers,
    StopSpeakers,
    ChargePoint,
}

enum ServerToClientMessageId : ushort
{
    NewPlayerJoined = 100,
    CurrentPlayerInfo,
    PosRotBlast,
    PowerBlast,
    PowerOverloaded,
    CameraChanged,
    CameraToggled,
    CameraActivated,
    CameraRot,
    ItemPickedUp,
    ItemDropped,
    ItemSwapped,
    ItemToggled,
    DoorPinged,
    DoorToggled,
    SpeakersStarted,
    SpeakersStopped,
    ChargePoint,
}

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    
    [SerializeField] private bool autoCreateLobby;
    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount = 4;
    [SerializeField] private string selfUsername = "PayishVibes";

    private bool _host;
    
    private Dictionary<ushort, Player> _clientPlayers = new Dictionary<ushort, Player>();

    public Dictionary<ushort, Player> players => _clientPlayers;

    public Server Server;
    public Client Client;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init(autoCreateLobby);
    }

    private void SubscribeToHooks()
    {
        if (Server != null)
        {
            Server.ClientConnected += ServerOnClientConnected;
        }
        
        Client.Connected += ClientOnConnected;
        Client.ClientConnected += ClientOnClientConnected;
        Client.Disconnected += ClientOnDisconnected;
        
        Client.ClientDisconnected += ClientOnClientDisconnected;
    }

    private void UnsubscribeFromHooks()
    {
        if (Server != null)
        {
            Server.ClientConnected -= ServerOnClientConnected;
        }
        
        Client.Connected -= ClientOnConnected;
        Client.ClientConnected -= ClientOnClientConnected;
        Client.Disconnected -= ClientOnDisconnected;
        
        Client.ClientDisconnected -= ClientOnClientDisconnected;
    }

    private void ServerOnClientConnected(object sender, ServerConnectedEventArgs e)
    {
        Debug.Log("Server registered a client connection");
    }

    private void ClientOnConnected(object sender, EventArgs e)
    {
        Debug.Log("Client registered its own connection");
        
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.BasicInfo);
        message.AddString(selfUsername);

        Client.Send(message);
    }
    
    private void ClientOnClientConnected(object sender, ClientConnectedEventArgs e)
    {
        
    }

    private void ClientOnDisconnected(object sender, DisconnectedEventArgs e)
    {
        UnsubscribeFromHooks();
        
        Client = null;
        Server = null;
        
        SceneManager.LoadScene("MainMenu");
    }

    private void ClientOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        Player.PlayerLeft(e.Id);
        players.Remove(e.Id);
    }

    public void Init(bool createLobby)
    {
        Client = new Client();
        
        if (createLobby)
        {
            Server = new Server();
            Server.Start(port, maxClientCount);

            _host = true;
            
            Client.Connect($"127.0.0.1:{port}");
        }
        else
        {
            Client.Connect($"127.0.0.1:{port}");
        }
        
        SubscribeToHooks();
    }
    
    public void ServerReceivedClientBasicInfo(ushort client, string username)
    {
        // Spawn a player for our own use
        
        bool local = client == Client.Id;
        
        Debug.Log($"Spawning new player {client}");
        Player newPlayer = Player.SpawnNewPlayer(username, client, local);
        
        // notify new joined player of existing players

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.CurrentPlayerInfo);
        message.AddInt(_clientPlayers.Count);
        foreach (var player in _clientPlayers)
        {
            message.AddUShort(player.Key);
            message.AddString(player.Value.username);
        }
        
        Server.Send(message, client);
        
        _clientPlayers.Add(client, newPlayer);
        
        // notify all existing players of the new joined player
        
        message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.NewPlayerJoined);
        message.AddUShort(client);
        message.AddString(username);

        Server.SendToAll(message, Client.Id);
        
        ActionBar.Instance.NewOutput($"{username} joined the game");
    }

    public void ClientNewPlayerJoined(ushort client, string username)
    {
        Debug.Log($"Spawning new player {client}");
        Player newPlayer = Player.SpawnNewPlayer(username, client, client == Client.Id);
        
        _clientPlayers.Add(client, newPlayer);
        
        ActionBar.Instance.NewOutput($"{username} joined the game");
    }

    public void CurrentPlayerInfo(Message message)
    {
        int length = message.GetInt();

        if (length != 0)
        {
            for (int i = 0; i < length; i++)
            {
                ushort id = message.GetUShort();
                Debug.Log($"Spawning new player {id}");
                Player newPlayer = Player.SpawnNewPlayer(message.GetString(), id, id == Client.Id);
                
                _clientPlayers.Add(id, newPlayer);
            }
        }
    }

    private void FixedUpdate()
    {
        if (Server != null)
        {
            Server.Update();
        }
        
        if (Client != null)
        {
            Client.Update();
        }
    }

    private void OnDisable()
    {
        if (Server != null)
        {
            Server.Stop();
        }

        if (Client != null)
        {
            Client.Disconnect();
        }
    }
}
