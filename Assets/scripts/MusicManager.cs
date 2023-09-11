using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource SFXSource;

    public AudioClip bg;
    public AudioClip death;

    private bool musicMuted = false;
    private bool sfxMuted = false;

    private void Awake()
    {
        // Завантаження стану вимкнення звуків з PlayerPrefs або використання значень за замовчуванням
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;
    }

    private void Start()
    {
        

        // Встановлення початкового стану звуків
        musicSource.clip = bg;
        musicSource.mute = musicMuted;
        SFXSource.mute = sfxMuted;
        if(SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2)
            musicSource.Play();
    }

    public void ToggleMusic()
    {
        musicMuted = !musicMuted;
        musicSource.mute = musicMuted;

        // Зберігаємо стан музики в PlayerPrefs
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX()
    {
        sfxMuted = !sfxMuted;
        SFXSource.mute = sfxMuted;

        // Зберігаємо стан звуків в PlayerPrefs
        PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsMusicMuted()
    {
        return musicMuted;
    }

    public bool IsSFXMuted()
    {
        return sfxMuted;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
