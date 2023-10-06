using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputLook : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private float _sensitivity = 0.1f;

    private PlayerInputs _playerInputs;

    private int _countFrame;

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
        Vector2 look;
        look.x = eventData.delta.x * _sensitivity;
        look.y = -eventData.delta.y * _sensitivity;

        if (_playerInputs != null)
            _playerInputs.SetLook(look);

        _countFrame = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_playerInputs != null)
            _playerInputs.SetLook(Vector2.zero);
    }

    public void SetPlayerInputs(PlayerInputs playerInputs)
    {
        _playerInputs = playerInputs;
        gameObject.SetActive(true);
    }
}
