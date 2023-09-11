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

    // ������������ �� ������� GetDataEvent � OnDisable
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
            // ���� ����������, �� ��������� ��� ����� ��� ��������
            GetLoadTime();

            // ���� ������ ��� �� �����������, �� ����� �� ���������� � ������ Start,
            // �� �� ���������� ��� ������ ������� GetDataEvent, ����� ��������� �������
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
