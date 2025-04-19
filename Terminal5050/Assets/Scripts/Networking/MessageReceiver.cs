using Riptide;

public class MessageReceiver
{
    [MessageHandler(((ushort)ClientToServerMessageId.BasicInfo))]
    private static void ServerBasicInfo(ushort clientId, Message message)
    {
        NetworkManager.Instance.ServerReceivedClientBasicInfo(clientId, message.GetString());
    }
    
    [MessageHandler(((ushort)ClientToServerMessageId.PosRot))]
    private static void ServerPosRot(ushort clientId, Message message)
    {
        PlayerSpawningInfo.Instance.ServerReceivedPosRot(clientId, message.GetVector3(), message.GetQuaternion());
    }
    
    // Also used to tell a client that just joined to spawn itself lol
    [MessageHandler(((ushort)ServerToClientMessageId.NewPlayerJoined))]
    private static void ClientNewPlayerJoined(Message message)
    {
        NetworkManager.Instance.ClientNewPlayerJoined(message.GetUShort(), message.GetString());
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.CurrentPlayerInfo))]
    private static void ClientCurrentPlayerInfo(Message message)
    {
        NetworkManager.Instance.CurrentPlayerInfo(message);
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.PosRotBlast))]
    private static void ClientPosRotBlast(Message message)
    {
        PlayerSpawningInfo.Instance.ClientReceivePosRotBlast(message);
    }
}
