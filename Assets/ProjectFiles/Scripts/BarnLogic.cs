using TMPro;
using UnityEngine;

public class BarnLogic : MonoBehaviour
{
    public int cattleCount = 0;
    public int maxCattle = 10;

    [SerializeField] private TextMeshProUGUI score;

    private void Start()
    {
        score.text = "Cattle: " + cattleCount + "/" + maxCattle;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cattle"))
        {
            cattleCount++;
            score.text = "Cattle: " + cattleCount + "/" + maxCattle;
            other.gameObject.SetActive(false);
            if(cattleCount == maxCattle)
            {
                Debug.Log("You win!");
            }
        }
    }
}
