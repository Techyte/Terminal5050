using UnityEngine;

public class PlayerRotationManager : MonoBehaviour
{
    public Transform cam;
    public CameraController camCamera;

    private Player _player;

    private void Awake()
    {
        _player = transform.parent.GetComponent<Player>();
        camCamera = cam.GetComponent<CameraController>();
    }

    private void Update()
    {
        Vector3 camEuler = cam.eulerAngles;
        Vector3 selfEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(selfEuler.x, camEuler.y, selfEuler.z);
    }
}
