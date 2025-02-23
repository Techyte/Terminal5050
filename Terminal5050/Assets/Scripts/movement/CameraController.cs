using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool cancel;
    
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    [SerializeField] private float clampAngle = 85f;

    [SerializeField] private Transform orientation;

    [Header("Camera Adjustment Controls")]
    [SerializeField] private PlayerMovement playerController;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float FOVChangeSpeed;
    [SerializeField] private float tiltSpeed;

    private Camera thisCam;
    
    private float xRotation;
    private float yRotation;
    private float zRotation;
    private float fov;

    private void Awake()
    {
        thisCam = GetComponent<Camera>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fov = defaultFOV;
    }

    private void Update()
    {
        if (cancel)
            return;
        
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX; // left and right??
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY; // up and down??

        yRotation += mouseX;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);

        thisCam.fieldOfView = fov;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    
    public void ToggleCursorMode()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }  
    }
}
