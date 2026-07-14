using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private float rotateSpeed = 5f;
    private float horizontalInput;
    private float verticalInput;
    private float initialVelocity = 0f;

    [SerializeField] private float walkingVelocity = 5f;
    [SerializeField] private float runningVelocity = 10f;
    [SerializeField] private float accelerationRate = 75f;

    [SerializeField] private Camera mainCamera;

    public float sphereCastRadius = 0.34f;

    private Vector3 prevDir;
    private Vector3 movementDir;
    private bool canMove = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        prevDir = transform.forward;
        
        movementDir = transform.forward;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        movementDir = verticalInput*cameraForward+horizontalInput*cameraRight;

        if (movementDir.magnitude > 0.1f)
        {
            RaycastHit hit;
            float verticalOffset = 0.5f;
            float sphereCastDist = 0.5f;
            Vector3 castOrigin = transform.position + Vector3.up * verticalOffset;

            if(Physics.SphereCast(castOrigin,sphereCastRadius,movementDir,out hit, sphereCastDist))
            {
                if (hit.normal.y < 0.1)
                {
                    movementDir = Vector3.ProjectOnPlane(movementDir, hit.normal).normalized;
                }
            }
        }

        if (!canMove)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        float targetVelocity = Input.GetKey(KeyCode.LeftShift) ? runningVelocity : walkingVelocity;
        if (movementDir == Vector3.zero)
        {
            targetVelocity = initialVelocity;
            
        }
        else
        {
            prevDir = movementDir;
        }
        
        Vector3 targetVelocityVector = movementDir * targetVelocity;
        
        // Preserve the current vertical velocity (e.g., gravity)
        targetVelocityVector.y = rb.linearVelocity.y;

        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity
            ,targetVelocityVector,accelerationRate*Time.fixedDeltaTime);

        
        Quaternion lookAt = Quaternion.LookRotation(prevDir);
        Quaternion currRotation = transform.rotation;

        //transform.rotation = Quaternion.Slerp(currRotation, lookAt, rotateSpeed * Time.fixedDeltaTime);
        Quaternion targetRotation = Quaternion.Slerp(currRotation,lookAt
            ,rotateSpeed*Time.fixedDeltaTime);
        rb.MoveRotation(targetRotation);
    }

    public float GetRunningVelocity()
    {
        return runningVelocity;
    }

    public void SetMoveState(bool value)
    {
        canMove = value;
    }
    public Vector3 GetMovementDir()
    {
        return movementDir;
    }

    private void OnDrawGizmos()
    {
        // 1. Match the exact origin of your SphereCast
        Vector3 castOrigin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        // 2. Match the exact radius of your SphereCast (0.3f)
        Gizmos.DrawWireSphere(castOrigin, 0.3f);
    }
}
