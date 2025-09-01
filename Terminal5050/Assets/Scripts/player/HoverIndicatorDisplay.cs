using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverIndicatorDisplay : MonoBehaviour
{
    [SerializeField] private ManualDoorInteractionManager interactionManager;
    [SerializeField] private LineRendererUi lineRendererUi;
    [SerializeField] private RectTransform center;
    [SerializeField] private TextMeshProUGUI indicatorText;
    [SerializeField] private Image interactionImage;
    [SerializeField] private Transform indicatorLabel;
    [SerializeField] private GameObject indicatorUI;
    [SerializeField] private float hoverSpeed = 8;
    [SerializeField] private float smooth = 2;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform origin;

    private Interactable _focused;
    
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        indicatorUI.SetActive(false);
    }

    private void Update()
    {
        if (_player.playerPauseManager.Paused || !_player.local)
        {
            return;
        }

        _focused = interactionManager.focused;
        
        indicatorUI.SetActive(_focused != null);

        if (_focused != null)
        {
            indicatorText.text = _focused.GetHoverText(_player);
            Vector2 screenPos = playerCamera.WorldToScreenPoint(_focused.GetHoverBounds());
            interactionImage.transform.position = screenPos;
            Sway();
        }
        else
        {
            interactionImage.transform.position = center.position;
        }
    }

    private void Sway()
    {
        float mouseXItem = Input.GetAxis("Mouse X") * hoverSpeed;
        float mouseYItem = Input.GetAxis("Mouse Y") * hoverSpeed;

        Vector2 targetRotation = new Vector2(mouseXItem, mouseYItem);
        
        indicatorLabel.position = Vector3.Slerp(indicatorLabel.position, (Vector2)origin.position + targetRotation, smooth * Time.deltaTime);
    }
}
