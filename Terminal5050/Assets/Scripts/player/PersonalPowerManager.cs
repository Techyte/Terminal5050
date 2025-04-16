using UnityEngine;
using UnityEngine.UI;

public class PersonalPowerManager : MonoBehaviour
{
    public float maxCharge;
    public float charge;
    [SerializeField] private Image chargeDisplay;
    public Transform batteryLocation;
    
    private float initialWidth;

    private void Awake()
    {
        initialWidth = chargeDisplay.transform.GetComponent<RectTransform>().rect.width;
    }

    private void Update()
    {
        chargeDisplay.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(charge / maxCharge * initialWidth,
            chargeDisplay.transform.GetComponent<RectTransform>().rect.height);
    }
}
