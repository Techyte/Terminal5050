using System;
using System.Collections.Generic;
using System.Linq;
using Riptide;
using UnityEngine;

public class PlayerSpawningInfo : MonoBehaviour
{
    public static PlayerSpawningInfo Instance;
    
    [SerializeField] private Transform spawnLocation;
    
    public Transform SpawnLocation => spawnLocation;

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        // If we are the host we only need to send out state blasts, we dont need to bother setting our own position because we already know it
        if (NetworkManager.Instance.Server != null)
        {
            ServerStateBlast();
        }
        else
        {
            ClientSendPosRot();
        }
    }

    private void ClientSendPosRot()
    {
        Debug.Log("Client sending PosRot");
        
        Message posRotMessage = Message.Create(MessageSendMode.Unreliable, ClientToServerMessageId.PosRot);

        Player player = Player.LocalPlayer;

        if (player != null)
        {
            posRotMessage.AddVector3(player.transform.position);
            posRotMessage.AddQuaternion(player.rotationManager.transform.rotation);
        
            NetworkManager.Instance.Client.Send(posRotMessage);
        }
    }

    private void ServerStateBlast()
    {
        Message posRotMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientMessageId.PosRotBlast);

        List<Player> players = NetworkManager.Instance.players.Values.ToList();

        posRotMessage.AddInt(players.Count);

        foreach (var player in players)
        {
            posRotMessage.AddUShort(player.id);
            posRotMessage.AddVector3(player.transform.position);
            posRotMessage.AddQuaternion(player.rotationManager.transform.rotation);
        }
        
        NetworkManager.Instance.Server.SendToAll(posRotMessage, Player.LocalPlayer.id);
    }

    public void ClientReceivePosRotBlast(Message message)
    {
        int length = message.GetInt();
        
        if (length != 0)
        {
            for (int i = 0; i < length; i++)
            {
                ushort id = message.GetUShort();
                Vector3 position = message.GetVector3();
                Quaternion rotation = message.GetQuaternion();
                
                if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
                {
                    if (!player.local)
                    {
                        player.transform.position = position;
                        player.rotationManager.transform.rotation = rotation;
                    }
                }
            }
        }
    }

    public void ServerReceivedPosRot(ushort id, Vector3 pos, Quaternion rot)
    {
        if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
        {
            if (!player.local)
            {
                Debug.Log($"setting pos to {pos.ToString()}");
                player.transform.position = pos;
                player.rotationManager.transform.rotation = rot;
            }
        }
    }
}
