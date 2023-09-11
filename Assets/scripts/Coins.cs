using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class Coins : MonoBehaviour
{
    // Подписываемся на событие GetDataEvent в OnEnable
    private void OnEnable() => YandexGame.GetDataEvent += LoadDataCoins;

    // Отписываемся от события GetDataEvent в OnDisable
    private void OnDisable() => YandexGame.GetDataEvent -= LoadDataCoins;

    public TextMeshProUGUI coinText; // Посилання на TextMeshPro для відображення кількості монеток

    public int coinsCollected = 0;
    public int startCoins = 0;

    private void Start()
    {
        // При запуску гри завантажте кількість монеток збережену в PlayerPrefs (якщо вона є)
        if (PlayerPrefs.HasKey("Coins"))
        {
            coinsCollected = PlayerPrefs.GetInt("Coins");
        }
        


        // Оновіть відображення кількості монеток на початку гри
        UpdateCoinText();
    }

    public void CollectCoin()
    {
        coinsCollected++;
        startCoins++;
        // Оновіть відображення кількості монеток після кожного збору монетки
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
        // Оновіть текст у TextMeshPro для відображення кількості монеток
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
