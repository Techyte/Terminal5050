using UnityEngine;
using UnityEngine.UI;

public class PersonalPowerManager : MonoBehaviour
{
    public float maxCharge;
    public float charge;
    [SerializeField] private Image chargeDisplay;
    public Transform batteryLocation;
    
    private float initialWidth;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        initialWidth = chargeDisplay.transform.GetComponent<RectTransform>().rect.width;
    }

    private void Update()
    {
        if (!_player.local)
            return;
        
        chargeDisplay.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(charge / maxCharge * initialWidth,
            chargeDisplay.transform.GetComponent<RectTransform>().rect.height);
    }
}
