using UnityEngine;
using YG;

public class ReceivingPurchase : MonoBehaviour
{
    [SerializeField] private PurchaseView _purchaseView;

    private void OnEnable()
    {
        YandexGame.PurchaseSuccessEvent += SuccessPurchased;
        YandexGame.PurchaseFailedEvent += FailedPurchased;
    }

    private void OnDisable()
    {
        YandexGame.PurchaseSuccessEvent -= SuccessPurchased;
        YandexGame.PurchaseFailedEvent -= FailedPurchased;
    }

    void SuccessPurchased(string id)
    {
        // Ваш код для обработки покупки. Например:
        if (id == "airplane")
        {
            _purchaseView.DisableView();
            YandexGame.savesData.isAirplanePurchased = true;
        }

        YandexGame.SaveProgress();
    }

    void FailedPurchased(string id)
    {

    }
}
