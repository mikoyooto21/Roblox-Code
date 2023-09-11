using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSwitcher : MonoBehaviour
{
    [SerializeField] GameObject musicOn;
    [SerializeField] GameObject musicOff;
    [SerializeField] GameObject sfxOn;
    [SerializeField] GameObject sfxOff;
    [SerializeField] MusicManager musicManager;

    void Start()
    {
        // Оновлюємо стан кнопок згідно зі збереженими налаштуваннями
        UpdateSoundButtons();
    }

    public void MusicSwitch()
    {
        musicManager.ToggleMusic(); // Включити / вимкнути музику
        UpdateSoundButtons();
    }

    public void SfxSwitch()
    {
        musicManager.ToggleSFX(); // Включити / вимкнути звуки
        UpdateSoundButtons();
    }

    private void UpdateSoundButtons()
    {
        bool musicMuted = musicManager.IsMusicMuted();
        bool sfxMuted = musicManager.IsSFXMuted();

        musicOn.SetActive(!musicMuted);
        musicOff.SetActive(musicMuted);
        sfxOn.SetActive(!sfxMuted);
        sfxOff.SetActive(sfxMuted);
    }
}
