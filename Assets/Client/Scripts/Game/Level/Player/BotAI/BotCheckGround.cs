using UnityEngine;

public class BotCheckGround : MonoBehaviour
{
    [SerializeField] private float _rayDistance;
    [SerializeField] private Vector3 _raySize;
    [SerializeField] private LayerMask _groundLayer;

    private Transform _transform;

    public bool CheckGround()
    {
        if (_transform == null)
            _transform = GetComponent<Transform>();

        bool isHit = Physics.BoxCast(_transform.position, _raySize, Vector3.down, Quaternion.identity, _rayDistance, _groundLayer);

        if (isHit)
            return true;
        else
            return false;
    }

    public Transform GetGroundTransform()
    {
        if (_transform == null)
            _transform = GetComponent<Transform>();

        bool isHit = Physics.BoxCast(_transform.position, _raySize, Vector3.down, out RaycastHit hit, Quaternion.identity, _rayDistance, _groundLayer);

        if (isHit)
            return hit.transform;
        else
            return null;
    }
}
