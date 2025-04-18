using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleInteraction : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float maxDistance;
    [SerializeField] private Image interact;
    [SerializeField] private Transform camViewLoc;
    [SerializeField] private float zoomTime = 2;
    [SerializeField] private float returnTime = 2;

    private bool _interacting = false;
    private bool _returning = false;
    
    private void Update()
    {
        if (!_interacting || !playerCam)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxDistance))
            {
                if (hit.transform == transform)
                {
                    interact.gameObject.SetActive(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        _interacting = true;
                        initTime = DateTime.Now;
                        initLoc = playerCam.position;
                        initRot = playerCam.rotation;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        playerCam.parent.GetComponent<MoveCamera>().cancel = true;
                        playerCam.GetComponent<CameraController>().cancel = true;
                        player.cancel = true;
                    }
                }
                else
                {
                    interact.gameObject.SetActive(false);
                }
            }
            else
            {
                interact.gameObject.SetActive(false);
            }
        }
        else
        {
            interact.gameObject.SetActive(false);
        }
        
        if (Input.GetMouseButtonDown(1) && _interacting)
        {
            StartCoroutine(GoBack());
            initTime = DateTime.Now;
        }

        if (_interacting)
        {
            LerpCam();
        }

        if (_returning)
        {
            LerpCamBack();
        }
    }
    
    private DateTime initTime;
    private Vector3 initLoc;
    private Quaternion initRot;

    private void LerpCam()
    {
        playerCam.transform.position = Vector3.Lerp(initLoc, camViewLoc.position,
            ((float)(DateTime.Now - initTime).TotalSeconds / zoomTime));
        playerCam.transform.rotation = Quaternion.Lerp(initRot, camViewLoc.rotation,
            (float)(DateTime.Now - initTime).TotalSeconds / zoomTime);
    }
    
    private void LerpCamBack()
    {
        playerCam.transform.position = Vector3.Lerp(camViewLoc.position, initLoc,
            (float)(DateTime.Now - initTime).TotalSeconds / returnTime);
        playerCam.transform.rotation = Quaternion.Lerp(camViewLoc.rotation, initRot,
            (float)(DateTime.Now - initTime).TotalSeconds / returnTime);
    }

    private IEnumerator GoBack()
    {
        _returning = true;
        yield return new WaitForSeconds(returnTime);
        _interacting = false;
        playerCam.parent.GetComponent<MoveCamera>().cancel = false;
        playerCam.GetComponent<CameraController>().cancel = false;
        player.cancel = false;
        _returning = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
