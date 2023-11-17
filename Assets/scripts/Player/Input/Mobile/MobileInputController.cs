using UnityEngine;

public class MobileInputController : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private MobileInputLook _mobileInputLook;

    private PlayerInputs _playerInputs;

    private void Update()
    {
        if (_playerInputs == null)
            return;

        Vector2 move = _joystick.Direction;
        _playerInputs.SetMove(move);
    }

    public void SetPlayerInputs(PlayerInputs playerInputs)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerInputs != null)
            _playerInputs = playerInputs;

        _mobileInputLook.SetPlayerInputs(_playerInputs);
    }

    public void OnClickJumpBtn()
    {
        if (_playerInputs == null)
            return;

        _playerInputs.SetJump(true);
    }
}

