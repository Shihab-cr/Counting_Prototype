using System.Collections;
using UnityEngine;

public class SheepBrain : MonoBehaviour
{
    private bool isLeader = false;
    
    public float panicThreshold = 30f;
    public float hungerLevel = 100f;
    public float hungerThreshold = 50f;
    public float hungerDrainRate = 3f;
    public float eatingRate = 5f;
    public float targetHungerLevel;
    public float safeDistance = 15f;
    public float fearDecayRate = 5f;

    private float currHungerLevel;
    private float currFearLevel;
    private float stateTime;

    private GameObject player;
    

    [SerializeField] private Vector2 wanderTimeBoundaries = new Vector2(5f, 15f);
    [SerializeField] private Vector2 idleTimeBoundaries = new Vector2(2f, 5f);
    [SerializeField] private Vector2 panicTimeBoundaries = new Vector2(5f, 10f);

    public enum SheepState
    {
        Idle,
        Fleeing,
        Wandering,
        Grazing,
        Fighting,
        Panicking
    }
    
    public SheepState currentState = SheepState.Idle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        stateTime = Random.Range(idleTimeBoundaries.x, idleTimeBoundaries.y); //initalize the stateTimer for idle state
        currHungerLevel = hungerLevel;
        currFearLevel = 0f;
    
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case SheepState.Idle:
                // Implement Idle behavior
                DecreaseHunger();
                IdleTimer();
                break;
            case SheepState.Fleeing:
                // Implement Fleeing behavior
                decreaseFear();
                break;
            case SheepState.Wandering:
                // Implement Wandering behavior
                WanderTimer();
                DecreaseHunger();
                break;
            case SheepState.Grazing:
                // Implement Grazing behavior
                IncreaseHunger();
                break;
            case SheepState.Fighting:
                // Implement Fighting behavior
                break;
            case SheepState.Panicking:
                // Implement Panicking behavior
                HandlePanic();
                break;
        }    
    }

    private void DecreaseHunger()
    {
        currHungerLevel -= hungerDrainRate * Time.deltaTime;
        float offset = 10f;

        if(currHungerLevel < hungerThreshold)
        {
            currentState = SheepState.Grazing;
            targetHungerLevel = Random.Range(hungerThreshold+offset, hungerLevel);
        }
    }
    private void IncreaseHunger()
    {
        currHungerLevel += eatingRate * Time.deltaTime;
        if(currHungerLevel>= targetHungerLevel)
        {
            currentState = SheepState.Wandering;
            stateTime = Random.Range(wanderTimeBoundaries.x, wanderTimeBoundaries.y); // Set a random wandering duration
        }
    }
    private void IdleTimer()
    {
        stateTime -= Time.deltaTime;
        if(stateTime <= 0f)
        {
            currentState = SheepState.Wandering;
            stateTime = Random.Range(wanderTimeBoundaries.x, wanderTimeBoundaries.y); // Reset the timer for wandering
        }
    }

    private void WanderTimer()
    {
        stateTime -= Time.deltaTime;
        if(stateTime <= 0f)
        {
            currentState = SheepState.Idle;
            stateTime = Random.Range(idleTimeBoundaries.x, idleTimeBoundaries.y); // Reset the timer for idle
        }
    }
    
    private void decreaseFear()
    {
        Transform playerTransform = player.transform;
        if (Vector3.Distance(transform.position, playerTransform.position) > safeDistance)
        {
            currFearLevel -= fearDecayRate * Time.deltaTime;
            if (currFearLevel <= 0f)
            {
                currFearLevel = 0f;
                currentState = SheepState.Idle;
                stateTime = Random.Range(idleTimeBoundaries.x, idleTimeBoundaries.y); // Reset the timer for idle
            }
        }
    }
    private void HandlePanic()
    {
        stateTime -= Time.deltaTime;
        if (stateTime <= 0f)
        {
            currentState = SheepState.Fleeing;
        }

    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FearZone"))
        {
            BarkController barkController = other.GetComponent<BarkController>();
            if (barkController != null)
            {
                currFearLevel += barkController.fearLevel;
                if (currFearLevel >= panicThreshold)
                {
                    currentState = SheepState.Panicking;
                    stateTime = Random.Range(panicTimeBoundaries.x, panicTimeBoundaries.y); // Set a random panic duration
                }
                else
                {
                    currentState = SheepState.Fleeing;
                }
            }
        }
    }
}
