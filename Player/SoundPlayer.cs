using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance ;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private AudioSource audioSourceChop;
    [SerializeField] private AudioSource audioSourceWatering;
    [SerializeField] private AudioSource audioSourceHoe;

    public void PlaySoundChop()
    {
        audioSourceChop.volume = SettingManager.instance.soundVolume / SettingManager.instance.maxVolume;
        if(!audioSourceChop.isPlaying)
        audioSourceChop.Play();
    }
    public void PlaySoundWatering()
    {
        audioSourceChop.volume = SettingManager.instance.soundVolume / SettingManager.instance.maxVolume;
        if(!audioSourceWatering.isPlaying)
        audioSourceWatering.Play();
    }
    public void PlaySoundHoe()
    {
        audioSourceChop.volume = SettingManager.instance.soundVolume / SettingManager.instance.maxVolume;
        if(!audioSourceHoe.isPlaying)
        audioSourceHoe.Play();
    }
}
