using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public bool on;

    private Camera _cam;
    private float _rot;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        _cam.enabled = on;
        transform.parent.rotation = Quaternion.Euler(0, _rot, 0);
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
