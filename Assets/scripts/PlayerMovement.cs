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
    private GameObject _skinPlayer;
    private void Start()
    {
        _transform = transform;
        _camera = Camera.main.transform;
        _playerInputs = GetComponent<PlayerInputs>();
        _controller = GetComponent<CharacterController>();
        _playerGroundCheck = GetComponent<PlayerGroundCheck>();

        _inGame = FindObjectOfType<UI_InGame>();
        int mapComplete = PlayerPrefs.GetInt("UnlockedLevelWithTimer", 0);
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        {
            PlayerPrefs.SetInt("UnlockedLevelWithTimer", 0);
            Vector3 playerPosition = new Vector3(PlayerPrefs.GetFloat("PlayerPosX"), PlayerPrefs.GetFloat("PlayerPosY"), PlayerPrefs.GetFloat("PlayerPosZ"));

            Debug.Log("----------" + playerPosition);
            TeleportToTarget(playerPosition);
        }

        _vectorPoint = transform.position;
        /*_checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        _checkpointsList.Clear();
        foreach (GameObject chekpointGameObject in _checkpoints)
        {
            _checkpointsList.Add(chekpointGameObject);
        }*/


        PlayerInputsContoller.Instance.ShowInputController(_playerInputs);

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
                _skinPlayer = modelAnimator.gameObject;
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

        // if (Input.GetKey(KeyCode.R))
        // {
        //     Dead();
        // }
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
    public void SetActiveSkinPlayer(bool activeSkin)
    {
        _skinPlayer.SetActive(activeSkin);
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
            //Dead();
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
            //_checkpointsList.Remove(collider.gameObject);
            //PassedLvls = _checkpointsList.Count;

            int checkpointId = MapController.Instance.GetCheckpointIdByGameObject(collider.gameObject);

            if (PassedLvls != checkpointId)
            {
                PassedLvls = MapController.Instance.GetCheckpointIdByGameObject(collider.gameObject);
                ProgressBarUI.InvokeCheckpointActivatedEvent(PassedLvls);
            }

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
            YandexGame.savesData.PlayerPosX = 8f;
            YandexGame.savesData.PlayerPosY = 0f;
            YandexGame.savesData.PlayerPosZ = 0f;
            PlayerPrefs.SetFloat("PlayerPosX", 8f);
            PlayerPrefs.SetFloat("PlayerPosY", 0f);
            PlayerPrefs.SetFloat("PlayerPosZ", 0f);
            PlayerPrefs.Save();
            YandexGame.SaveProgress();
        }
    }
    void Dead()
    {
        if (AdManager.Instance != null)
            AdManager.Instance.ShowAd();

        TeleportToTarget(_vectorPoint + Vector3.up * 3);
        _sfx.PlaySFX(_sfx.death);
    }
}