using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class SpawnerLogic : MonoBehaviour
{
    [SerializeField] private GameObject[] Cattle;
    [SerializeField] private Transform[] Spawns;

    [SerializeField]private Vector2 spawnLimits = new Vector2(-15,15);
    [SerializeField] private Vector2 sheepSizeRange = new Vector2(0.6f, 1.8f);
    private int maxSheepCount;
    private List<SheepBrain> spawnedSheep = new List<SheepBrain>();

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxSheepCount = PlayerPrefs.GetInt("MaxSheepCount", 5);

        while (Cattle.Length > 0 && maxSheepCount>0)
        {
            int spawnIndex = Random.Range(0,Spawns.Length);
            int localMaxSheepCount;
            if (maxSheepCount/2 == 0 || maxSheepCount/2 == 1)
            {
                localMaxSheepCount = maxSheepCount;
                maxSheepCount = 0;
            }
            else
            {
                localMaxSheepCount = maxSheepCount - Random.Range(1, (maxSheepCount / 2)+1);
                maxSheepCount -= localMaxSheepCount;
            }

            for(int i=0;i<localMaxSheepCount;i++)
            {
                int sheepIndex = Random.Range(0, Cattle.Length);
                
                Vector2 offset = new Vector2(Random.Range(spawnLimits.x, spawnLimits.y),Random.Range(spawnLimits.x,spawnLimits.y));
                Vector3 spawnPos = Spawns[spawnIndex].position + new Vector3(offset.x,0f,offset.y);
                
                GameObject cattle = Instantiate(Cattle[sheepIndex], spawnPos, Quaternion.identity);

                float sheepSize = Random.Range(sheepSizeRange.x, sheepSizeRange.y);
                cattle.transform.localScale = new Vector3(sheepSize, sheepSize,sheepSize);
                
                spawnedSheep.Add(cattle.GetComponent<SheepBrain>());
                
            }

            float maxScale = 0;
            SheepBrain biggestSheep = null;
            foreach(SheepBrain animal in spawnedSheep)
            {
                if (animal.canBeLeader)
                {
                    float animalSize = animal.transform.localScale.magnitude;
                    if (animalSize > maxScale)
                    {
                        maxScale = animalSize;
                        biggestSheep = animal;
                    }
                }
            }
            if(biggestSheep != null)
            {
                biggestSheep.isLeader = true;
            }
            spawnedSheep.Clear();
        }
    }
}
