using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup Music;
    public AudioMixerGroup Effects;
    public Slider musicVolume;
    public Slider sfxVolume;
    public AudioSource clickButton;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);

        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            sfxVolume.value = PlayerPrefs.GetFloat("SfxVolume");
        }
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume.value);
    }
    public void ChangeVolumeMusic()
    {
        Music.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, musicVolume.value));
        PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);
    }
    public void ChangeVolumeSfx()
    {
        Effects.audioMixer.SetFloat("SfxVolume", Mathf.Lerp(-80, 0, sfxVolume.value));
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume.value);
    }
    public void ClickButtonSound()
    {
        clickButton.Play();  
    }
}
