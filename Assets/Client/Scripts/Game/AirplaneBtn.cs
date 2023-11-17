using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneBtn : MonoBehaviour
{
    [SerializeField] private Image _flightDurationImg;

    [SerializeField] private Color _reloadColor;
    [SerializeField] private Color _flightDurationColor;

    public void PlayReloadAnim(float duration)
    {
        _flightDurationImg.color = _reloadColor;
        _flightDurationImg.DOFillAmount(1f, duration).OnComplete(() =>
        {
            _flightDurationImg.color = _flightDurationColor;
        });
    }

    public void PlayFlightDurationAnim(float duration)
    {
        _flightDurationImg.fillAmount = 1;
        _flightDurationImg.color = _flightDurationColor;
        _flightDurationImg.DOFillAmount(0f, duration);
    }
}
