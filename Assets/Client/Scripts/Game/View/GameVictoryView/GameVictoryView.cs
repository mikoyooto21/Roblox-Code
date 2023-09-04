using DG.Tweening;
using UnityEngine;

public class GameVictoryView : MonoBehaviour
{
    public void EnableView()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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

    public void RestartGame()
    {
        GameController.Instance.RestartGame();
    }

    public void LoadMenuScene()
    {
        GameController.Instance.LoadMenuScene();
    }
}
