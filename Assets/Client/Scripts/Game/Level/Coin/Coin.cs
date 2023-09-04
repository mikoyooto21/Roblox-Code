using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void Start()
    {
        transform.DORotate(new Vector3(0, 180, 0), 3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}
