using Unity.Mathematics;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public bool on;

    private Camera _cam;
    private float _rot;
    private Quaternion _originRot;

    private void Awake()
    {
        _originRot = transform.rotation;
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        _cam.enabled = on;
        transform.rotation = _originRot * quaternion.Euler(Vector3.up * _rot);
    }

    public void Rotation(float newRot)
    {
        _rot = newRot;
    }

    public void Enable()
    {
        on = true;
    }

    public void Disable()
    {
        on = false;
    }
}
