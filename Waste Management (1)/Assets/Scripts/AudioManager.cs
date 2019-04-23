using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string masterVolKey;
    [SerializeField] private string musicVolKey;
    [SerializeField] private string sfxVolKey;

    [SerializeField] private Slider masterVolSlider;
    [SerializeField] private Slider musicVolSlider;
    [SerializeField] private Slider sfxVolSlider;


    private void OnEnable()
    {
      
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSoundData();
        gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        SaveSoundData();
    }

    public void SetMasterVolume(float vol)
    {
        masterMixer.SetFloat(masterVolKey, vol);
    }

    public void SetMusicVolume(float vol)
    {
        masterMixer.SetFloat(musicVolKey, vol);
    }

    public void SetSfxVolume(float vol)
    {
        masterMixer.SetFloat(sfxVolKey, vol);
    }

    private void SaveSoundData()
    {
        float tempVol;

        masterMixer.GetFloat(masterVolKey, out tempVol);
        PlayerPrefs.SetFloat(masterVolKey, tempVol);

        masterMixer.GetFloat(musicVolKey, out tempVol);
        PlayerPrefs.SetFloat(musicVolKey, tempVol);

        masterMixer.GetFloat(sfxVolKey, out tempVol);
        PlayerPrefs.SetFloat(sfxVolKey, tempVol);
        Debug.Log("Data saved");
    }

    private void LoadSoundData()
    {
        masterVolSlider.value = PlayerPrefs.GetFloat(masterVolKey);
        musicVolSlider.value = PlayerPrefs.GetFloat(musicVolKey);
        sfxVolSlider.value = PlayerPrefs.GetFloat(sfxVolKey);
    }
}
