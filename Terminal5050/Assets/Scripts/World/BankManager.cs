using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankManager : MonoBehaviour
{
    public static BankManager Instance;
    [SerializeField] private Image chargeDisplay;
    [SerializeField] private RectTransform chargeDisplayText;
    [SerializeField] private int maxBankValue;
    [SerializeField] private int requiredBankValue;
    [SerializeField] private TextMeshProUGUI currencyDisplay;
    [SerializeField] private Image requiredIndicator;
    [SerializeField] private Color reachedRequiredColour;
    [SerializeField] private Color notReachedRequiredColour;

    private int _value;
    private float initialHeight;

    public int Value
    {
        get
        {
            return _value;
        }
    }

    private void Awake()
    {
        Instance = this;
        initialHeight = chargeDisplay.transform.GetComponent<RectTransform>().rect.height;
        
        float indicatorMaxHeight = requiredIndicator.transform.parent.GetComponent<RectTransform>().rect.height;
        
        requiredIndicator.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            requiredIndicator.transform.GetComponent<RectTransform>().anchoredPosition.x,
            requiredBankValue / (float)maxBankValue * indicatorMaxHeight);
    }

    public void SetNewValue(int newValue)
    {
        _value = newValue;
    }
    
    private void Update()
    {
        chargeDisplay.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(
            chargeDisplay.transform.GetComponent<RectTransform>().rect.width,
            _value / (float)maxBankValue * initialHeight);

        chargeDisplayText.sizeDelta =
            new Vector2(chargeDisplayText.rect.width, _value / (float)maxBankValue * initialHeight);

        currencyDisplay.text = $"${_value}";

        if (_value >= requiredBankValue)
        {
            requiredIndicator.color = reachedRequiredColour;
        }
        else
        {
            requiredIndicator.color = notReachedRequiredColour;
        }
    }
}
