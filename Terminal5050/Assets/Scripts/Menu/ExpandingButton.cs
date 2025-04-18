using UnityEngine;
using UnityEngine.EventSystems;

public class ExpandingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private bool expandWhenHover = true;
    [SerializeField] private float scale = 1.1f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Transform targetGraphic;

    private bool _hovering = false;
    private bool _focused = false;
    private Vector3 _baseScale;

    protected void Awake()
    {
        _baseScale = targetGraphic.localScale;
    }

    private void OnEnable()
    {
        targetGraphic.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ButtonClicked);
    }

    private void ButtonClicked()
    {
        targetGraphic.localScale = _baseScale;
    }

    private void Update()
    {
        if (!EventSystem.current)
        {
            return;
        }

        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            _focused = true;
        }
        else
        {
            _focused = false;
        }

        if (_hovering || _focused)
        {
            targetGraphic.localScale =
                Vector3.Lerp(targetGraphic.localScale, _baseScale * scale, speed * Time.deltaTime);
        }
        else
        {
            targetGraphic.localScale = Vector3.Lerp(targetGraphic.localScale, _baseScale, speed * Time.deltaTime);
        }
    }

    // private void LateUpdate()
    // {
    //     _hovering = false;
    // }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (expandWhenHover)
        {
            _hovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (expandWhenHover)
        {
            targetGraphic.localScale = _baseScale;
        }
    }
}