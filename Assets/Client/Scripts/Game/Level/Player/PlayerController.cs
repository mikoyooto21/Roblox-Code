using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsBot = false;
    public bool CanMove = true;
    public bool CanRotation = true;
    public bool CanJump = true;
    public bool CanAttack = true;
    public bool CanHit = true;

    public string Nickname;

    [Space(10)]
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _sprintSpeed = 3f;
    [SerializeField] private float _throwSpeed = 5f;

    [SerializeField, Range(0.0f, 0.3f)] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _speedChangeRate = 10f;

    [Space(10)]
    [SerializeField] private float _jumpHeight = 0.7f;
    [SerializeField] private float _gravity = -15f;

    [Space(10)]
    [SerializeField] private float _jumpTimeout = 0.2f;
    [SerializeField] private float _fallTimeout = 0.15f;
    [SerializeField] private float _groundedOffset = -0.15f;
    [SerializeField] private float _groundedRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayers;

    [Space(10)]
    [SerializeField] private float _attackTimeout = 0.5f;
    [SerializeField] private float _attackRadius = 2f;
    [SerializeField] private Transform _attackPosition;
    [SerializeField] private LayerMask _enemyLayer;

    [Space(10)]
    [SerializeField] private Transform _nicknameTxtPosition;
    [SerializeField] private Animator _animator;

    private bool _grounded;

    private bool _isAttack;
    private bool _isHit;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private float _attackTimeoutDelta;

    private Transform _camera;
    private PlayerInputs _input;
    private Transform _transform;
    private CharacterController _controller;
    private PlayerCustomInput _playerCustomInput;
    private BotAI _botAI;

    public bool Grounded => _grounded;
    public Transform NicknameTxtPosition => _nicknameTxtPosition;

    private void Start()
    {
        _camera = Camera.main.transform;

        _input = GetComponent<PlayerInputs>();
        _transform = GetComponent<Transform>();
        _controller = GetComponent<CharacterController>();
        _playerCustomInput = GetComponent<PlayerCustomInput>();
        _botAI = GetComponent<BotAI>();

        if (!IsBot)
            SetSkin();

        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public void MyUpdate()
    {
        if (_playerCustomInput != null)
            _playerCustomInput.MyUpdate();

        if (_botAI != null)
            _botAI.MyUpdate();

        GroundedCheck();
        JumpAndGravity();

        if (!_isHit)
            Move();
        else
            Throw();

        Attack();
    }

    private void SetSkin()
    {
        int skinId = PlayerPrefs.GetInt("PlayerSkinId");
        SkinsConfig skinConfig = GameDataManager.Instance.SkinsConfig;

        for (int i = 0; i < skinConfig.Skins.Count; i++)
        {
            if (skinConfig.Skins[i].SkinId == skinId)
            {
                _transform.GetChild(0).gameObject.SetActive(false);

                Animator modelAnimator = Instantiate(skinConfig.Skins[i].SkinModelPrefab, _transform);
                _animator = modelAnimator;

                break;
            }
        }

    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(_transform.position.x, _transform.position.y - _groundedOffset, _transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

        _animator.SetBool("Grounded", _grounded);
    }

    private void JumpAndGravity()
    {
        if (_grounded)
        {
            _fallTimeoutDelta = _fallTimeout;

            _animator.SetBool("Jump", false);
            _animator.SetBool("FreeFall", false);

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;

            // Jump
            if (_input.Jump && _jumpTimeoutDelta <= 0.0f && CanJump && !_isAttack)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _animator.SetBool("Jump", true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
                _jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = _jumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
                _fallTimeoutDelta -= Time.deltaTime;
            else
                _animator.SetBool("FreeFall", true);

            _input.SetJump(false);
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += _gravity * Time.deltaTime;
    }

    private void Move()
    {
        float targetSpeed = _input.Sprint ? _sprintSpeed : _moveSpeed;

        if (_input.Move == Vector2.zero)
            targetSpeed = 0.0f;

        float speedOffset = 0.1f;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChangeRate);

        if (_animationBlend < 0.01f)
            _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;

        if (CanRotation && _input.Move != Vector2.zero)
        {
            if (IsBot)
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            else
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);

            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection;

        if (CanRotation)
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        else
            targetDirection = new Vector3(_input.Move.x, 0, _input.Move.y);

        if (!CanMove)
        {
            _animationBlend = 0;
            targetDirection = Vector2.zero;
        }

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _animator.SetFloat("Speed", _animationBlend);
        _animator.SetFloat("MotionSpeed", 1);
    }

    private void Throw()
    {
        if (!CanHit)
            return;

        Vector3 targetDirection = new Vector3(_input.Move.x, 0, _input.Move.y);

        _controller.Move(targetDirection.normalized * (_throwSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _animator.SetFloat("Speed", 0);
        _animator.SetFloat("MotionSpeed", 1);
    }

    private void ThrowEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_attackPosition.position, _attackRadius, _enemyLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Player" && hitCollider.transform != _transform)
            {
                Vector3 pushDirection = (hitCollider.transform.position - _transform.position).normalized;

                if (hitCollider.TryGetComponent<PlayerController>(out PlayerController controller))
                    controller.OnHit(new Vector2(pushDirection.x, pushDirection.z));
            }
        }
    }

    private void Attack()
    {
        _attackTimeoutDelta -= Time.deltaTime;

        if (CanAttack && !_isAttack && _grounded && !_isHit && _input.Attack && _attackTimeoutDelta <= 0f)
        {
            _attackTimeoutDelta = _attackTimeout;
            _input.SetAttack(false);
            StartCoroutine(AttackAnimation());
        }
    }

    private IEnumerator AttackAnimation()
    {
        if (!CanAttack || _isAttack)
            yield break;

        _isAttack = true;
        CanMove = false;

        _animator.SetTrigger("Hit");

        yield return new WaitForSeconds(0.03f);
        ThrowEnemies();
        yield return new WaitForSeconds(0.3f);

        _isAttack = false;
        CanMove = true;
    }

    private void OnHit(Vector2 bounceDirection)
    {
        if (CanHit)
            StartCoroutine(OnHitAnim(bounceDirection));
    }

    IEnumerator OnHitAnim(Vector2 direction)
    {
        _isHit = true;
        _input.SetMove(direction);
        _input.SetSprint(true);
        _input.CanChange = false;

        yield return new WaitForSeconds(0.2f);

        _input.CanChange = true;
        _input.SetMove(Vector2.zero);
        _input.SetSprint(false);
        _input.CanChange = false;

        yield return new WaitForSeconds(0.1f);
        _input.CanChange = true;
        _isHit = false;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = Color.green;
        Color transparentRed = Color.red;

        transparentRed.a = 0.3f;
        transparentGreen.a = 0.3f;

        if (Grounded)
            Gizmos.color = transparentGreen;
        else
            Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPosition.position, _attackRadius);
    }
}
