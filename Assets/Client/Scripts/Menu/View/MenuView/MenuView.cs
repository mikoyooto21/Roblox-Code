using DG.Tweening;
using UnityEngine;

public class MenuView : MonoBehaviour
{
    public void EnableView()
    {
        if (TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
        {
            canvasGroup.DOFade(1, 0.5f).SetEase(Ease.Linear);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void DisableView()
    {
        if (TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
        {
            canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void NewGame()
    {
        DisableView();
        MenuViewController.Instance.Invoke("LoadGameView", 0.5f);
    }

    // public void NewGameTimer()
    // {
    //     DisableView();
    //     MenuViewController.Instance.Invoke("LoadGameTimerView", 0.5f);
    // }

    public void EnableShopView()
    {
        DisableView();
        MenuViewController.Instance.ShopView.Invoke("EnableView", 0.5f);
    }
}
