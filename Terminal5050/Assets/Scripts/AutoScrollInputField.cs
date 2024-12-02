using UnityEngine;
using UnityEngine.UI;
using None;

public class AutoScrollInputField : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private AltTMP_InputField input;
    
    private void Awake()
    {
        scroll = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        scroll.verticalNormalizedPosition = 0;
    }
}