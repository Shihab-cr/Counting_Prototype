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

    [SerializeField] private TextMeshProUGUI coinsFromSheepTXT;
    [SerializeField] private TextMeshProUGUI timeBonusTXT;
    [SerializeField] private TextMeshProUGUI totalMoneyEarnedTXT;

    [SerializeField] private TextMeshProUGUI timer;

    private AudioSource audioSource;

    private int timeMinutes =0;
    private int timeSeconds =0;
    private int totalCoinsEarned =0;
    private int totalSheepCoins = 0;
    private int timeCoinBonus = 0;

    [SerializeField] private int bonusPerSecondSaved = 2;
    [SerializeField] private int sheepEntryTime = 20;
    private void Start()
    {
        if (levelCompleteUI != null) levelCompleteUI.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        maxCattle = PlayerPrefs.GetInt("MaxSheepCount",5);

        score.text = "Cattle: " + cattleCount + "/" + maxCattle;

        InvokeRepeating("IncrementTime", 0, 1);
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
            totalSheepCoins += sheep.sheepRewardValue;
            PlayerPrefs.SetInt("Coins", currCoins);
            PlayerPrefs.Save();
        }
    }

    private void HandleLevelComplete()
    {
        if (cattleCount == maxCattle)
        {
            CancelInvoke("IncrementTime");

            CalculateTimeBonus();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            int currCoins = PlayerPrefs.GetInt("Coins", 0);
            currCoins += timeCoinBonus;
            PlayerPrefs.SetInt("Coins", currCoins);
            PlayerPrefs.SetInt("HighestUnlockedLevel", maxCattle + 1);
            PlayerPrefs.Save();

            totalCoinsEarned = totalSheepCoins + timeCoinBonus;

            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
                
                if (timeBonusTXT != null)
                {
                    timeBonusTXT.text = "Bonus: "+timeCoinBonus.ToString();
                }
                if (coinsFromSheepTXT != null) {
                    coinsFromSheepTXT.text = "Coins: "+totalSheepCoins.ToString();
                }
                if (totalMoneyEarnedTXT != null) {
                    totalMoneyEarnedTXT.text = "Total: "+totalCoinsEarned.ToString();
                }
            }
            Time.timeScale = 0f;
        }
    }

    private void IncrementTime()
    {
        timeSeconds++;
        if (timeSeconds >= 60)
        {
            timeMinutes++;
            timeSeconds %= 60;
        }
        
        if(timer != null) timer.text = timeMinutes.ToString("00")+":"+timeSeconds.ToString("00");
    }
    private void CalculateTimeBonus()
    {
        int totalSecondsTake = timeMinutes * 60 + timeSeconds;

        int expectedLevelTime = maxCattle * sheepEntryTime;

        int secondsSaved = expectedLevelTime - totalSecondsTake;

        if (secondsSaved > 0) {
            timeCoinBonus = secondsSaved * bonusPerSecondSaved;
        }
        else
        {
            timeCoinBonus = 0;
        }
    }
}
