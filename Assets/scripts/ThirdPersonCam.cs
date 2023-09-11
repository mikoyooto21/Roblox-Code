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

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;

    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Basic,
        Combat
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        playerObj = player.GetChild(4);

        // switch styles
        if (YandexGame.EnvironmentData.isDesktop) SwitchCameraStyle(CameraStyle.Basic);
        if (YandexGame.EnvironmentData.isMobile) SwitchCameraStyle(CameraStyle.Combat);

        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // roate player object
        if(currentStyle == CameraStyle.Basic)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        else if (currentStyle == CameraStyle.Combat)
        {
            float horizontalInput = joystick.Horizontal;
            float verticalInput = joystick.Vertical;
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
                // Оновіть орієнтацію камери, щоб вона дивилася в тому ж напрямку, що і персонаж.
                orientation.forward = playerObj.forward;
            }
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);

        currentStyle = newStyle;
    }
}
