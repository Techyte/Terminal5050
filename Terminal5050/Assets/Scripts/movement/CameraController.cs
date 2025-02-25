using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool cancel;
    
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    [SerializeField] private float clampAngle = 85f;

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform itemDisplay;

    [Header("Camera Adjustment Controls")]
    [SerializeField] private PlayerMovement playerController;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float viewBobIntensity = 0.05f;
    [SerializeField] private float viewBobSpeed = 14f;

    private Camera thisCam;
    
    float defaultPosY = 0;
    float itemDefaultPosY = 0;
    float timer = 0;
    
    private float xRotation;
    private float yRotation;
    private float zRotation;

    private void Awake()
    {
        thisCam = GetComponent<Camera>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultPosY = transform.localPosition.y;
        itemDefaultPosY = itemDisplay.localPosition.y;
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

        transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        
        // We are grounded, so recalculate move direction based on axes
        float curSpeedX = Input.GetAxis("Vertical");
        float curSpeedY = Input.GetAxis("Horizontal");
        Vector3 moveDirection = (Vector3.forward * curSpeedX) + (Vector3.right * curSpeedY);
        
        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            //Player is moving
            timer += Time.deltaTime * viewBobSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x,
                defaultPosY + Mathf.Sin(timer) * viewBobIntensity, transform.localPosition.z);
            itemDisplay.localPosition = new Vector3(itemDisplay.localPosition.x,
                itemDefaultPosY + Mathf.Sin(timer) * viewBobIntensity/2, itemDisplay.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x,
                Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * viewBobSpeed),
                transform.localPosition.z);
            itemDisplay.localPosition = new Vector3(itemDisplay.localPosition.x,
                Mathf.Lerp(itemDisplay.localPosition.y, itemDefaultPosY, Time.deltaTime * viewBobSpeed),
                itemDisplay.localPosition.z);
        }
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
