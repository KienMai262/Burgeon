using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private GameObject settingMusicVolume;
    [SerializeField] private GameObject settingSoundVolume;
    [SerializeField] private GameObject settingRainEffect;
    [SerializeField] private GameObject settingDarkSkyEffect;

    [SerializeField] protected AudioSource cameraSound;

    public float maxVolume = 15;
    public float musicVolume;
    public float soundVolume;

    private void Start()
    {
        musicVolume = 15;
        soundVolume = 15;
    }
    public void IncreaseMusic()
    {
        if (musicVolume < maxVolume)
        {
            musicVolume++;
            cameraSound.volume = musicVolume / maxVolume;
            Debug.Log("Music Volume: " + musicVolume);
            for (int i = 0; i < settingMusicVolume.transform.childCount; i++)
            {
                if (!settingMusicVolume.transform.GetChild(i).gameObject.activeSelf)
                {
                    settingMusicVolume.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
    public void DecreaseMusic()
    {
        if (musicVolume > 0)
        {
            musicVolume--;
            cameraSound.volume = musicVolume / maxVolume;
            for (int i = settingMusicVolume.transform.childCount - 1; i >= 0; i--)
            {
                if (settingMusicVolume.transform.GetChild(i).gameObject.activeSelf)
                {
                    settingMusicVolume.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
    public void IncreaseSound()
    {
        if (soundVolume < maxVolume)
        {
            soundVolume++;
            for (int i = 0; i < settingSoundVolume.transform.childCount; i++)
            {
                if (!settingSoundVolume.transform.GetChild(i).gameObject.activeSelf)
                {
                    settingSoundVolume.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
    public void DecreaseSound()
    {
        if (soundVolume > 0)
        {
            soundVolume--;
            for (int i = settingSoundVolume.transform.childCount - 1; i >= 0; i--)
            {
                if (settingSoundVolume.transform.GetChild(i).gameObject.activeSelf)
                {
                    settingSoundVolume.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    public void ToggleRainEffect()
    {
        settingRainEffect.SetActive(!settingRainEffect.activeSelf);
    }
    public void ToggleDarkSkyEffect()
    {
        settingDarkSkyEffect.SetActive(!settingDarkSkyEffect.activeSelf);
    }
}
