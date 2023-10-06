using UnityEngine;

public class MobileInputController : MonoBehaviour
{
    [SerializeField] private MyJoystick.Joystick _joystick;
    [SerializeField] private MobileInputLook _mobileInputLook;

    private PlayerInputs _playerInputs;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (_playerInputs == null)
            return;

        Vector2 move = _joystick.Direction;
        _playerInputs.SetMove(move);
    }

    public void SetPlayerInputs(PlayerInputs playerInputs)
    {
        _playerInputs = playerInputs;
        _mobileInputLook.SetPlayerInputs(_playerInputs);

        gameObject.SetActive(true);
    }

    public void OnClickJumpBtn()
    {
        if (_playerInputs == null)
            return;

        _playerInputs.SetJump(true);
    }
}

