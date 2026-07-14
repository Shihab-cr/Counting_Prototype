using TMPro;
using UnityEngine.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera mainMenuCam;
    [SerializeField] private CinemachineCamera levelCam;
    [SerializeField] private CinemachineCamera marketCam;

    [Header("Ui Panels")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject levelMenuUI;
    [SerializeField] private GameObject marketUI;
    [SerializeField] private GameObject optionsUI;

    [Header("Level Buttons")]
    [SerializeField] private Button[] levelBtns;
    [SerializeField] private GameObject[] lockIcons;

    [Header("Money")]
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Player Dog model")]
    [SerializeField] private DogModelVisual dogMarketModel;
    [SerializeField] private DogModelVisual dogMainMenuModel;

    [Header("Market Progress UI")]
    [SerializeField] private TextMeshProUGUI[] dogLabels;

    private int playerCoins;
    private int dogIndex = 0;

    void Start()
    {
        Time.timeScale = 1;   
        LoadPlayerData();
        OpenMainPanel();
        SetUpLevelUnlock();
    }
    private void LoadPlayerData()
    {
        playerCoins = PlayerPrefs.GetInt("Coins", 0);
        if (moneyText != null) moneyText.text = playerCoins.ToString();

        PlayerPrefs.SetInt("Unlocked_dog0", 1);

        if (dogLabels.Length > 0)
        {
            for(int i=0;i<dogLabels.Length; i++)
            {
                if(PlayerPrefs.GetInt("Unlocked_dog"+i.ToString(),0) == 1)
                {

                    dogLabels[i].text = "Unlocked";
                }
            }
        }
        dogIndex = PlayerPrefs.GetInt("CurrDogIndex", 0);
        EquipDog(dogIndex);
        if(dogLabels.Length>0 && dogIndex<dogLabels.Length) dogLabels[dogIndex].text = "Equipped";
    }
    public void SwitchCamera(CinemachineCamera targetCam)
    {
        if(mainMenuCam != null) mainMenuCam.Priority = 0;
        if(levelCam !=null) levelCam.Priority = 0;
        if(marketCam != null) marketCam.Priority = 0;

        if (targetCam != null) targetCam.Priority = 10;
    }

    public void OpenMainPanel()
    {
        if (mainMenuUI != null) mainMenuUI.SetActive(true);
        
        if(marketUI != null) marketUI.SetActive(false);
        if(levelMenuUI != null) levelMenuUI.SetActive(false);
        if(optionsUI != null) optionsUI.SetActive(false);

        SwitchCamera(mainMenuCam);
    }
    public void OpenLevelSelect()
    {
        if (levelMenuUI != null) levelMenuUI.SetActive(true);

        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        if (marketUI != null) marketUI.SetActive(false);
        if (optionsUI != null) optionsUI.SetActive(false);

        SwitchCamera(levelCam);
    }
    public void OpenMarketPanel()
    {
        if (marketUI != null) marketUI.SetActive(true);

        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        if (levelMenuUI != null) levelMenuUI.SetActive(false);
        if (optionsUI != null) optionsUI.SetActive(false);

        SwitchCamera(marketCam);
    }
    public void SetDogIndex(int index)
    {
        dogIndex = index;
    }
    public void AttemptToBuyDog(int price)
    {
        if(PlayerPrefs.GetInt("Unlocked_dog"+dogIndex.ToString(),0) == 1)
        {
            EquipDog(dogIndex);
            return;
        }

        if(playerCoins >= price)
        {
            playerCoins -= price;
            PlayerPrefs.SetInt("Coins", playerCoins);
            PlayerPrefs.SetInt("Unlocked_dog" + dogIndex.ToString(), 1);
            PlayerPrefs.Save();

            dogLabels[dogIndex].text = "Unlocked";

            if (moneyText != null) moneyText.text = playerCoins.ToString();
            EquipDog(dogIndex);
        }
        else
        {
            //indicate insufficient coins
        }
    }
    public void EquipDog(int dogIndex)
    {
        //we will assign index to dogs and use these indices to load the dog we play with
        PlayerPrefs.SetInt("CurrDogIndex",dogIndex);
        dogMarketModel.UpdateDogModel();
        dogMainMenuModel.UpdateDogModel();

        for (int i = 0; i < dogLabels.Length; i++) {
            if (PlayerPrefs.GetInt("Unlocked_dog" + i.ToString(), 0) == 1)
            {
                dogLabels[i].text = "Unlocked";
            }
        }
        dogLabels[dogIndex].text = "Equipped";
    }
    private void SetUpLevelUnlock()
    {
        if (levelBtns.Length <= 0) return;

        int highestLevelUnlocked = PlayerPrefs.GetInt("HighestUnlockedLevel", 1);

        for(int i = 0; i < levelBtns.Length; i++)
        {
            if(i+1 <= highestLevelUnlocked)
            {
                levelBtns[i].interactable = true;
                if(lockIcons.Length>0) lockIcons[i].gameObject.SetActive(false);
            }
            else
            {
                levelBtns[i].interactable = false;
                if(lockIcons.Length>0) lockIcons[i].gameObject.SetActive(true);
            }

        }
    }
    public void LoadLevel(int levelSheepCount)
    {
        PlayerPrefs.SetInt("MaxSheepCount",levelSheepCount);
        SceneManager.LoadSceneAsync("GameScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
