using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputLook : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _sensitivity = 0.1f;

    private PlayerInputs _playerInputs;

    private int _countFrame;
    private bool _isDragging;

    private void Update()
    {
        _countFrame++;

        if (_countFrame > 5)
        {
            if (_playerInputs != null)
                _playerInputs.SetLook(Vector2.zero);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging)
            return;

        UpdateDrag(eventData.delta);
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
            _isDragging = true;
            UpdateDrag(eventData.delta);
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        EndDrag();
    }

    public void SetPlayerInputs(PlayerInputs playerInputs)
    {
        _playerInputs = playerInputs;
        gameObject.SetActive(true);
    }

    private void UpdateDrag(Vector2 position)
    {
        Vector2 look;
        look.x = position.x * _sensitivity;
        look.y = -position.y * _sensitivity;

        if (_playerInputs != null)
            _playerInputs.SetLook(look);

        _countFrame = 0;
    }

    private void EndDrag()
    {
        _isDragging = false;

        if (_playerInputs != null)
            _playerInputs.SetLook(Vector2.zero);
    }
}
