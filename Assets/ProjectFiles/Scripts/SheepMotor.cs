using System.Linq.Expressions;
using UnityEngine;

public class SheepMotor : MonoBehaviour
{
    private Rigidbody rb;
    private SheepBrain sheepBrain;

    private GameObject player;

    private Vector3 movementDir = Vector3.zero;
    private Vector3 previousDir = Vector3.zero;


    [SerializeField] private float idleVelocity =0f;
    [SerializeField] private float wanderVelocity = 2f;
    [SerializeField] private float fleeVelocity = 8f;
    [SerializeField] private float panicVelocity = 14f;
    [SerializeField] private float accelerationRate = 75f;
    [SerializeField] private float wanderRotationSpeed = 5f; // Slow, lazy turning
    [SerializeField] private float runRotationSpeed = 80f;

    private bool isMoving = false;
    private bool canSwitchDir = true;
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
            previousDir = movementDir;
        }
        Vector3 targetVelocityVector = movementDir * targetVelocity;
        // Preserve the current vertical velocity (e.g., gravity)
        targetVelocityVector.y = rb.linearVelocity.y;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocityVector
            , accelerationRate * Time.fixedDeltaTime);
        // Rotate the sheep to face the movement direction
       
        Quaternion targetRotation = Quaternion.LookRotation(previousDir);
        float currentRotationSpeed = (sheepBrain.currentState == SheepBrain.SheepState.Wandering) ? wanderRotationSpeed : runRotationSpeed;
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, currentRotationSpeed * Time.fixedDeltaTime);
        
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
}
