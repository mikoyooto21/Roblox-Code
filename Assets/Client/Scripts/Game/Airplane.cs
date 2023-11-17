using HeneGames.Airplane;
using System;
using UnityEngine;
using YG;

public class Airplane : MonoBehaviour
{
    public static Airplane Instance { private set; get; }

    [SerializeField] private GameObject _purchaseView;

    [Space(10)]
    [SerializeField] private SimpleAirPlaneController _prefabAirPlane;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CapsuleCollider _targetCollider;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _effect;

    [Space(10)]
    [SerializeField] private float _reloadTimeInSeconds = 15;
    [SerializeField] private float _flightDurationInSeconds = 5;

    private float _reloadTimeoutDelta;
    private float _flightTimeoutDelta;

    private bool _isActivePlane;
    private SimpleAirPlaneController _currentAirplaneInstance;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        _reloadTimeoutDelta -= Time.deltaTime;
        _flightTimeoutDelta -= Time.deltaTime;

        if (_isActivePlane && _flightTimeoutDelta <= 0)
            DioctivateAirplane();

        TpInPlane();
        SetCursorLockMode();
    }

    public void OnClickAirplaneBtn()
    {
        Debug.Log("--------------------------");

        if (!_isActivePlane)
            ActivateAirplane();
        else
            DioctivateAirplane();
    }

    private void SetCursorLockMode()
    {
        if (_purchaseView.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ActivateAirplane()
    {
        if (!YandexGame.savesData.isAirplanePurchased)
        {
            _purchaseView.SetActive(true);
            return;
        }

        if (_reloadTimeoutDelta > 0)
            return;

        if (!_isActivePlane)
        {
            InPlane();
            _isActivePlane = true;
            _flightTimeoutDelta = _flightDurationInSeconds;
        }
    }

    public void DioctivateAirplane()
    {
        if (_isActivePlane)
        {
            ReturnPlayer();
            _isActivePlane = false;
        }
    }

    private void InPlane()
    {
        _playerMovement.enabled = false;
        _targetCollider.isTrigger = true;
        _playerMovement.SetActiveSkinPlayer(false);
        Instantiate(_effect, _player.transform.position, Quaternion.identity);
        Debug.Log("In plane");

        Vector3 posOfSpawn = _player.transform.position;
        Quaternion rotation = _player.transform.rotation;

        _currentAirplaneInstance = Instantiate(_prefabAirPlane, posOfSpawn, rotation);

        PlayerInputsContoller.Instance.ShowAirplaneInputController(_currentAirplaneInstance);
        PlayerInputsContoller.Instance.GetAirplaneBtn().PlayFlightDurationAnim(_flightDurationInSeconds);
    }

    private void ReturnPlayer()
    {
        _reloadTimeoutDelta = _reloadTimeInSeconds;

        _playerMovement.enabled = true;
        _targetCollider.isTrigger = false;
        _playerMovement.SetActiveSkinPlayer(true);
        Instantiate(_effect, _player.transform.position, Quaternion.identity);
        Debug.Log("Out plane");
        if (_currentAirplaneInstance != null)
            Destroy(_currentAirplaneInstance.gameObject);

        PlayerInputsContoller.Instance.ShowInputController();
        PlayerInputsContoller.Instance.GetAirplaneBtn().PlayReloadAnim(_reloadTimeInSeconds);
    }

    private void TpInPlane()
    {
        if (_isActivePlane && _currentAirplaneInstance != null)
            _player.transform.position = _currentAirplaneInstance.transform.position;
    }
}
