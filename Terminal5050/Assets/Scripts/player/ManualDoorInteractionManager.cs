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

    public WorldItem focusedItem;

    private Player _player;
    private PlayerPauseManager _pause;

    private void Awake()
    {
        _player = GetComponent<Player>();
        
        _inventory = GetComponent<Inventory>();
        _pause = GetComponent<PlayerPauseManager>();
    }
    
    private void Update()
    {
        if (!_player.local || _pause.Paused)
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
            Interactable[] interactHit = hit.transform.gameObject.GetComponents<Interactable>();
            if (interactHit.Length > 0)
            {
                interacted = true;
                if (wantToInteract)
                {
                    for (int i = 0; i < interactHit.Length; i++)
                    {
                        interactHit[i].Interact(_player);
                    }
                }
            }

            focusedItem = hit.transform.gameObject.GetComponent<WorldItem>();
        }
        else
        {
            focusedItem = null;
        }
        
        interact.gameObject.SetActive(interacted);
    }
}
