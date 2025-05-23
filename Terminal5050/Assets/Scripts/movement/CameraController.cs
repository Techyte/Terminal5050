using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool cancel;
    
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    [SerializeField] private float clampAngle = 85f;

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform itemDisplay;
    [SerializeField] private float headRotationYLimit;
    [SerializeField] private float headRotationXLimit;
    [SerializeField] private Transform headTransform;

    [Header("Camera Adjustment Controls")]
    [SerializeField] private PlayerMovement playerController;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float viewBobIntensity = 0.05f;
    [SerializeField] private float viewBobSpeed = 14f;
    [SerializeField] private float smooth = 8;
    [SerializeField] private float swayMultiplier = 2;

    private Camera thisCam;
    
    float defaultPosY = 0;
    float itemDefaultPosY = 0;
    float timer = 0;
    
    private float xRotation;
    private float yRotation;
    private float zRotation;

    private float defaultHeadXRotation;

    private AudioListener _thisListener;

    private Player _player;
    
    private void Awake()
    {
        thisCam = GetComponent<Camera>();
        _player = playerController.GetComponent<Player>();
        _thisListener = GetComponent<AudioListener>();
        defaultHeadXRotation = headTransform.eulerAngles.z;
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
        if (!_player.local)
        {
            thisCam.enabled = false;
            _thisListener.enabled = false;
            return;
        }
        
        thisCam.enabled = true;
        _thisListener.enabled = true;
        
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
        
        float mouseXItem = Input.GetAxis("Mouse X") * swayMultiplier;
        float mouseYItem = Input.GetAxis("Mouse Y") * swayMultiplier;
        
        Quaternion rotationX = Quaternion.AngleAxis(-mouseYItem, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseXItem, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;
        
        itemDisplay.localRotation = Quaternion.Slerp(itemDisplay.localRotation, targetRotation, smooth * Time.deltaTime);
        
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

    private float _currentYRotation;
    private float _currentXRotation;

    public void ChangeRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        Vector3 headEuler = headTransform.rotation.eulerAngles;

        float difference = _currentYRotation - euler.y;
        if (difference < 0)
        {
            difference = -difference;
        }

        // if (difference - euler.y > headRotationYLimit)
        // {
        //     headTransform.rotation = Quaternion.Euler(headEuler.x, euler.y, headEuler.z);
        // }
        // else
        // {
        //     Vector3 currentEuler = transform.rotation.eulerAngles;
        //     transform.rotation = Quaternion.Euler(currentEuler.x, euler.y, currentEuler.z);
        //     headTransform.rotation = Quaternion.Euler(headEuler.x, 3.783f, headEuler.z);
        //     _currentYRotation = euler.y;
        // }
        Vector3 currentEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentEuler.x, euler.y, currentEuler.z);
        
        _currentXRotation = Mathf.Clamp(euler.x, -headRotationXLimit, headRotationXLimit);
    }

    private void LateUpdate()
    {
        if (!_player.local)
        {
            Vector3 headEuler = headTransform.rotation.eulerAngles;
            headTransform.rotation = Quaternion.Euler(headEuler.x, headEuler.y, defaultHeadXRotation + _currentXRotation);
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
