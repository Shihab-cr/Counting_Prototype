using System.Linq.Expressions;
using UnityEngine;

public class SheepMotor : MonoBehaviour
{
    private Rigidbody rb;
    private SheepBrain sheepBrain;

    private GameObject player;

    private Vector3 movementDir = Vector3.zero;
    private Vector3 previousDir = Vector3.zero;

    [Header("Speeds")]
    [SerializeField] private float idleVelocity =0f;
    [SerializeField] private float wanderVelocity = 1f;
    [SerializeField] private float fleeVelocity = 8f;
    [SerializeField] private float panicVelocity = 14f;
    [SerializeField] private float accelerationRate = 75f;
    [SerializeField] private float wanderRotationSpeed = 5f; // Slow, lazy turning
    [SerializeField] private float runRotationSpeed = 10f;

    [Header("Obstacle avoidance")]
    [SerializeField] private float sphereCastRadius = 0.52f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Bark boost settings")]
    [SerializeField] private float maxBarkBoostMultiplier = 1.5f;
    [SerializeField] private float boostCooldown = 2f;
    [SerializeField] private float boostDecayRate = 0.5f;
    private float currSpeedMultiplier = 1f;
    private float lastBoostTime = -10f;

    private bool isMoving = false;
    private bool canSwitchDir = true;

    private bool canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        sheepBrain = GetComponent<SheepBrain>();

        previousDir = transform.forward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        if (currSpeedMultiplier > 1f)
        {
            currSpeedMultiplier -= Time.fixedDeltaTime * boostDecayRate;
            currSpeedMultiplier = Mathf.Max(1f, currSpeedMultiplier);
        }


        switch(sheepBrain.currentState)
        {
            case SheepBrain.SheepState.Idle:
                movementDir = Vector3.zero;
                isMoving = false;
                MoveSheep(idleVelocity);
                break;
            case SheepBrain.SheepState.Fleeing:
                movementDir = (transform.position - player.transform.position).normalized;
                MoveSheep(fleeVelocity);
                break;
            case SheepBrain.SheepState.Wandering:
                if(!isMoving)
                    movementDir = GenerateWanderDir();
                MoveSheep(wanderVelocity);
                break;
            case SheepBrain.SheepState.Grazing:
                movementDir = Vector3.zero;
                isMoving = false;
                MoveSheep(idleVelocity);
                break;
            case SheepBrain.SheepState.Fighting:
                movementDir = Vector3.zero;
                MoveSheep(idleVelocity);
                break;
            case SheepBrain.SheepState.Panicking:
                //movementDir = (transform.position - player.transform.position).normalized;
                if (canSwitchDir)
                {
                    movementDir = SwitchMovementDir();
                }
                MoveSheep(panicVelocity);
                break;
        }
    }

    private void MoveSheep(float targetVelocity)
    {
        if (movementDir == Vector3.zero)
        {
            targetVelocity = idleVelocity;
        }
        else
        {
            //movementDir = ObstacleAvoidanceLogic(movementDir); //Deprecated method
            movementDir = SmartObstacleAvoidance(movementDir);
            previousDir = movementDir;
        }
        float finalVelocity = targetVelocity * currSpeedMultiplier;

        Vector3 targetVelocityVector = transform.forward * finalVelocity;


        if (movementDir == Vector3.zero) targetVelocityVector = Vector3.zero;
        // Preserve the current vertical velocity (e.g., gravity)
        targetVelocityVector.y = rb.linearVelocity.y;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocityVector
            , accelerationRate * Time.fixedDeltaTime);
        // Rotate the sheep to face the movement direction
       
        Quaternion targetRotation = Quaternion.LookRotation(previousDir);
        float currentRotationSpeed = (sheepBrain.currentState == SheepBrain.SheepState.Wandering) ? wanderRotationSpeed : runRotationSpeed;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation
            , currentRotationSpeed * Time.fixedDeltaTime));
        
    }
    public void ApplyBarkBoost()
    {
        if(Time.time >= lastBoostTime + boostCooldown)
        {
            currSpeedMultiplier = maxBarkBoostMultiplier;
            lastBoostTime = Time.time;
        }
    }
    private Vector3 GenerateWanderDir()
    {
        isMoving = true;
        Invoke(nameof(ResetIsMoving), Random.Range(1f, 3f));
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }
    private void ResetIsMoving()
    {
        isMoving = false;
    }
    private Vector3 SwitchMovementDir()
    {
        canSwitchDir = false;
        Invoke(nameof(ResetSwitchDir), 3f); // Reset the switch direction flag after 3 seconds
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    private void ResetSwitchDir()
    {
        canSwitchDir = true;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    public float GetMaxSpeed()
    {
        return panicVelocity;
    }

    //deprecated
    private Vector3 ObstacleAvoidanceLogic(Vector3 movementDir)
    {
        RaycastHit hit;
        float verticalOffset = 0.5f;
        float sphereCastDist = 1.5f;
        Vector3 castOrigin = transform.position + Vector3.up * verticalOffset;
        if (Physics.SphereCast(castOrigin, sphereCastRadius, movementDir, out hit, sphereCastDist, obstacleLayer))
        {
            if (hit.normal.y < 0.1f)
            {
                Vector3 projected = Vector3.ProjectOnPlane(movementDir, hit.normal).normalized;

                if (projected.sqrMagnitude < 0.01f)
                {
                    projected = Vector3.Cross(hit.normal, Vector3.up);
                }

                movementDir = projected.normalized;
            }
        }
        return movementDir;
    }
    //****

    private Vector3 ObstacleAvoidanceWhiskersLogic(Vector3 desiredDir)
    {
        Vector3 bestDir = desiredDir;
        RaycastHit rayHit;
        float maxAlignement = -Mathf.Infinity;
        bool foundClearPath = false;

        float verticalOffset = 0.5f;
        float sphereCastDist = 1.5f;
        Vector3 castOrigin = transform.position + Vector3.up*verticalOffset;

        for(int whisker =0; whisker<8; whisker++)
        {
            float angle = whisker * 45;
            Vector3 baseDir = (desiredDir == Vector3.zero) ? transform.forward : desiredDir;
            Vector3 testDir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir ;
            if(!Physics.SphereCast(castOrigin,sphereCastRadius,testDir,out rayHit, sphereCastDist, obstacleLayer))
            {
                float alignement = Vector3.Dot(testDir, desiredDir);
                if (alignement > maxAlignement)
                {
                    maxAlignement = alignement;
                    bestDir = testDir;
                    foundClearPath = true;
                }
            }
        }

        if (!foundClearPath)
        {
            return Vector3.zero;
        }
        return bestDir.normalized;
    }

    private Vector3 SmartObstacleAvoidance(Vector3 moveDir)
    {
        RaycastHit rayHit;
        float verticalOffset = 0.5f;
        float sphereCastDist = 2f;
        Vector3 castOrigin = transform.position + Vector3.up* verticalOffset;

        if(Physics.SphereCast(castOrigin, sphereCastRadius, moveDir,out rayHit,sphereCastDist, obstacleLayer))
        {
            if(rayHit.normal.y < 0.1f)
            {
                Vector3 project = Vector3.ProjectOnPlane(moveDir, rayHit.normal);
                Vector3 projectNormalized = project.normalized;
                if(project.sqrMagnitude<0.01f || Physics.SphereCast(castOrigin, sphereCastRadius
                    , projectNormalized,out RaycastHit secondRayHit, sphereCastDist, obstacleLayer))
                {
                    return ObstacleAvoidanceWhiskersLogic(moveDir);
                }

                return projectNormalized;
            }
        }
        return moveDir;
    }
}
