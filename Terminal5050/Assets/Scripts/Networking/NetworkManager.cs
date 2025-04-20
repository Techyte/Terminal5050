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

    private bool _wantToInit;
    private bool _wantToAutoCreateLobby;

    private void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (_wantToInit)
        {
            Init(_wantToAutoCreateLobby);
            _wantToInit = false;
        }
    }

    private void SubscribeToServerHooks()
    {
        Server.ClientConnected += ServerOnClientConnected;
    }

    private void SubscribeToClientHooks()
    {
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
        
        if (Server != null)
        {
            Server.Stop();
        }

        Client = null;
        Server = null;
        
        players.Clear();
        
        SceneManager.LoadScene("MainMenu");
    }

    private void ClientOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        Player.PlayerLeft(e.Id);
        players.Remove(e.Id);
    }

    public void RegisterInit(bool createLobby)
    {
        _wantToInit = true;
        _wantToAutoCreateLobby = createLobby;
    }

    private void Init(bool createLobby)
    {
        Client = new Client();
        SubscribeToClientHooks();
        
        if (createLobby)
        {
            Server = new Server();
            SubscribeToServerHooks();
            Server.Start(port, maxClientCount);

            _host = true;
        }
        
        Client.Connect($"127.0.0.1:{port}");
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
        
        SceneManager.sceneLoaded -= SceneLoaded;
    }
}
