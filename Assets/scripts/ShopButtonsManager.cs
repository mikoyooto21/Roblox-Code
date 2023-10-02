using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonsManager : MonoBehaviour
{
    public Button buyButton;
    public Button selectButton;
    public Button watchButton;
    public void DisplayBuyButton()
    {
        buyButton.gameObject.SetActive(true);
        selectButton.gameObject.SetActive(false);
        watchButton.gameObject.SetActive(false);
    }

    public void DisplayOwnButton()
    {
        buyButton.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(true);
        watchButton.gameObject.SetActive(false);
    }

    public void DisplayWatchButton()
    {
        buyButton.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(false);
        watchButton.gameObject.SetActive(true);
    }

    public void HideButton()
    {
        buyButton.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(false);
        watchButton.gameObject.SetActive(false);
    }
}
