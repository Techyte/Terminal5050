using Riptide;

public class MessageReceiver
{
    [MessageHandler(((ushort)ClientToServerMessageId.BasicInfo))]
    private static void ServerBasicInfo(ushort clientId, Message message)
    {
        NetworkManager.Instance.ServerReceivedClientBasicInfo(clientId, message.GetString());
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.NewPlayerJoined))]
    private static void ClientNewPlayerJoined(Message message)
    {
        NetworkManager.Instance.ClientNewPlayerJoined(message.GetUShort(), message.GetString());
    }
}
