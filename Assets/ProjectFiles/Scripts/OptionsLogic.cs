using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using UnityEditor.Recorder.Input;
public class OptionsLogic : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider animalsVolume;
    [SerializeField] private Slider backgroundVolume;
    [SerializeField] private Slider cameraSensitivity;
    [SerializeField] private Slider brightnessLevel;

    [Header("System References")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixer animalMixer;
    [SerializeField] private AudioMixer backgroundMixer;
    [SerializeField] private Volume essentialVolume;
    [SerializeField] private Volume expensiveVolume;

    [Header("Cinemachine freelook cam")]
    [SerializeField] private CinemachineInputAxisController freeLookCamController;

    [Header("Base camera speed")]
    [SerializeField] private float baseSpeedX = 100f;
    [SerializeField] private float baseSpeedY = 1f;

    private ColorAdjustments colorAdjustments;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (essentialVolume != null) {
            essentialVolume.profile.TryGet(out colorAdjustments);
        }
        LoadMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume",masterVolume.value);
        PlayerPrefs.SetFloat("AnimalsVolume",animalsVolume.value);
        PlayerPrefs.SetFloat("BackgroundVolume",backgroundVolume.value);
        PlayerPrefs.SetFloat("CameraSensitivity", cameraSensitivity.value);
        PlayerPrefs.SetFloat("Brightness", brightnessLevel.value);
        PlayerPrefs.Save();
    }
    public void LoadMenu()
    {
        masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        animalsVolume.value = PlayerPrefs.GetFloat("AnimalsVolume", 1);
        backgroundVolume.value = PlayerPrefs.GetFloat("BackgroundVolume", 1);
        cameraSensitivity.value = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
        brightnessLevel.value = PlayerPrefs.GetFloat("Brightness", 0);

        SetMainMixerVol();
        SetAnimalsVolume();
        SetBackgroundVolume();
        SetBrightness();
    }

    public void SetMainMixerVol()
    {
        mainMixer.SetFloat("MasterVol", Mathf.Log10(masterVolume.value) * 20);
    }
    public void SetAnimalsVolume()
    {
        animalMixer.SetFloat("AnimalsVol", Mathf.Log10(animalsVolume.value) * 20);
    }
    public void SetBackgroundVolume()
    {
        backgroundMixer.SetFloat("BackgroundVol", Mathf.Log10(backgroundVolume.value) * 20);
    }
    public void SetBrightness()
    {
        if(colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = brightnessLevel.value;
        }
    }
    public void ApplySensitivity()
    {
        if (freeLookCamController == null) return;

        float sensitivityMultiplier = PlayerPrefs.GetFloat("CameraSensitivity", 1f);

        if(freeLookCamController.Controllers.Count > 0)
        {
            freeLookCamController.Controllers[0].Input.Gain = baseSpeedX * sensitivityMultiplier;
        }
        if (freeLookCamController.Controllers.Count > 1)
        {
            freeLookCamController.Controllers[1].Input.Gain = baseSpeedY * sensitivityMultiplier;
        }
    }

    public void ToggleExpensivePostProcessing(bool value)
    {
        expensiveVolume.gameObject.SetActive(value);
    }
}
