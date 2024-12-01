using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoScrollInputField : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private TMP_InputField input;
    
    private void Awake()
    {
        scroll = GetComponent<ScrollRect>();
    }

    void Update()
    {
        scroll.verticalNormalizedPosition = 0;
        input.ForceLabelUpdate();
    }
}