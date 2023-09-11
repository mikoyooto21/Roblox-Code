using UnityEngine;
using TMPro;

public class AllSkinsPanel : ShopButtonsManager
{
    [SerializeField] private SelectSkinBtn[] _selectSkinBtns;
    [SerializeField] private Transform _selectSkinParent;
    [SerializeField] private BuySellWatch _buySellWatch;
    public TextMeshProUGUI coinText; // Посилання на TextMeshPro для відображення кількості монеток


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
        // Перевірте умови, які визначають, які кнопки мають бути активні або неактивні.
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
            Debug.Log("");
        }
    }

    public void NotActive()
    {
        _selectSkinParent.gameObject.SetActive(false);
    }

    public void IsActive()
    {
        _selectSkinParent.gameObject.SetActive(true);
    }
}
