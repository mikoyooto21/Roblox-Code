using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlock : MonoBehaviour
{
    public GameObject unlockedLevelButton; // Посилання на кнопку рівня з таймером
    public GameObject lockedLevelButton; // Посилання на кнопку рівня без таймеру

    private void Start()
    {
        // Перевірте, чи рівень розблокований і активуйте відповідну кнопку
        if (PlayerPrefs.HasKey("UnlockedLevelWithTimer") && PlayerPrefs.GetInt("UnlockedLevelWithTimer") == 1)
        {
            unlockedLevelButton.SetActive(true); // Рівень розблоковано
            lockedLevelButton.SetActive(false); // Рівень без таймеру неактивний
        }
        else
        {
            unlockedLevelButton.SetActive(false); // Рівень розблоковано
            lockedLevelButton.SetActive(true); // Рівень без таймеру активний
        }
    }
}
