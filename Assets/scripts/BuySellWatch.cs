using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG;

public class BuySellWatch : ShopButtonsManager
{
    public TextMeshProUGUI coinText; // Посилання на TextMeshPro для відображення кількості монеток
    private List<SkinData> skinDatas;
    public AllSkinsPanel allSkinsPanel;
    public SkinData _skinData;

    [SerializeField] private GameObject playerParent;
    private int coinsCollected = 0; // Грошовий баланс гравця


    // Подписываемся на событие открытия рекламы в OnEnable
    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    // Отписываемся от события открытия рекламы в OnDisable
    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;


    private void Start()
    {
        
        skinDatas = new List<SkinData>();
        // При запуску гри завантажте грошовий баланс гравця збережену в PlayerPrefs (якщо вона є)
        if (PlayerPrefs.HasKey("Coins"))
        {
            coinsCollected = PlayerPrefs.GetInt("Coins");

        }

        // Оновіть відображення кількості монеток на початку гри
        UpdateCoinText();
    }

    private void Update()
    {
        UpdateCoinText();
        if (_skinData.SkinId == PlayerPrefs.GetInt("PlayerSkinId"))
            HideButton();
    }

    public void UpdateSkinData(SkinData newSkinData)
    {
        _skinData = newSkinData;
    }

    public void Own()
    {
        PlayerPrefs.SetInt("PlayerSkinId", PlayerPrefs.GetInt("LastSelected"));
        Debug.Log("Skin selected " + PlayerPrefs.GetInt("LastSelected") + " " + _skinData.SkinId);
        HideButton();
    }

    public void Buy()
    {
        // Отримайте обраного скіна
        SkinData selectedSkin = skinDatas.Find(skin => skin.SkinId == PlayerPrefs.GetInt("LastSelected"));
            // Перевірте, чи гравець має достатньо монеток для покупки
        if (coinsCollected >= _skinData.skinPrice)
        {
            // Витратити монетки та оновити грошовий баланс
            coinsCollected -= _skinData.skinPrice;
            PlayerPrefs.SetInt("Coins", coinsCollected);
            // Виконати операції з покупкою, наприклад, встановити власність гравця для скіну.
            _skinData.skinAvailable = true;
            PlayerPrefs.SetInt("OwnedSkin" + PlayerPrefs.GetInt("LastSelected"), 1); // Збережіть інформацію про власність
            PlayerPrefs.Save();
            YandexGame.savesData.isBuyed.Add(_skinData.SkinId);
            YandexGame.SaveProgress();
            // Оновити відображення кількості монеток
            UpdateCoinText();
            SaveCoins();
            HideButton();
        }
        else
        {
            // Повідомте гравця про недостатню кількість монеток для покупки
            Debug.Log("Недостатньо монеток для покупки цього скіна.");
            
        }
    }

    public void Watch()
    {
        // Код для перегляду відео та отримання скіна
        // Ось приклад:
        
        YandexGame.savesData.isBuyed.Add(_skinData.SkinId);
        YandexGame.SaveProgress();
        HideButton();
    }

    private void UpdateCoinText()
    {
        // Оновіть текст у TextMeshPro для відображення кількості монеток
        if (coinText != null)
        {
            coinText.text = coinsCollected.ToString();
        }
    }

    void Rewarded(int id)
    {
            Watch();
    }

    // Метод для вызова видео рекламы
    public void OpenRewardAd()
    {
        // Вызываем метод открытия видео рекламы
        YandexGame.RewVideoShow(0);
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt("Coins", coinsCollected);
        YandexGame.savesData.money = coinsCollected;
        PlayerPrefs.Save();
        YandexGame.SaveProgress();
    }

    public void closeIf()
    {
        if(PlayerPrefs.GetInt("OwnedSkin" + PlayerPrefs.GetInt("LastSelected"))!=1 || !YandexGame.savesData.isBuyed.Contains(_skinData.SkinId))
        {
            playerParent.SetActive(true);
            SkinsConfig skinConfig = MenuDataManager.Instance.SkinsConfig;
            SetSkin(skinConfig.Skins[PlayerPrefs.GetInt("PlayerSkinId") - 1]);
            PlayerPrefs.SetInt("LastSelected", 0);
        }

        if (PlayerPrefs.GetInt("OwnedSkin" + PlayerPrefs.GetInt("LastSelected")) == 1 || YandexGame.savesData.isBuyed.Contains(_skinData.SkinId) || _skinData.skinAvailable)
        {
            playerParent.SetActive(true);
            PlayerPrefs.SetInt("LastSelected", 0);
        }
    }

    public void SetSkin(SkinData skinData)
    {

        _skinData = skinData;

        if (_skinData == null)
            return;

        if (playerParent.transform.childCount > 0)
            Destroy(playerParent.transform.GetChild(0).gameObject);

        Instantiate(_skinData.SkinModelPrefab, playerParent.transform);
    }
}
