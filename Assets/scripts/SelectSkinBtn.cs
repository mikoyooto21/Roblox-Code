using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;
using System.Collections.Generic;

public class SelectSkinBtn : MonoBehaviour
{
    [SerializeField] private Image _skinImg;

    private SkinData _skinData;
    private Transform _selectSkinParent;
    private BuySellWatch _buySellWatch;
    [SerializeField] private AllSkinsPanel _allSkinsPanel;

    public GameObject border;
    private void OnEnable() => YandexGame.GetDataEvent += LoadSkinData;

    // Отписываемся от события GetDataEvent в OnDisable
    private void OnDisable() => YandexGame.GetDataEvent -= LoadSkinData;

    public TextMeshProUGUI price;

    public List<int> isBuyedData = new List<int>();

    private void Start()
    {
        
    }

    public void Update()
    {
        
        if (PlayerPrefs.GetInt("LastSelected") == _skinData.SkinId)
            border.SetActive(true);
        else
            border.SetActive(false);
        
    }

    public void LoadSkinData()
    {
        isBuyedData = YandexGame.savesData.isBuyed;
    }

    public void SetSkin(SkinData skinData, Transform selectSkinParent)
    {
        _skinData = skinData;
        _skinImg.sprite = skinData._skinImage;

        _selectSkinParent = selectSkinParent;
    }

    public void SetBuySellWatch(BuySellWatch buySellWatch)
    {
        _buySellWatch = buySellWatch;
    }

    public void SelectSkin()
    {
        if (_skinData == null)
            return;

        if (_selectSkinParent.childCount > 0)
            Destroy(_selectSkinParent.GetChild(0).gameObject);

        Instantiate(_skinData.SkinModelPrefab, _selectSkinParent);
        PlayerPrefs.SetInt("LastSelected", _skinData.SkinId);
        Debug.Log(PlayerPrefs.GetInt("OwnedSkin" + _skinData.SkinId));


        if (_skinData != null && !_skinData.skinAvailable && _skinData.skinPrice > 0 && PlayerPrefs.GetInt("OwnedSkin" + _skinData.SkinId) == 0)
        {
            price.text = _skinData.skinPrice.ToString();
            _allSkinsPanel.UpdateUI(1); // Оновіть інтерфейс після вибору скіну. PlayerPrefs.GetInt("LastSelected")
        }
        else if (_skinData != null && ((_skinData.skinAvailable || PlayerPrefs.GetInt("OwnedSkin" + _skinData.SkinId) == 1) || isBuyedData.Contains(_skinData.SkinId)))
            _allSkinsPanel.UpdateUI(2); // Оновіть інтерфейс після вибору скіну.
        else if (_skinData != null && !_skinData.skinAvailable && PlayerPrefs.GetInt("OwnedSkin" + _skinData.SkinId) == 0)
            _allSkinsPanel.UpdateUI(3); // Оновіть інтерфейс після вибору скіну.

        if (_buySellWatch != null && _skinData != null)
        {
            _buySellWatch.UpdateSkinData(_skinData);
        }
    }



    


}
