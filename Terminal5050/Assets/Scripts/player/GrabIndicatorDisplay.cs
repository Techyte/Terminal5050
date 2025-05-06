using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrabIndicatorDisplay : MonoBehaviour
{
    [SerializeField] private ManualDoorInteractionManager interactionManager;
    [SerializeField] private LineRendererUi lineRendererUi;
    [SerializeField] private TextMeshProUGUI indicatorText;
    [SerializeField] private Image interactionImage;
    [SerializeField] private Transform indicatorLabel;
    [SerializeField] private GameObject indicatorUI;
    [SerializeField] private float hoverSpeed = 8;
    [SerializeField] private float smooth = 2;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform canvasObject;

    private WorldItem _focusedItem;
    
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

        _focusedItem = interactionManager.focusedItem;
        
        indicatorUI.SetActive(_focusedItem != null);

        if (_focusedItem != null)
        {
            indicatorText.text = _focusedItem.Item.template.name;
            Vector2 screenPos = playerCamera.WorldToScreenPoint(_focusedItem.mRenderer.bounds.center);
            interactionImage.transform.position = screenPos;
            Sway();
        }
        else
        {
            interactionImage.transform.position = Vector3.zero;
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
