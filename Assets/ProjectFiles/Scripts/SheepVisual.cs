using UnityEngine;

public class SheepVisual : MonoBehaviour
{
    public Animator animator;
    public string walkForwardAnimation = "walk_forward";
    public string walkBackwardAnimation = "walk_backwards";
    public string runForwardAnimation = "run_forward";
    public string turn90LAnimation = "turn_90_L";
    public string turn90RAnimation = "turn_90_R";
    public string trotAnimation = "trot_forward";
    public string sittostandAnimation = "sit_to_stand";
    public string standtositAnimation = "stand_to_sit";

    private SheepBrain sheepBrain;
    private SheepMotor sheepMotor;
    private Rigidbody rb;

    private string speedF = "Speed_f";
    private string atk_t = "Attack";
    private string headShake_t = "ShakeHead";
    private string blink_t = "Blink";
    private string isEating ="IsEating";
    private string isSitting = "IsSitting";
    private string isAlerted = "IsAlerted";
    private string push_L_t = "Push_L_t";
    private string push_R_t = "Push_R_t";

    private float idleTime = 3.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sheepBrain = GetComponent<SheepBrain>();
        sheepMotor = GetComponent<SheepMotor>();
        rb = GetComponent<Rigidbody>();

        InvokeRepeating("Blink", 1f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        SheepBrain.SheepState currState = sheepBrain.currentState;
        animator.SetFloat(speedF,Mathf.Clamp01(rb.linearVelocity.magnitude / sheepMotor.GetMaxSpeed()));
        
        switch(currState)
        {
            case SheepBrain.SheepState.Idle:
                //animator.SetFloat(speedF, 0f);
                animator.SetBool(isEating, false);
                sheepMotor.SetCanMove(false);
                ShakeHead();
                break;
            case SheepBrain.SheepState.Wandering:
                animator.SetFloat(speedF, 0.3f);
                animator.SetBool(isEating, false);
                sheepMotor.SetCanMove(true);
                break;
            case SheepBrain.SheepState.Fleeing:
                //animator.SetFloat(speedF, 0.6f);
                animator.SetBool(isEating, false);
                sheepMotor.SetCanMove(true);
                break;
            case SheepBrain.SheepState.Grazing:
                animator.SetBool(isEating, true);
                sheepMotor.SetCanMove(false);
                break;
            case SheepBrain.SheepState.Panicking:
                //animator.SetFloat(speedF, 1f);
                animator.SetBool(isEating, false);
                sheepMotor.SetCanMove(true);
                break;
            default:
                break;
        }
    }

    private void Blink()
    {
        animator.SetTrigger(blink_t);
    }
    private void ShakeHead()
    {
        if (sheepBrain.currentState == SheepBrain.SheepState.Idle)
        {

            idleTime -= Time.deltaTime;
            if (idleTime <= 0f)
            {
                idleTime = 3.5f;
                animator.SetTrigger(headShake_t);
            }
        }
        else
        {
            idleTime = 3.5f;
        }
    }
}
