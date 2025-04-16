using System.Collections.Generic;
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

    public void ChangeCharge(float amount)
    {
        Debug.Log("changing charge");
        currentCharge += amount;
    }

    private void Update()
    {
        chargeDisplay.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(CurrentCharge / MaxCharge * initialWidth, chargeDisplay.transform.GetComponent<RectTransform>().rect.height);

        if (CurrentCharge <= 0)
        {
            Overloaded();
        }
    }

    private void Overloaded()
    {
        DoorManager.Instance.CloseAllDoors();
        SpeakerManager.Instance.PowerOverload();
    }
}
