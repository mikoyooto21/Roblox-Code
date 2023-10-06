using UnityEngine;
using UnityEngine.EventSystems;

namespace MyJoystick
{
    public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private bool _isDragging;
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private Vector2 _direction;
        private float _distance;

        private float _maxDistance;
        private RectTransform _handleRectTransform;
        private RectTransform _rectTransform;

        public Vector2 Direction => _direction;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _handleRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
            _maxDistance = _rectTransform.sizeDelta.y / 2;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging)
                return;

            UpdateDrag(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDragging)
            {
                EndDrag();
            }
            else
            {
                _startPoint = _rectTransform.position;
                _isDragging = true;

                UpdateDrag(eventData.position);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EndDrag();
        }

        private void UpdateDrag(Vector2 position)
        {
            _endPoint = position;
            _direction = -(_startPoint - _endPoint).normalized;
            _distance = Vector2.Distance(_startPoint, _endPoint);
            _distance = Mathf.Min(_distance, _maxDistance);

            _handleRectTransform.localPosition = _direction * _distance;
        }

        private void EndDrag()
        {
            _isDragging = false;
            _direction = Vector2.zero;
            _handleRectTransform.localPosition = Vector2.zero;
        }
    }
}
