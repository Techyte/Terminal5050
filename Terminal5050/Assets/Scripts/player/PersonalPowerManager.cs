using UnityEngine;
using UnityEngine.UI;

public class PersonalPowerManager : MonoBehaviour
{
    public float maxCharge;
    public float charge;
    [SerializeField] private Image chargeDisplay;

    private void Update()
    {
        chargeDisplay.transform.localScale = new Vector2(charge / maxCharge, 1);
    }
}
