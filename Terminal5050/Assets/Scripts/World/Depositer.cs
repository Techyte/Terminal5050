using Riptide;
using UnityEngine;

public class Depositer : Interactable
{
    public static Depositer Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void Interact(Player player)
    {
        if (player.local)
        {
            Item currentItem = player.inventory.smallItems[player.inventory.selectedIndex];

            if (currentItem != null)
            {
                if (currentItem.template.canBeSold)
                {
                    SendDepositItemMessage(player.id, player.inventory.selectedIndex, currentItem.template.sellPrice);
                }
            }
        }
    }

    private void RemoveItem(ushort id, int index)
    {
        // remove the item from the players inventory
        if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
        {
            Debug.Log($"Removing item for id {id}");
            if (player.local)
            {
                ActionBar.NewOutput($"Sold {player.inventory.smallItems[index].template.name}");
            }
            
            player.inventory.smallItems[index] = null;
            player.inventory.UpdateSelectedItem();
        }
    }

    public static void SendDepositItemMessage(ushort id, int itemToDepositIndex, int value)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            // add the value of the item
            BankManager.Instance.SetNewValue(BankManager.Instance.Value + value);
            
            // change the value we send to the new total, this is what the clients will receive
            value = BankManager.Instance.Value;
            
            Instance.RemoveItem(id, itemToDepositIndex);
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.ItemDeposited);
            message.AddUShort(id);
            message.AddInt(itemToDepositIndex);
            message.AddInt(value);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.DepositItem);
            message.AddInt(itemToDepositIndex);
            message.AddInt(value);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static void ServerReceivedDepositItem(ushort id, int itemToDepositIndex, int value)
    {
        SendDepositItemMessage(id, itemToDepositIndex, value);
    }

    public static void ClientReceivedItemDeposited(ushort id, int itemToDepositIndex, int value)
    {
        BankManager.Instance.SetNewValue(value);

        Instance.RemoveItem(id, itemToDepositIndex);
    }
}
