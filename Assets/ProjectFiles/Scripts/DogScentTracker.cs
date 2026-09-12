using System.Collections;
using UnityEngine;

public class DogScentTracker : MonoBehaviour
{
    [SerializeField] private GameObject particlesParent;
    [SerializeField] private ParticleSystem sniffParticles;
    [SerializeField] private float moveTime = 1;
    [SerializeField] private float moveDist = 3;
    [SerializeField] private float sniffCoolDown =3;
    [SerializeField] private float emissionRateOverDistance = 20f;
    [SerializeField] private float verticalOffset = 0.2f;
    [SerializeField] private float displayDuration = 0.04f;
    private GameObject[] cattles;
    private GameObject closestCattle;

    private float lastsniffTime = -10f;
    public void HandleSniff()
    {
        if(Time.time < lastsniffTime + sniffCoolDown)
        {
            return;
        }
        lastsniffTime = Time.time;
        SetClosestCattle();
        if (closestCattle != null)
        {
            Vector3 direction = (closestCattle.transform.position -transform.position);
            direction.y = 0;
            direction.Normalize();
            StartCoroutine(DrawIndicator(direction));
        }
    }

    private void SetClosestCattle()
    {
        cattles = GameObject.FindGameObjectsWithTag("Cattle");
        closestCattle = null;
        float minDist = Mathf.Infinity;
        float currDist;
        
        foreach(GameObject c in cattles)
        {
            if (c.activeInHierarchy)
            {
                currDist = Vector3.Distance(transform.position, c.transform.position);
                if (minDist > currDist)
                {
                    minDist = currDist;
                    closestCattle = c;
                }
            }
        }

        
    }

    private IEnumerator DrawIndicator(Vector3 direction)
    {
        if (sniffParticles != null) {
            
            var em = sniffParticles.emission;
            em.rateOverTimeMultiplier = 0;
            em.rateOverDistance = emissionRateOverDistance;
            sniffParticles.Play();
        }
        if (particlesParent != null) {
            particlesParent.transform.position = transform.position+Vector3.up*verticalOffset;
        }

        
        
            Vector3 startPos = particlesParent.transform.position;
            Vector3 targetPos = startPos + direction * moveDist;

            float elapsedTime = 0;
            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                float timePercent = elapsedTime / moveTime;
                particlesParent.transform.position = Vector3.Lerp(startPos, targetPos , timePercent);
                yield return null;
            }
        
        yield return new WaitForSeconds(displayDuration);
        if (sniffParticles != null) {
            var em = sniffParticles.emission;
            em.rateOverDistance = 0;
           
        }
        if(particlesParent != null)particlesParent.transform.position = transform.position;
    }
}
