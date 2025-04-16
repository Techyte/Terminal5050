using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public bool on;
    [SerializeField] private Material focusedMat;
    [SerializeField] private Material idleMat;
    [SerializeField] private MeshRenderer blinkingLight;
    [SerializeField] private float turnOnSpeed = 1;

    private bool enabled;

    private Camera _cam;
    private float _rot;
    private float _xRot;
    private float _targetXRot;

    private void Awake()
    {
        Disable();
        _cam = GetComponent<Camera>();
        _targetXRot = transform.parent.eulerAngles.x;
    }

    private void Update()
    {
        _cam.enabled = on;
        _xRot = Mathf.Lerp(transform.parent.eulerAngles.x, _targetXRot, turnOnSpeed * Time.deltaTime);
        transform.parent.localRotation = Quaternion.Euler(_xRot, _rot, 0);
    }

    public void Rotation(float newRot)
    {
        _rot = newRot;
    }

    public void Enable()
    {
        on = true;
        blinkingLight.material = focusedMat;
    }

    public void Disable()
    {
        on = false;
        blinkingLight.material = idleMat;
    }

    public void Trigger()
    {
        if (!enabled)
        {
            _targetXRot = 0;
            enabled = true;
            CameraManager.Instance.AddCam(this);
        }
    }
}
