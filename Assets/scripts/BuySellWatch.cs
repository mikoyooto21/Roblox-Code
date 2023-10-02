using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG;

public class BuySellWatch : ShopButtonsManager
{
    public TextMeshProUGUI coinText; // ��������� �� TextMeshPro ��� ����������� ������� �������
    private List<SkinData> skinDatas;
    public AllSkinsPanel allSkinsPanel;
    public SkinData _skinData;

    [SerializeField] private GameObject playerParent;
    private int coinsCollected = 0; // �������� ������ ������


    // ������������� �� ������� �������� ������� � OnEnable
    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    // ������������ �� ������� �������� ������� � OnDisable
    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;


    private void Start()
    {
        
        skinDatas = new List<SkinData>();
        // ��� ������� ��� ���������� �������� ������ ������ ��������� � PlayerPrefs (���� ���� �)
        if (PlayerPrefs.HasKey("Coins"))
        {
            coinsCollected = PlayerPrefs.GetInt("Coins");

        }

        // ������ ����������� ������� ������� �� ������� ���
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
        // ��������� �������� ����
        SkinData selectedSkin = skinDatas.Find(skin => skin.SkinId == PlayerPrefs.GetInt("LastSelected"));
            // ��������, �� ������� �� ��������� ������� ��� �������
        if (coinsCollected >= _skinData.skinPrice)
        {
            // ��������� ������� �� ������� �������� ������
            coinsCollected -= _skinData.skinPrice;
            PlayerPrefs.SetInt("Coins", coinsCollected);
            // �������� �������� � ��������, ���������, ���������� �������� ������ ��� ����.
            _skinData.skinAvailable = true;
            PlayerPrefs.SetInt("OwnedSkin" + PlayerPrefs.GetInt("LastSelected"), 1); // �������� ���������� ��� ��������
            PlayerPrefs.Save();
            YandexGame.savesData.isBuyed.Add(_skinData.SkinId);
            YandexGame.SaveProgress();
            // ������� ����������� ������� �������
            UpdateCoinText();
            SaveCoins();
            HideButton();
        }
        else
        {
            // �������� ������ ��� ���������� ������� ������� ��� �������
            Debug.Log("����������� ������� ��� ������� ����� ����.");
            
        }
    }

    public void Watch()
    {
        // ��� ��� ��������� ���� �� ��������� ����
        // ��� �������:
        
        YandexGame.savesData.isBuyed.Add(_skinData.SkinId);
        YandexGame.SaveProgress();
        HideButton();
    }

    private void UpdateCoinText()
    {
        // ������ ����� � TextMeshPro ��� ����������� ������� �������
        if (coinText != null)
        {
            coinText.text = coinsCollected.ToString();
        }
    }

    void Rewarded(int id)
    {
            Watch();
    }

    // ����� ��� ������ ����� �������
    public void OpenRewardAd()
    {
        // �������� ����� �������� ����� �������
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
