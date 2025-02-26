using System;
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
    
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxDistance, manualDoorLayer))
        {
            interact.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                hit.transform.gameObject.GetComponent<ManualDoorInteract>().Interact();
            }
        }else if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxDistance, cycleCameraLayer))
        {
            interact.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                CameraManager.Instance.Cycle();
            }
        }
        else
        {
            interact.gameObject.SetActive(false);
        }
    }
}
