using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class Coins : MonoBehaviour
{
    // ������������� �� ������� GetDataEvent � OnEnable
    private void OnEnable() => YandexGame.GetDataEvent += LoadDataCoins;

    // ������������ �� ������� GetDataEvent � OnDisable
    private void OnDisable() => YandexGame.GetDataEvent -= LoadDataCoins;

    public TextMeshProUGUI coinText; // ��������� �� TextMeshPro ��� ����������� ������� �������

    public int coinsCollected = 0;
    public int startCoins = 0;

    private void Start()
    {
        // ��� ������� ��� ���������� ������� ������� ��������� � PlayerPrefs (���� ���� �)
        if (PlayerPrefs.HasKey("Coins"))
        {
            coinsCollected = PlayerPrefs.GetInt("Coins");
        }
        


        // ������ ����������� ������� ������� �� ������� ���
        UpdateCoinText();
    }

    public void CollectCoin()
    {
        coinsCollected++;
        startCoins++;
        // ������ ����������� ������� ������� ���� ������� ����� �������
        UpdateCoinText();
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt("Coins", coinsCollected);
        YandexGame.savesData.money = coinsCollected;
        PlayerPrefs.Save();
        YandexGame.SaveProgress();
    }

    public void UpdateCoinText()
    {
        // ������ ����� � TextMeshPro ��� ����������� ������� �������
        if (coinText != null)
        {
            coinText.text = coinsCollected.ToString();
        }
    }

    public void LoadDataCoins()
    {
        coinsCollected = YandexGame.savesData.money;

        UpdateCoinText();
    }


}
