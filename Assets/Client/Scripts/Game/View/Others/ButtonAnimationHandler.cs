using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimationHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 _defaultSize;
    private Vector2 _pressedSize;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TryGetComponent<RectTransform>(out RectTransform rectTransform))
        {
            if (_defaultSize == Vector2.zero)
            {
                _defaultSize = rectTransform.sizeDelta;

                _pressedSize.x = rectTransform.sizeDelta.x - (rectTransform.sizeDelta.x * 0.1f);
                _pressedSize.y = rectTransform.sizeDelta.y - (rectTransform.sizeDelta.y * 0.1f);
            }


            rectTransform.DOSizeDelta(_pressedSize, 0.2f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (TryGetComponent<RectTransform>(out RectTransform rectTransform))
        {
            if (_defaultSize == Vector2.zero)
            {
                _defaultSize = rectTransform.sizeDelta;

                _pressedSize.x = rectTransform.sizeDelta.x - (rectTransform.sizeDelta.x * 0.1f);
                _pressedSize.y = rectTransform.sizeDelta.y - (rectTransform.sizeDelta.y * 0.1f);
            }

            rectTransform.DOSizeDelta(_defaultSize, 0.2f);
        }
    }
}
