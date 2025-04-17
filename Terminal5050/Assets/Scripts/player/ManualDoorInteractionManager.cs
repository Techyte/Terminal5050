using UnityEngine;
using UnityEngine.UI;

public class ManualDoorInteractionManager : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float maxDistance;
    [SerializeField] private Image interact;
    [SerializeField] private LayerMask manualDoorLayer;
    [SerializeField] private LayerMask cycleCameraLayer;
    [SerializeField] private LayerMask interactableLayer;
    private Inventory _inventory;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        
        _inventory = GetComponent<Inventory>();
    }
    
    private void Update()
    {
        if (!_player.local)
            return;
        
        RaycastHit hit;

        bool wantToInteract = false;

        wantToInteract = Input.GetMouseButtonDown(0);

        bool interacted = false;
        
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxDistance, manualDoorLayer))
        {
            interacted = true;
            if (wantToInteract)
            {
                hit.transform.gameObject.GetComponent<ManualDoorInteract>().Interact();
            }
        }else if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxDistance, cycleCameraLayer))
        {
            interacted = true;
            if (wantToInteract)
            {
                CameraManager.Instance.Cycle();
            }
        }else if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxDistance, interactableLayer))
        {
            Interactable interactHit = hit.transform.gameObject.GetComponent<Interactable>();
            if (interactHit)
            {
                interacted = true;
                if (wantToInteract)
                {
                    interactHit.Interact(GetComponent<PersonalPowerManager>());
                }
            }
        }
        
        interact.gameObject.SetActive(interacted);
    }
}
