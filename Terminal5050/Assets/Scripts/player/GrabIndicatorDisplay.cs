using TMPro;
using UnityEngine;

public class GrabIndicatorDisplay : MonoBehaviour
{
    [SerializeField] private ManualDoorInteractionManager interactionManager;
    [SerializeField] private TextMeshProUGUI indicatorText;
    [SerializeField] private Transform indicatorLabel;
    [SerializeField] private GameObject indicatorUI;
    [SerializeField] private float hoverSpeed = 8;
    [SerializeField] private float smooth = 2;

    private WorldItem _focusedItem;

    private Vector2 _origin;
    
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        indicatorUI.SetActive(false);
        _origin = indicatorLabel.position;
    }

    private void Update()
    {
        if (!_player.local)
        {
            return;
        }

        if (_player.playerPauseManager.Paused)
        {
            return;
        }

        _focusedItem = interactionManager.focusedItem;
        
        indicatorUI.SetActive(_focusedItem != null);

        if (_focusedItem != null)
        {
            indicatorText.text = _focusedItem.Item.template.name;
            Sway();
        }
    }

    private Vector2 _vel = Vector2.zero;

    private void Sway()
    {
        float mouseXItem = Input.GetAxis("Mouse X") * hoverSpeed;
        float mouseYItem = Input.GetAxis("Mouse Y") * hoverSpeed;

        Vector2 targetRotation = new Vector2(mouseXItem, mouseYItem);
        
        indicatorLabel.position = Vector3.Slerp(indicatorLabel.position, _origin + targetRotation, smooth * Time.deltaTime);
    }
}
