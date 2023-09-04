using UnityEngine;

public class PlayerCustomInput : MonoBehaviour
{
    private PlayerInputs _input;

    private void Start()
    {
        _input = GetComponent<PlayerInputs>();
    }

    public void MyUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetButtonDown("Jump");
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);
        bool attackInput = Input.GetMouseButtonDown(0);

        Vector2 moveDirection = new Vector3(horizontalInput, verticalInput);
        Vector2 lookInput = new Vector2(mouseX, -mouseY);

        _input.SetMove(moveDirection);
        _input.SetLook(lookInput);
        _input.SetJump(jumpInput);
        _input.SetSprint(sprintInput);
        _input.SetAttack(attackInput);
    }
}
