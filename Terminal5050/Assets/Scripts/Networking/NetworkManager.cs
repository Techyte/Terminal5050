using System;
using System.Collections.Generic;
using Riptide;
using Riptide.Transports.Steam;
using Riptide.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SteamClient = Riptide.Transports.Steam.SteamClient;

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
    DepositItem,
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
    ItemDeposited,
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

    public bool usingSteam;

    private CSteamID _currentLobbyId;

    private bool _wantToInit;
    private bool _wantToAutoCreateLobby;
    private bool _wantToUseSteam;

    private void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += SceneLoaded;
        
#if UNITY_EDITOR
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
#else
            RiptideLogger.Initialize(Debug.Log, true);
#endif
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (_wantToInit)
        {
            Init(_wantToAutoCreateLobby, _wantToUseSteam);
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
        Client.ConnectionFailed += ClientOnConnectionFailed;
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
        Client.ConnectionFailed -= ClientOnConnectionFailed;
    }

    private void ServerOnClientConnected(object sender, ServerConnectedEventArgs e)
    {
        Debug.Log("Server registered a client connection");
    }

    private void ClientOnConnected(object sender, EventArgs e)
    {
        LoadingScreen.Loaded();
        
        Debug.Log("Client registered its own connection");
        
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.BasicInfo);
        if (usingSteam)
        {
            message.AddString(SteamFriends.GetPersonaName());
        }
        else
        {
            message.AddString(selfUsername);
        }

        Client.Send(message);
    }
    
    private void ClientOnClientConnected(object sender, ClientConnectedEventArgs e)
    {
        
    }

    private void ClientOnConnectionFailed(object sender, ConnectionFailedEventArgs e)
    {
        ConnectionTerminated();
    }

    private void ClientOnDisconnected(object sender, DisconnectedEventArgs e)
    {
        ConnectionTerminated();
    }

    private void ConnectionTerminated()
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

    public void SetLobbyId(CSteamID lobbyId)
    {
        _currentLobbyId = lobbyId;
    }

    public CSteamID GetLobbyId()
    {
        return _currentLobbyId;
    }

    public void RegisterInit(bool createLobby, bool useSteam)
    {
        _wantToInit = true;
        _wantToAutoCreateLobby = createLobby;
        _wantToUseSteam = useSteam;
    }

    private void Init(bool createLobby, bool useSteam)
    {
        SteamServer steamServer = new SteamServer();
        
        if (createLobby)
        {
            if (useSteam)
            {
                Debug.Log("Starting server with steam processes");
                Server = new Server(steamServer);
                Server.Start(0, maxClientCount);
            }
            else
            {
                Server = new Server();
                Server.Start(port, maxClientCount);
            }
            SubscribeToServerHooks();

            _host = true;
        }
        
        if (useSteam)
        {
            Debug.Log("Starting client with steam processes");
            Client = new Client(new SteamClient(steamServer));
        }
        else
        {
            Client = new Client();
        }
        SubscribeToClientHooks();

        if (useSteam && createLobby)
        {
            Debug.Log("Joining a hosted game with steam processes");
            Client.Connect($"127.0.0.1");
        }
        else if (useSteam)
        {
            Debug.Log("Joining a non hosted game with steam processes");
            CSteamID hostId = SteamMatchmaking.GetLobbyOwner(_currentLobbyId);
            
            Client.Connect(hostId.ToString());
        }
        else // not using steam, we could be hosting but it doesent matter
        {
            Debug.Log("Joining without steam");
            Client.Connect($"127.0.0.1:{port}");
        }

        usingSteam = useSteam;
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
        if (usingSteam && !SteamManager.Initialized)
        {
            return;
        }
        
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
