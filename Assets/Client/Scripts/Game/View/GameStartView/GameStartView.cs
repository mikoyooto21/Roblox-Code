using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartView : MonoBehaviour
{
    [SerializeField] private TMP_Text _startTxt;

    private void Start()
    {
        _startTxt.text = "загрузка";

        if (TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
            canvasGroup.DOFade(1f, 0f).SetEase(Ease.Linear);

        GameEvents.OnLevelBuiltEvent.AddListener(() =>
        {
            EnambelView();
        });
    }

    public void EnambelView()
    {
        if (TryGetComponent<Image>(out Image image))
        {
            image.DOFade(0.7843137f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                StartCoroutine(StartAnim());
            });
        }
    }

    public void DisableView()
    {
        if (TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
        {
            canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                GameEvents.InvokeGameStartEvent();
            });
        }
    }


    IEnumerator StartAnim()
    {
        _startTxt.text = "3";
        yield return new WaitForSeconds(1f);

        _startTxt.text = "2";
        yield return new WaitForSeconds(1f);

        _startTxt.text = "1";
        yield return new WaitForSeconds(1f);

        _startTxt.text = "старт!";
        yield return new WaitForSeconds(1f);

        DisableView();
    }
}
