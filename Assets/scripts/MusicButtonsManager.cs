using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicButtonsManager : MonoBehaviour
{
    [SerializeField] GameObject musicOn;
    [SerializeField] GameObject musicOff;
    [SerializeField] GameObject sfxOn;
    [SerializeField] GameObject sfxOff;

    public void music()
    {
        if(PlayerPrefs.GetInt("MusicMuted") == 0)
        {
            musicOff.SetActive(true);
            musicOn.SetActive(false);
            PlayerPrefs.SetInt("MusicMuted", 1);
            PlayerPrefs.Save();
        }
        else
        {
            musicOn.SetActive(true);
            musicOff.SetActive(false);
            PlayerPrefs.SetInt("MusicMuted", 0);
            PlayerPrefs.Save();
        }
    }

    public void sfx()
    {
        if (PlayerPrefs.GetInt("SfxMuted") == 0)
        {
            sfxOff.SetActive(true);
            sfxOn.SetActive(false);
            PlayerPrefs.SetInt("SfxMuted", 1);
            PlayerPrefs.Save();
        }
        else
        {
            sfxOn.SetActive(true);
            sfxOff.SetActive(false);
            PlayerPrefs.SetInt("SfxMuted", 0);
            PlayerPrefs.Save();
        }
    }
}
