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
            ClientStateInfo();
        }
    }

    private void ServerStateBlast()
    {
        if (Player.LocalPlayer == null)
        {
            return;
        }
        
        ServerPosRotBlast();
        ServerPowerBlast();
        ServerSendCameraRotation();
        ServerSendEntityPosRot();
    }

    private void ClientStateInfo()
    {
        if (Player.LocalPlayer == null)
        {
            return;
        }
        
        ClientSendPosRot();
        ClientPowerInfo();
    }

    private void ServerSendCameraRotation()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientMessageId.CameraRot);
        message.AddFloat(CameraManager.Instance.rotation);

        NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
    }

    private void ServerSendEntityPosRot()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientMessageId.EntityPosRotBlast);

        message.AddInt(Entity.entities.Count);

        foreach (var entity in Entity.entities.Values)
        {
            message.AddString(entity.id);
            message.AddVector3(entity.transform.position);
            message.AddQuaternion(entity.transform.rotation);
        }

        NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
    }

    #region PosRot

    private void ClientSendPosRot()
    {
        Message posRotMessage = Message.Create(MessageSendMode.Unreliable, ClientToServerMessageId.PosRot);

        Player player = Player.LocalPlayer;

        posRotMessage.AddVector3(player.transform.position);
        posRotMessage.AddQuaternion(player.rotationManager.transform.rotation);
        
        NetworkManager.Instance.Client.Send(posRotMessage);
    }

    private void ServerPosRotBlast()
    {
        Message posRotMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientMessageId.PosRotBlast);

        List<Player> players = NetworkManager.Instance.players.Values.ToList();

        posRotMessage.AddInt(players.Count);

        foreach (var player in players)
        {
            posRotMessage.AddUShort(player.id);
            posRotMessage.AddVector3(player.transform.position);
            posRotMessage.AddQuaternion(player.rotationManager.cam.transform.rotation);
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
                        player.rotationManager.camCamera.ChangeRotation(rotation);
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
                player.transform.position = pos;
                player.rotationManager.camCamera.ChangeRotation(rot);
            }
        }
    }

    #endregion

    #region Power

    private void ServerPowerBlast()
    {
        Message posRotMessage = Message.Create(MessageSendMode.Unreliable, ServerToClientMessageId.PowerBlast);

        List<Player> players = NetworkManager.Instance.players.Values.ToList();

        posRotMessage.AddInt(players.Count);

        foreach (var player in players)
        {
            posRotMessage.AddUShort(player.id);
            posRotMessage.AddFloat(player.powerManager.charge);
            posRotMessage.AddFloat(player.powerManager.maxCharge);
        }

        posRotMessage.AddFloat(PowerManager.Instance.CurrentCharge);
        posRotMessage.AddFloat(PowerManager.Instance.MaxCharge);
        
        NetworkManager.Instance.Server.SendToAll(posRotMessage, Player.LocalPlayer.id);
    }
    
    public void ServerReceivedPowerInfo(ushort id, float charge, float maxCharge)
    {
        if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
        {
            if (!player.local)
            {
                player.powerManager.charge = charge;
                player.powerManager.maxCharge = maxCharge;
            }
        }
    }
    
    private void ClientPowerInfo()
    {
        PersonalPowerManager pManager = Player.LocalPlayer.powerManager;
        
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerMessageId.PersonalPower);
        message.AddFloat(pManager.charge);
        message.AddFloat(pManager.maxCharge);

        NetworkManager.Instance.Client.Send(message);
    }
    
    public void ClientReceivedPowerBlast(Message message)
    {
        int length = message.GetInt();
        
        if (length != 0)
        {
            for (int i = 0; i < length; i++)
            {
                ushort id = message.GetUShort();
                float charge = message.GetFloat();
                float maxCharge = message.GetFloat();
                
                if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
                {
                    if (!player.local)
                    {
                        player.powerManager.charge = charge;
                        player.powerManager.maxCharge = maxCharge;
                    }
                }
            }
        }

        PowerManager.Instance.SetCharge(message.GetFloat());
        PowerManager.Instance.MaxCharge = message.GetFloat();
    }

    #endregion
    
    public void ClientReceivedEntityPosRot(Message message)
    {
        int length = message.GetInt();
        
        if (length != 0)
        {
            for (int i = 0; i < length; i++)
            {
                string id = message.GetString();
                Vector3 pos = message.GetVector3();
                Quaternion rot = message.GetQuaternion();
                
                if (Entity.entities.TryGetValue(id, out Entity entity))
                {
                    entity.transform.position = pos;
                    entity.transform.rotation = rot;
                }
            }
        }
    }
}
