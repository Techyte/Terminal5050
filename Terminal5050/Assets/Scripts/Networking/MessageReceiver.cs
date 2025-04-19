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
    
    [MessageHandler(((ushort)ClientToServerMessageId.PersonalPower))]
    private static void ServerPower(ushort clientId, Message message)
    {
        PlayerSpawningInfo.Instance.ServerReceivedPowerInfo(clientId, message.GetFloat(), message.GetFloat());
    }
    
    [MessageHandler(((ushort)ClientToServerMessageId.ToggleCamera))]
    private static void ServerToggleCameras(ushort clientId, Message message)
    {
        CameraManager.Instance.ServerReceivedToggleCameras(message.GetBool());
    }
    
    [MessageHandler(((ushort)ClientToServerMessageId.ChangedCamera))]
    private static void ServerChangeCameras(ushort clientId, Message message)
    {
        CameraManager.Instance.ServerReceivedSwitchCameras();
    }
    
    [MessageHandler(((ushort)ClientToServerMessageId.ActivateCamera))]
    private static void ServerActivateCamera(ushort clientId, Message message)
    {
        CameraManager.Instance.ServerReceivedActivateCamera(message.GetString());
    }
    
    [MessageHandler(((ushort)ClientToServerMessageId.SwapItem))]
    private static void ServerSwapItem(ushort clientId, Message message)
    {
        Inventory.ServerSwapItem(clientId, message.GetInt());
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
    
    [MessageHandler(((ushort)ServerToClientMessageId.PowerBlast))]
    private static void ClientPowerBlast(Message message)
    {
        PlayerSpawningInfo.Instance.ClientReceivedPowerBlast(message);
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.CameraToggled))]
    private static void ClientCameraToggled(Message message)
    {
        CameraManager.Instance.ClientReceivedCameraToggled(message.GetBool());
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.CameraChanged))]
    private static void ClientCameraSwitched(Message message)
    {
        CameraManager.Instance.ClientReceivedCameraSwitched(message.GetString());
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.CameraActivated))]
    private static void ClientCameraActivated(Message message)
    {
        CameraManager.Instance.ClientReceivedCameraActivated(message.GetString());
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.CameraRot))]
    private static void ClientCameraRotation(Message message)
    {
        CameraManager.Instance.ClientReceivedCameraRotation(message.GetFloat());
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.PowerOverloaded))]
    private static void ClientPowerOverloaded(Message message)
    {
        PowerManager.Instance.Overloaded();
    }
    
    [MessageHandler(((ushort)ServerToClientMessageId.ItemSwapped))]
    private static void ClientItemSwapped(Message message)
    {
        Inventory.ClientSwapItem(message.GetUShort(), message.GetInt());
    }
}
