using DG.Tweening;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float _endYPosition;
    [SerializeField] private BoxCollider _boxCollider;

    public void FallAnimation()
    {
        transform.DOLocalMoveY(_endYPosition, 2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _boxCollider.enabled = true;
            });
    }

    public void DestroyAnim()
    {
        _boxCollider.enabled = false;
        transform.DOLocalMoveY(transform.position.y - 0.5f, 1f);
    }

    public void Deactivate()
    {
        _boxCollider.enabled = false;
    }
}
