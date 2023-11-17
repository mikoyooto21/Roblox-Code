using HeneGames.Airplane;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class AirplaneMobileInputController : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private MobileInputLook _mobileInputLook;

    private PlayerInputs _playerInputs;
    private SimpleAirPlaneController _airPlaneController;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (_playerInputs == null || _airPlaneController == null)
            return;

        _airPlaneController.inputYaw = _joystick.Direction.x;
        _airPlaneController.inputV = _joystick.Direction.y;
    }

    public void SetPlayerInputs(PlayerInputs playerInputs)
    {
        if (playerInputs != null)
            _playerInputs = playerInputs;

        _mobileInputLook.SetPlayerInputs(_playerInputs);
    }

    public void SetAirPlaneController(SimpleAirPlaneController airPlaneController)
    {
        if (airPlaneController != null)
            _airPlaneController = airPlaneController;
    }

    public void SetInputYaw(int value)
    {
        if (_airPlaneController == null)
            return;

        _airPlaneController.inputH = value;
    }
}
