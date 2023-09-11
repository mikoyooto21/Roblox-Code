using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG;

public class BestTime : MonoBehaviour
{
    [SerializeField] Image TimeImg;
    [SerializeField] TextMeshProUGUI timeText;

    private void OnEnable() => YandexGame.GetDataEvent += GetLoadTime;

    // Отписываемся от события GetDataEvent в OnDisable
    private void OnDisable() => YandexGame.GetDataEvent -= GetLoadTime;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("time"))
        {
            TimeImg.gameObject.SetActive(true);
            timeText.text = PlayerPrefs.GetFloat("time").ToString();
        }
        if (YandexGame.SDKEnabled == true)
        {
            // Если запустился, то выполняем Ваш метод для загрузки
            GetLoadTime();

            // Если плагин еще не прогрузился, то метод не выполнится в методе Start,
            // но он запустится при вызове события GetDataEvent, после прогрузки плагина
        }
    }

    public void GetLoadTime()
    {
        if (YandexGame.savesData.time > 0)
        {
            float minutes = Mathf.FloorToInt(YandexGame.savesData.time / 60);
            float seconds = Mathf.FloorToInt(YandexGame.savesData.time % 60);

            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
    }
}
