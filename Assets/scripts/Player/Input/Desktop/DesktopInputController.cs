using UnityEngine;

public class DesktopInputController : MonoBehaviour
{
    private PlayerInputs _playerInputs;

    private void Update()
    {
        if (_playerInputs == null)
            return;

        // to rotate the camera 
        Vector2 lookInput;
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = -Input.GetAxis("Mouse Y");

        // to move the player
        Vector2 moveDirection = new Vector2();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveDirection.y = 1;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveDirection.y = -1;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveDirection.x = 1;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveDirection.x = -1;

        if (Input.GetKeyDown(KeyCode.G))
            PlayerInputsContoller.Instance.OnClickAirplaneBtn();

        // to jump
        bool jumpInput = Input.GetKeyDown(KeyCode.Space);

        // transmit data 
        _playerInputs.SetMove(moveDirection);
        _playerInputs.SetLook(lookInput);
        _playerInputs.SetJump(jumpInput);
    }

    public void SetPlayerInputs(PlayerInputs playerInputs)
    {
        HideCursor();
        Invoke("HideCursor", 1);

        if (playerInputs != null)
            _playerInputs = playerInputs;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

