using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    private float moveSpeed;

    public bool cancel;

    [SerializeField] private float walkSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSPeed;

    [SerializeField] private float groundDrag;

    [Header("Ground Check")] 
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    private bool _grounded;
    
    [Space]

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform respawn;

    [SerializeField] private Rigidbody rb;
    
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Player _player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        rb.freezeRotation = true;

        respawn = PlayerSpawningInfo.Instance.SpawnLocation;
    }

    private void Update()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
        
        SpeedControl();
        MyInput();
        StateHandler();

        if (_grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if (!cancel)
        {
            MovePlayer();
        }

        if (transform.position.y < -60)
        {
            transform.position = respawn.position + Vector3.up * 0.5f;
            rb.velocity = Vector3.zero;
        }
    }

    private void MyInput()
    {
        if (_player.local)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }
    }

    private void StateHandler()
    {
        desiredMoveSpeed = walkSpeed;

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSPeed) > 4 && moveSpeed != 0)
        {
            StopAllCoroutines();
            moveSpeed = desiredMoveSpeed;
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSPeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // Basically just orient the direction vector
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        
        rb.AddForce(moveSpeed * 10f * _moveDirection.normalized, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
