using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool IsEmpty;

    [SerializeField] private Color32[] _destroyedColors;
    [SerializeField] private GameObject _destroyedBlockPlatform;
    [SerializeField] private GameObject _trapShadow;
    [SerializeField] private LayerMask _destroyedLayerMask;

    public void Destroy()
    {
        gameObject.layer = LayerMask.NameToLayer("DestroyedBlock");

        _destroyedBlockPlatform.SetActive(true);
        MeshRenderer meshRenderer = _destroyedBlockPlatform.GetComponent<MeshRenderer>();

        Sequence seq = DOTween.Sequence();

        seq.Append(meshRenderer.material.DOColor(_destroyedColors[0], 0))
            .Append(meshRenderer.material.DOColor(_destroyedColors[1], 2).SetEase(Ease.Linear))
            .Join(transform.DOMoveY(-0.2f, 1))
            .Append(transform.DOMoveY(-0.5f, 1))
            .Append(transform.DOMoveY(-20, 3).OnComplete(() =>
            {
                gameObject.SetActive(false);
            }));
    }

    public void EnableTrapShadow()
    {
        if (!IsEmpty)
            return;

        StartCoroutine(EnableTrapShadowCoroutine());
    }

    IEnumerator EnableTrapShadowCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _trapShadow.SetActive(true);
        yield return new WaitForSeconds(1f);
        _trapShadow.SetActive(false);
    }
}
