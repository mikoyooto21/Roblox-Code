using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private TMP_Text _placeTxt;

    public void EnableView()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
        {
            SetPlaceTxt((LevelController.Instance.LevelBuilder.PlayersList.Count + 1).ToString());

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

    private void SetPlaceTxt(string place)
    {
        _placeTxt.text = place + " место";
    }

    public void LoadMenuScene()
    {
        GameController.Instance.LoadMenuScene();
    }
}
