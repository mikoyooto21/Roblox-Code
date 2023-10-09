using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharRotation : MonoBehaviour
{
    public Transform cameraTransform;    // Посилання на трансформ камери
    public Transform player; // Посилання на трансформ гравця (персонажа)
    public Transform playerObj;
    public Transform orientation;
    [SerializeField] Joystick joystick;

    public float rotationSpeed = 2f;

    void Start()
    {
        Debug.Log("ChR Start script " + player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        playerObj = player.GetChild(4);

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // Зчитайте введення користувача для руху
        float horizontalInput = joystick.Direction.x;
        float verticalInput = joystick.Direction.y;


        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        Debug.Log("ChR UPD script " + player.transform.position);
    }


}
