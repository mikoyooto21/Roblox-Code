using UnityEngine;
using TMPro;
using YG;

public class AllSkinsPanel : ShopButtonsManager
{
    [SerializeField] private SelectSkinBtn _selectSkinBtn;
    [SerializeField] private SelectSkinBtn[] _selectSkinBtns;
    [SerializeField] private Transform _selectSkinParent;
    [SerializeField] private BuySellWatch _buySellWatch;
    public TextMeshProUGUI coinText; // ��������� �� TextMeshPro ��� ����������� ������� �������
    [SerializeField] private GameObject playerImg;

    private void OnEnable()
    {

        SkinsConfig skinConfig = MenuDataManager.Instance.SkinsConfig;

        for (int i = 0; i < skinConfig.Skins.Count; i++)
        {
            if (_selectSkinBtns.Length > i)
            {
                _selectSkinBtns[i].SetSkin(skinConfig.Skins[i], _selectSkinParent);
                _selectSkinBtns[i].SetBuySellWatch(_buySellWatch);
            }
        }

        UpdateUI(0);
    }
    public void UpdateUI(int i)
    {
        // �������� �����, �� ����������, �� ������ ����� ���� ������ ��� ��������.
        if (i == 1)
        {
            DisplayBuyButton();
        }
        else if (i == 2)
        {
            DisplayOwnButton();
        }
        else if (i == 3)
        {
            DisplayWatchButton();
        }
        else
        {
            HideButton();
            Debug.Log("");
        }
    }

    public void IsActive()
    {
        _selectSkinParent.gameObject.SetActive(true);
    }
}
