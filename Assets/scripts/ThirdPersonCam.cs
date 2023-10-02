using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    [SerializeField] Joystick joystick;

    public PlayerMovement playerMovement;

    public float rotationSpeed;

    public GameObject thirdPersonCam;


    private void Start()
    {
        Debug.Log("TPC Start script " + player.transform.position);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        playerObj = player.GetChild(4);

        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // roate player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        Debug.Log("TPC UPD script " + player.transform.position);
    }
}
