using UnityEngine;
using UnityEngine.Rendering;


public class PlayerController : MonoBehaviour
{
    [Header("Referencies")]
    [SerializeField] private Camera _mCamera;
    [SerializeField] private Rigidbody rb;

    [Header("Speed")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runnigSpeed = 9f;

    private float currentSpeed = 0f;

    [Header("Ground")]
    private bool isOnGround;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float playerHeight = 2;
    [SerializeField] private float playerRadius = 0.4f;

    [Header("Jumping")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpForce = 5f;

    private Vector3 inputPlayer;
    private Vector2 mouseInput;
    private float xRot;

    [SerializeField] private float cameraSensivility;

    private void Start()
    {
        _mCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        inputPlayer = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        HandleJump();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        MoveCamara();
        Debug.Log($"On Ground: {isOnGround}");
    }

    private void MoveCharacter()
    {
        Vector3 moveVector = transform.TransformDirection(inputPlayer) * currentSpeed;
        rb.linearVelocity = new Vector3(moveVector.x, rb.linearVelocity.y, moveVector.z);

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runnigSpeed;
        else 
            currentSpeed = moveSpeed;
    }

    private void MoveCamara()
    {
        xRot -= mouseInput.y * cameraSensivility;

        xRot = Mathf.Clamp(xRot, -80f, 80f);

        transform.Rotate(0f, mouseInput.x * cameraSensivility, 0f);

        _mCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GroundChecker())
            rb.linearVelocity += Vector3.up * jumpForce;

        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    private bool GroundChecker()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        isOnGround = Physics.SphereCast(origin, playerRadius, Vector3.down * playerHeight, out hit, 2f, groundMask);
        return isOnGround;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * playerHeight, playerRadius);
    }
}
