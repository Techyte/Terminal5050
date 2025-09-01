using UnityEngine;
using UnityEngine.UI;

public class BankManager : MonoBehaviour
{
    public static BankManager Instance;
    [SerializeField] private Image bankedDisplay;
    [SerializeField] private int maxBankValue;
    [SerializeField] private int requiredBankValue;
    [SerializeField] private Image requiredIndicator;
    [SerializeField] private Color reachedRequiredColour;
    [SerializeField] private Color notReachedRequiredColour;

    private int _value;
    private float _initialWidth;

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
        _initialWidth = bankedDisplay.transform.GetComponent<RectTransform>().rect.width;
    }

    public void SetNewValue(int newValue)
    {
        _value = newValue;
    }
    
    private void Update()
    {
        bankedDisplay.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(
            _value / (float)maxBankValue * _initialWidth,
            /*bankedDisplay.transform.GetComponent<RectTransform>().rect.height*/ 1);
        
        Debug.Log(bankedDisplay.transform.GetComponent<RectTransform>().sizeDelta);
        
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
