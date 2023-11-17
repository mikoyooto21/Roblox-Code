using HeneGames.Airplane;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirplaneDesktopInputController : MonoBehaviour
{
    private PlayerInputs _playerInputs;
    private SimpleAirPlaneController _airPlaneController;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

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

        _airPlaneController.inputYaw = 0;
        if (Input.GetKey(KeyCode.Q))
            _airPlaneController.inputYaw = -1;
        else if (Input.GetKey(KeyCode.E))
            _airPlaneController.inputYaw = 1;

        if (Input.GetKeyDown(KeyCode.G))
            PlayerInputsContoller.Instance.OnClickAirplaneBtn();

        // transmit data 

        _airPlaneController.inputH = moveDirection.x;
        _airPlaneController.inputV = moveDirection.y;
        _playerInputs.SetLook(lookInput);
    }

    public void SetPlayerInputs(PlayerInputs playerInputs)
    {
        HideCursor();
        Invoke("HideCursor", 1);

        if (playerInputs != null)
            _playerInputs = playerInputs;
    }

    public void SetAirPlaneController(SimpleAirPlaneController airPlaneController)
    {
        if (airPlaneController != null)
            _airPlaneController = airPlaneController;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
