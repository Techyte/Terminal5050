using System;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

enum ClientToServerMessageId : ushort
{
    BasicInfo,
}

enum ServerToClientMessageId : ushort
{
    NewPlayerJoined,
    CurrentPlayerInfo,
}

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    
    [SerializeField] private bool autoCreateLobby;
    [SerializeField] private bool autoJoinLobby;
    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount = 4;
    [SerializeField] private string selfUsername = "PayishVibes";

    private bool _host;
    
    private Dictionary<ushort, Player> _clientPlayers = new Dictionary<ushort, Player>();

    public Server Server;
    public Client Client;

    private void Awake()
    {
        Instance = this;
        
        Server = new Server();

        Client = new Client();
        
        SubscribeToHooks();
    }

    private void Start()
    {
        if (autoCreateLobby)
        {
            Server.Start(port, maxClientCount);

            _host = true;
            
            Client.Connect($"127.0.0.1:{port}");
        }
        else if (autoJoinLobby)
        {
            Client.Connect($"127.0.0.1:{port}");
        }
    }

    private void SubscribeToHooks()
    {
        Server.ClientConnected += ServerOnClientConnected;
        
        Client.Connected += ClientOnConnected;
        Client.ClientConnected += ClientOnClientConnected;
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
    
    public void ServerReceivedClientBasicInfo(ushort client, string username)
    {
        // Spawn a player for our own use
        
        bool local = client == Client.Id;
        
        Player newPlayer = Player.SpawnNewPlayer(username, local);
        
        _clientPlayers.Add(client, newPlayer);
        
        // notify all existing players of the new joined player
        
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.NewPlayerJoined);
        message.AddUShort(client);
        message.AddString(username);
        
        Server.SendToAll(message, client);
        
        ActionBar.Instance.NewOutput($"{username} joined the game");
    }

    public void ClientNewPlayerJoined(ushort client, string username)
    {
        Player newPlayer = Player.SpawnNewPlayer(username, false);
        
        _clientPlayers.Add(client, newPlayer);
        
        ActionBar.Instance.NewOutput($"{username} joined the game");
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
