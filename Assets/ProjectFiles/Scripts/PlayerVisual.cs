using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BarkController barkController;

    private PlayerController playerController;
    private Rigidbody rb;
    
    private float Speed_f;
    private float maxSpeed;
    [SerializeField] private float timeToSit = 8f; // Time in seconds before the character sits down when idle
    private float idleTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        if(playerController != null)
        {
            maxSpeed = playerController.GetRunningVelocity();
        }

        Speed_f = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        animator.SetFloat("Speed_f", Speed_f);

    }

    // Update is called once per frame
    void Update()
    {

        Speed_f = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        animator.SetFloat("Speed_f", Speed_f);

        Vector3 playerIntent = playerController.GetMovementDir();
        if (playerIntent.magnitude <= 0.1f) {
            idleTimer += Time.deltaTime;
            if (idleTimer >= timeToSit)
            {
                animator.SetBool("Sit_b", true);
                
            }
        }
        else
        {
            animator.SetBool("Sit_b", false);
            idleTimer = 0f;
        }

        if (barkController.GetIsBarking())
        {
            animator.SetBool("Bark_b", true);
        }
        else
        {
            animator.SetBool("Bark_b", false); 
        }
    }
}
