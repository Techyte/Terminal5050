using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance;

    [SerializeField] private int maxLoad;
    [SerializeField] private TextMeshProUGUI loadText;
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI statusText;
    
    private Dictionary<string, int> _loads = new Dictionary<string, int>();

    public int currentLoad => GetLoad();
    public int maximumLoad => maxLoad;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadIncreased(string id, int load)
    {
        _loads.Add(id, load);
        if (currentLoad == maximumLoad)
        {
            CMDManager.Instance.Output("Power load at capacity, please do not turn on any more devices or risk a blackout");
        }
    }

    public void LoadReduced(string id)
    {
        _loads.Remove(id);
    }

    private int GetLoad()
    {
        int total = 0;
        for (int i = 0; i < _loads.Values.Count; i++)
        {
            total += _loads.Values.ToList()[i];
        }

        return total;
    }

    private void Update()
    {
        string newLoadText = "";

        for (int i = 0; i < _loads.Values.Count; i++)
        {
            newLoadText += _loads.Keys.ToList()[i] + ": " + _loads.Values.ToList()[i] + "\n";
        }

        loadText.text = newLoadText;

        totalText.text = $"{currentLoad}/{maxLoad}";

        switch ((float)currentLoad/maxLoad)
        {
            case <= 0.5f:
                statusText.text = "OK";
                break;
            case <= 0.75f:
                statusText.text = "HEAVY";
                break;
            case 1:
                statusText.text = "FULL";
                break;
            case > 1:
                statusText.text = "OVER";
                Overloaded();
                break;
        }
    }

    private void Overloaded()
    {
        DoorManager.Instance.CloseAllDoors();
        SpeakerManager.Instance.PowerOverload();
    }
}
