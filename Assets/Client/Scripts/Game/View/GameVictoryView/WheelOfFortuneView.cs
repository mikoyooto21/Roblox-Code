using DG.Tweening;
using UnityEngine;

public class WheelOfFortuneView : MonoBehaviour
{
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _speed;

    private void Start()
    {
        transform.localPosition = new Vector2(_minX, transform.localPosition.y);

        transform.DOLocalMoveX(_maxX, (_maxX - _minX) * _speed).SetEase(Ease.Linear).SetLoops(-1);
    }
}
