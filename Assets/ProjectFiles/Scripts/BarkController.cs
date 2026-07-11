using System.Collections;
using UnityEngine;

public class BarkController : MonoBehaviour
{
    private SphereCollider barkCollider;
    public float barkDuration = 5f;
    public float barkCooldown = 0.5f;
    public float barkRadius = 10f;

    private const float barkSpeed = 360f;

    private bool isBarking = false;
    public float fearLevel = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        barkCollider = GetComponent<SphereCollider>();

        barkCollider.radius = 0f;
        barkCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space) && !isBarking)
        {
            StartCoroutine(BarkWave());
        }
    }

    private IEnumerator BarkWave()
    {
        isBarking = true;
        barkCollider.enabled = true;
        while (barkCollider.radius < barkRadius)
        {
            barkCollider.radius += barkSpeed * Time.deltaTime;
            yield return null;
        }
        
        barkCollider.radius = barkRadius;
        yield return null;
        barkCollider.enabled = false;

        yield return new WaitForSeconds(barkCooldown);
        /*while (barkCollider.radius > 0)
        {
            barkCollider.radius -= barkSpeed * Time.deltaTime;
            yield return null;
        }*/
        barkCollider.radius = 0f;
        isBarking = false;
    }

    private void OnDrawGizmos()
    {
        // This only runs in the Unity Editor Scene View!
        if (barkCollider != null && barkCollider.enabled)
        {
            // Set the color of our drawing pen to Red
            Gizmos.color = Color.red;
            
            // Draw a wireframe sphere exactly matching our collider's radius
            Gizmos.DrawWireSphere(transform.position, barkCollider.radius);
        }
    }
    public bool GetIsBarking()
    {
        return isBarking;
    }
}
