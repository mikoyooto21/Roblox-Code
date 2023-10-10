using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class PlayerMovement : MonoBehaviour
{

    [HideInInspector] static public int PassedLvls = 0;

    [HideInInspector] public bool CanMove = true;
    [HideInInspector] public bool CanRotation = true;
    [HideInInspector] public bool CanJump = true;

    [Space(10)]
    [SerializeField] private float _moveSpeed = 1.5f;

    [SerializeField, Range(0.0f, 0.3f)] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _speedChangeRate = 10f;

    [Space(10)]
    [SerializeField] private float _jumpHeight = 0.7f;
    [SerializeField] private float _gravity = -15f;

    [Space(10)]
    [SerializeField] private float _jumpTimeout = 0.2f;
    [SerializeField] private float _fallTimeout = 0.15f;

    [Space(10)]
    [SerializeField] private MusicManager _sfx;
    [SerializeField] private float _distanceForReset;
    [SerializeField] private float _timeInAirForReset;

    private int _speedBoostPercentage;
    private int _jumpBoostPercentage;

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
    private float _airTimeoutDelta;

    private Transform _camera;
    private Transform _transform;
    private Animator _animator;
    private PlayerInputs _playerInputs;
    private CharacterController _controller;
    private PlayerGroundCheck _playerGroundCheck;

    private UI_InGame _inGame;
    private Vector3 _vectorPoint;
    static public GameObject[] _checkpoints;
    static public List<GameObject> _checkpointsList = new List<GameObject>();

    private Vector3 _currentVelocity;

    private void Start()
    {
        _transform = transform;
        _camera = Camera.main.transform;
        _playerInputs = GetComponent<PlayerInputs>();
        _controller = GetComponent<CharacterController>();
        _playerGroundCheck = GetComponent<PlayerGroundCheck>();

        _inGame = FindObjectOfType<UI_InGame>();
        int mapComplete = PlayerPrefs.GetInt("UnlockedLevelWithTimer", 0);
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ") && mapComplete == 0)
        {
            PlayerPrefs.SetInt("UnlockedLevelWithTimer", 0);
            Vector3 playerPosition = new Vector3(PlayerPrefs.GetFloat("PlayerPosX"), PlayerPrefs.GetFloat("PlayerPosY"), PlayerPrefs.GetFloat("PlayerPosZ"));
            TeleportToTarget(playerPosition);
        }

        _vectorPoint = transform.position;
        _checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        _checkpointsList.Clear();
        foreach (GameObject chekpointGameObject in _checkpoints)
        {
            _checkpointsList.Add(chekpointGameObject);
        }


        if (Application.isMobilePlatform)
            PlayerInputsContoller.Instance.ShowMobileInputController(_playerInputs);
        else
            PlayerInputsContoller.Instance.ShowDesktopInputController(_playerInputs);

        SetSkin();
    }

    private void Update()
    {
        _playerGroundCheck.MyUpdate();

        CheckForLegitimacy();

        JumpAndGravity();
        Rotation();
        Move();
    }

    private void SetSkin()
    {
        int skinId = 1;

        if (PlayerPrefs.HasKey("PlayerSkinId"))
            skinId = PlayerPrefs.GetInt("PlayerSkinId");
        SkinsConfig skinConfig = GameDataManager.Instance.SkinsConfig;

        for (int i = 0; i < skinConfig.Skins.Count; i++)
        {
            if (skinConfig.Skins[i].SkinId == skinId)
            {
                Debug.Log(skinId);
                transform.GetChild(0).gameObject.SetActive(false);

                Animator modelAnimator = Instantiate(skinConfig.Skins[i].SkinModelPrefab, transform);
                modelAnimator.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
                modelAnimator.transform.localPosition = Vector3.zero;
                _animator = modelAnimator;

                _playerGroundCheck.SetAnimator(_animator);
                break;
            }
        }
    }

    private void CheckForLegitimacy()
    {
        if (_playerGroundCheck.IsGround)
            _airTimeoutDelta = 0;
        else
            _airTimeoutDelta += Time.deltaTime;

        if (_airTimeoutDelta >= _timeInAirForReset)
        {
            Dead();
            _airTimeoutDelta = 0;
        }

        if (transform.position.y < _vectorPoint.y - _distanceForReset && !_playerGroundCheck.IsGround)
        {
            Dead();
        }

        if (Input.GetKey(KeyCode.R))
        {
            Dead();
        }
    }

    public void SetBoost(int speedBoostPercentage, int jumpBoostPercentage)
    {
        _speedBoostPercentage = speedBoostPercentage;
        _jumpBoostPercentage = jumpBoostPercentage;
    }

    private void JumpAndGravity()
    {
        if (_verticalVelocity <= -4 || _verticalVelocity >= 4)
            _animator.SetBool("Air", true);
        else
            _animator.SetBool("Air", false);

        if (_playerGroundCheck.IsGround)
        {
            _fallTimeoutDelta = _fallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;

            // Jump
            if (CanJump && _playerInputs.Jump && _jumpTimeoutDelta <= 0.0f)
            {
                float boost = (_jumpBoostPercentage / 100) * _jumpHeight;
                float targetJumpHeight = _jumpHeight + boost;

                _verticalVelocity = Mathf.Sqrt(targetJumpHeight * -2f * _gravity);
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
        }

        _playerInputs.SetJump(false);

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += _gravity * Time.deltaTime;
    }

    private void Rotation()
    {
        float boost = (_speedBoostPercentage / 100) * _moveSpeed;
        float targetSpeed = _moveSpeed + boost;
        float speedOffset = 0.1f;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        if (_playerInputs.Move == Vector2.zero)
            targetSpeed = 0;

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
        Vector3 inputDirection = new Vector3(_playerInputs.Move.x, 0.0f, _playerInputs.Move.y).normalized;

        if (CanRotation && _playerInputs.Move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);

            // player rotation
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    private void Move()
    {
        float targetSpeed = _speed;
        Vector3 targetDirection;

        if (CanRotation)
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        else
            targetDirection = new Vector3(_playerInputs.Move.x, 0, _playerInputs.Move.y);

        if (!CanMove || _playerInputs.Move == Vector2.zero)
        {
            targetDirection = Vector2.zero;
            _animator.SetBool("Idle", true);
            _animator.SetBool("Walk", false);
        }
        else
        {
            _animator.SetBool("Idle", false);
            _animator.SetBool("Walk", true);
        }

        // move the player
        _controller.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    public void TeleportToTarget(Vector3 position)
    {
        // move the player to a new position 
        _controller.enabled = false;
        _transform.position = position;
        _controller.enabled = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Down"))
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Up"))
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("hotCube") || collision.gameObject.CompareTag("Spikes"))
        {
            Debug.Log(5545);
            Dead();
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Checkpoint") || collider.CompareTag("CheckpointFix"))
        {
            _vectorPoint = collider.transform.position;
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                YandexGame.savesData.PlayerPosX = _vectorPoint.x;
                YandexGame.savesData.PlayerPosY = _vectorPoint.y;
                YandexGame.savesData.PlayerPosZ = _vectorPoint.z;
                YandexGame.savesData.isExited = 1;
                PlayerPrefs.SetFloat("PlayerPosX", _vectorPoint.x);
                PlayerPrefs.SetFloat("PlayerPosY", _vectorPoint.y);
                PlayerPrefs.SetFloat("PlayerPosZ", _vectorPoint.z);
                PlayerPrefs.SetInt("PlayerExited", 1);
                PlayerPrefs.Save();
                YandexGame.SaveProgress();
            }
            _checkpointsList.Remove(collider.gameObject);
            PassedLvls = _checkpointsList.Count;
            if (collider.CompareTag("Checkpoint"))
            {
                _timeInAirForReset = 3f;
                _distanceForReset = 10f;
            }
            else if (collider.CompareTag("CheckpointFix"))
            {
                _timeInAirForReset = 10f;
                _distanceForReset = 100f;
            }
            Debug.Log(PassedLvls);
        }
        else if (collider.CompareTag("Complete"))
        {
            _inGame.MapComplete();
            // YandexGame.savesData.PlayerPosX = 0f;
            // YandexGame.savesData.PlayerPosY = 0f;
            // YandexGame.savesData.PlayerPosZ = 0f;
            // PlayerPrefs.SetFloat("PlayerPosX", 0f);
            // PlayerPrefs.SetFloat("PlayerPosY", 0f);
            // PlayerPrefs.SetFloat("PlayerPosZ", 0f);
            // PlayerPrefs.Save();
            // YandexGame.SaveProgress();
        }
    }

    void Dead()
    {
        TeleportToTarget(_vectorPoint + Vector3.up * 3);
        _sfx.PlaySFX(_sfx.death);
    }





































    /*[Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float airDrag;
    private float startDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public float playerHeight;
    public float groundDistance = 0.3f;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Checkpoint System")]
    bool checkpointCheck;
    [SerializeField] float dead;

    private Vector3 vectorPoint;

    static public int passedLvls = 0;

    static public GameObject[] checkpoints;
    static public List<GameObject> checkpointsList = new List<GameObject>();

    [Header("Additional")]
    private UI_InGame inGame;
    public Transform orientation;
    [SerializeField] Joystick joystick;
    [SerializeField] MusicManager sfx;
    float horizontalInput;
    float verticalInput;
    public float startTimeInAir = 0f;
    public float endTimeInAir = 7f;

    public bool ControllPc = true;

    public Animator animator;
    Vector3 moveDirection;

    Rigidbody rb;

    static public int win = 0;
    public MovementState state;
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        air
    }

    public bool idle = true;
    public bool walking = false;
    int skinId = 1;

    float playerPosX;
    float playerPosY;
    float playerPosZ;

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        {
            playerPosX = PlayerPrefs.GetFloat("PlayerPosX");
            playerPosY = PlayerPrefs.GetFloat("PlayerPosY");
            playerPosZ = PlayerPrefs.GetFloat("PlayerPosZ");

            Vector3 playerPosition = new Vector3(playerPosX, playerPosY, playerPosZ);
            transform.position = playerPosition;
            Debug.Log("Ui_InGame script " + transform.position);
        }

        Debug.Log("PM Start script " + transform.position);
        inGame = FindObjectOfType<UI_InGame>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;

        startDrag = airDrag;

        readyToJump = true;

        startYScale = transform.localScale.y;

        vectorPoint = transform.position;
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        checkpointsList.Clear();
        foreach (GameObject chekpointGameObject in checkpoints)
        {
                checkpointsList.Add(chekpointGameObject);
        }

        SetSkin();

        transform.GetChild(4).localScale += new Vector3(0.2f, 0.2f, 0.2f);
        //transform.GetChild(4).position += new Vector3(0f, 0.7f, 0f);
    }



    private void Update()
    {
        if (YandexGame.EnvironmentData.isMobile)
            ControllPc = false;

        if (transform.position.y < vectorPoint.y - dead && !grounded && startTimeInAir >= endTimeInAir)
        {
            Dead();
        }

        if (Input.GetKey(KeyCode.R))
        {
            Dead();
        }

        // ground check
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);

        if (checkpointsList.Count != 0)
            MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            startTimeInAir = 0f;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void SetSkin()
    {
        if (PlayerPrefs.HasKey("PlayerSkinId"))
            skinId = PlayerPrefs.GetInt("PlayerSkinId");
        SkinsConfig skinConfig = GameDataManager.Instance.SkinsConfig;

        for (int i = 0; i < skinConfig.Skins.Count; i++)
        {
            if (skinConfig.Skins[i].SkinId == skinId)
            {
                Debug.Log(skinId);
                transform.GetChild(0).gameObject.SetActive(false);

                Animator modelAnimator = Instantiate(skinConfig.Skins[i].SkinModelPrefab, transform);
                modelAnimator.transform.localPosition = Vector3.zero;
                animator = modelAnimator;
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (ControllPc)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horizontalInput = joystick.Horizontal;
            verticalInput = joystick.Vertical;
        }



        MovePlayer();
        StateHandler();
    }

    public void ButtonJump()
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MyInput()
    {

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Air", false);
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded && (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Air", false);
            idle = false;
            walking = true;
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else if (grounded)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Air", false);
            idle = true;
            walking = false;
            state = MovementState.idle;
        }

        // Mode - Air
        else
        {
            animator.SetBool("Air", true);
            //animator.SetBool("Walk", false);
            state = MovementState.air;
            rb.AddForce(transform.up * -airDrag, ForceMode.Impulse);
            startTimeInAir += Time.deltaTime;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);

            rb.position = Vector3.up;
        }

        // on ground
        if (grounded && !Input.GetKey(jumpKey))
        {
            rb.velocity = new Vector3(moveDirection.normalized.x * moveSpeed, rb.velocity.y, moveDirection.normalized.z * moveSpeed);
        }

        // in air
        else if (!grounded && state == MovementState.air)
        {
            if (moveDirection.x != 0 && moveDirection.z != 0)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            else
                rb.velocity = new Vector3(0, rb.velocity.y, 0);

            // turn gravity off while on slope
            rb.useGravity = !OnSlope();
        }
    }


    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public void Jump()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        animator.SetTrigger("Jump");

        exitingSlope = true;
        rb.velocity = new Vector3(moveDirection.normalized.x * 6, 0f, moveDirection.normalized.z * 6); // ������� ����������� ���������� ��������

        rb.AddForce(transform.up * jumpForce, ForceMode.Force);
        startTimeInAir = 0;
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Down"))
        {
            endTimeInAir = 0.65f;
            collision.gameObject.SetActive(false);
            startTimeInAir = 0;
            airDrag = startDrag;
        }
        if (collision.gameObject.CompareTag("Up"))
        {
            endTimeInAir = 20f;
            collision.gameObject.SetActive(false);
            airDrag = 0.05f;
        }
        if (collision.gameObject.CompareTag("hotCube") || collision.gameObject.CompareTag("Spikes"))
        {
            Dead();
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Checkpoint"))
        {
            vectorPoint = collider.transform.position;
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                YandexGame.savesData.PlayerPosX = vectorPoint.x;
                YandexGame.savesData.PlayerPosY = vectorPoint.y;
                YandexGame.savesData.PlayerPosZ = vectorPoint.z;
                PlayerPrefs.SetFloat("PlayerPosX", vectorPoint.x);
                PlayerPrefs.SetFloat("PlayerPosY", vectorPoint.y);
                PlayerPrefs.SetFloat("PlayerPosZ", vectorPoint.z);
                PlayerPrefs.Save();
                YandexGame.SaveProgress();
            }
            checkpointsList.Remove(collider.gameObject);
            passedLvls = checkpointsList.Count;
            Debug.Log(passedLvls);
        }
        if (collider.CompareTag("Complete"))
        {
            inGame.MapComplete();
        }
    }

    void Dead()
    {
        transform.position = vectorPoint + Vector3.up * 3;
        sfx.PlaySFX(sfx.death);
    }*/
}