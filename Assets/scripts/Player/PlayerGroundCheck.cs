using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private float _groundedOffset = -0.15f;
    [SerializeField] private float _groundedRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayers;

    private bool _isGround;
    private Animator _animator;
    private Transform _transform;

    public bool IsGround => _isGround;

    public void MyUpdate()
    {
        GroundedCheck();
    }

    public void SetAnimator(Animator animator)
    {
        _animator = animator;
    }

    private void GroundedCheck()
    {
        if (_transform == null)
            _transform = transform;

        // check for ground underfoot 
        Vector3 spherePosition = new Vector3(_transform.position.x, _transform.position.y - _groundedOffset, _transform.position.z);
        _isGround = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

        if (_animator != null)
            _animator.SetBool("Ground", _isGround);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = Color.green;
        Color transparentRed = Color.red;

        transparentRed.a = 0.3f;
        transparentGreen.a = 0.3f;

        if (_isGround)
            Gizmos.color = transparentGreen;
        else
            Gizmos.color = transparentRed;

        // grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
    }
}

