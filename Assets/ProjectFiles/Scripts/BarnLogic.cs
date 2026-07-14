using TMPro;
using UnityEngine;

public class BarnLogic : MonoBehaviour
{
    public int cattleCount = 0;
    public int maxCattle = 10;

    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject levelCompleteUI;
    [SerializeField] private AudioClip[] sfx;
    [SerializeField] private GameObject[] vfx;

    private AudioSource audioSource;
    private void Start()
    {
        if (levelCompleteUI != null) levelCompleteUI.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        maxCattle = PlayerPrefs.GetInt("MaxSheepCount",5);

        score.text = "Cattle: " + cattleCount + "/" + maxCattle;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cattle"))
        {
            cattleCount++;
            score.text = "Cattle: " + cattleCount + "/" + maxCattle;
            UpdatePlayerCoins(other);
            if (sfx.Length > 0)
            {
                int sfxIndex = Random.Range(0, sfx.Length);
                AudioClip clipToPlay = sfx[sfxIndex];
                if(audioSource != null)
                    audioSource.PlayOneShot(clipToPlay);
            }
            if(vfx.Length > 0)
            {
                int vfxIndex = Random.Range(0, vfx.Length);
                GameObject vfxToDisplay = vfx[vfxIndex];
                Instantiate(vfxToDisplay,other.transform.position,Quaternion.identity);
            }
            
            other.gameObject.SetActive(false);
            HandleLevelComplete();
        }
    }

    private void UpdatePlayerCoins(Collider other)
    {
            SheepBrain sheep = other.GetComponent<SheepBrain>();
        if (sheep != null)
        {
            int currCoins = PlayerPrefs.GetInt("Coins", 0);
            currCoins += sheep.sheepRewardValue;
            PlayerPrefs.SetInt("Coins", currCoins);
            PlayerPrefs.Save();
        }
    }

    private void HandleLevelComplete()
    {
        if (cattleCount == maxCattle)
        {
            if (levelCompleteUI != null)
                levelCompleteUI.SetActive(true);
        }
    }
}
