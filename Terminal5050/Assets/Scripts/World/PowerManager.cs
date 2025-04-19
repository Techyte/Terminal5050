using System.Collections.Generic;
using Riptide;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance;
    [SerializeField] private Image chargeDisplay;

    public float MaxCharge;

    public float CurrentCharge
    {
        get
        {
            return currentCharge;
        }
    }

    private float currentCharge;

    private float initialWidth;

    public Dictionary<string, float> drains = new Dictionary<string, float>();

    private void Awake()
    {
        Instance = this;
        initialWidth = chargeDisplay.transform.GetComponent<RectTransform>().rect.width;
        currentCharge = MaxCharge;
    }

    public void NewDrain(string name, float drainPerSecond)
    {
        drains.TryAdd(name, drainPerSecond);
    }

    public void RemoveDrain(string name)
    {
        if (drains.ContainsKey(name))
        {
            drains.Remove(name);
        }
    }

    public void SetCharge(float newCharge)
    {
        currentCharge = newCharge;
        if (currentCharge > MaxCharge)
        {
            currentCharge = MaxCharge;
        }

        if (currentCharge < 0)
        {
            currentCharge = 0;
        }
    }

    public void ChangeCharge(float amount)
    {
        currentCharge += amount;
        if (currentCharge > MaxCharge)
        {
            currentCharge = MaxCharge;
        }

        if (currentCharge < 0)
        {
            currentCharge = 0;
        }
    }

    private bool _overloaded;

    private void Update()
    {
        chargeDisplay.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(CurrentCharge / MaxCharge * initialWidth, chargeDisplay.transform.GetComponent<RectTransform>().rect.height);
        
        // need to be the host in order to initiate the overload sequence, ensures its synced across all players
        if (CurrentCharge <= 0 && !_overloaded && NetworkManager.Instance.Server != null)
        {
            Overloaded();
        }
    }

    public void Overloaded()
    {
        currentCharge = 0;
        _overloaded = true;
        
        DoorManager.Instance.CloseAllDoors();
        SpeakerManager.Instance.PowerOverload();

        if (NetworkManager.Instance.Server != null)
        {
            SendPowerOverloadMessage();
        }
    }

    private void SendPowerOverloadMessage()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.PowerOverloaded);

        NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
    }
}
