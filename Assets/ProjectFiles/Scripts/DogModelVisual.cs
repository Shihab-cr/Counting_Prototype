using UnityEngine;


[System.Serializable]
public struct DogData
{
    public string dogName;
    public GameObject dogModel;
    public AudioClip[] dogClips;
}

public class DogModelVisual : MonoBehaviour
{
    private int dogIndex = 0;
    [Header("Dog Models Prefabs")]
    [SerializeField] private DogData[] dogs;

    private AudioSource audioSource;
    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();

        UpdateDogModel();
    }
    public void UpdateDogModel()
    {
        int savedDogIndex = PlayerPrefs.GetInt("CurrDogIndex", 0);

        if (savedDogIndex >= dogs.Length)
            return;


        dogs[dogIndex].dogModel.SetActive(false);

        dogIndex = savedDogIndex;
        dogs[dogIndex].dogModel.SetActive(true);
    }

    public void PlayBark()
    {
        if (dogs[dogIndex].dogClips.Length>0 && audioSource != null)
        {
            int audioClipIndex = Random.Range(0, dogs[dogIndex].dogClips.Length);
            audioSource.PlayOneShot(dogs[dogIndex].dogClips[audioClipIndex]);
        }
    }
}
