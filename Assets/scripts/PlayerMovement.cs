using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using YG;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float airDrag;
    private float startDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public float playerHeight;
    public float groundDistance = 0.3f;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Checkpoint System")]
    bool checkpointCheck;
    [SerializeField] float dead;

    private Vector3 vectorPoint;

    static public int passedLvls = 0;

    static public GameObject[] checkpoints;
    static public List<GameObject> checkpointsList = new List<GameObject>();

    [Header("Additional")]
    private UI_InGame inGame;
    public Transform orientation;
    [SerializeField] Joystick joystick;
    [SerializeField] MusicManager sfx;
    float horizontalInput;
    float verticalInput;
    public float startTimeInAir = 0f;
    public float endTimeInAir = 7f;

    public bool ControllPc = true;

    public Animator animator;
    Vector3 moveDirection;

    Rigidbody rb;

    static public int win = 0;
    public MovementState state;
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        air
    }

    public bool idle = true;
    public bool walking = false;
    int skinId = 1;


    private void Start()
    {
        inGame = FindObjectOfType<UI_InGame>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;

        startDrag = airDrag;

        readyToJump = true;

        startYScale = transform.localScale.y;

        vectorPoint = transform.position;
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        checkpointsList.Clear();
        foreach (GameObject chekpointGameObject in checkpoints)
        {
                checkpointsList.Add(chekpointGameObject);
        }

        SetSkin();

        transform.GetChild(4).localScale += new Vector3(0.2f, 0.2f, 0.2f);
        transform.GetChild(4).position += new Vector3(0f, 0.7f, 0f);
    }



    private void Update()
    {
        if (YandexGame.EnvironmentData.isMobile)
            ControllPc = false;

        if (transform.position.y < vectorPoint.y - dead && !grounded && startTimeInAir >= endTimeInAir)
        {
            Dead();
        }

        if (Input.GetKey(KeyCode.R))
        {
            Dead();
        }

        // ground check
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);

        if (checkpointsList.Count != 0)
            MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            startTimeInAir = 0f;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void SetSkin()
    {
        if (PlayerPrefs.HasKey("PlayerSkinId"))
            skinId = PlayerPrefs.GetInt("PlayerSkinId");
        SkinsConfig skinConfig = GameDataManager.Instance.SkinsConfig;

        for (int i = 0; i < skinConfig.Skins.Count; i++)
        {
            if (skinConfig.Skins[i].SkinId == skinId)
            {
                Debug.Log(skinId);
                transform.GetChild(0).gameObject.SetActive(false);

                Animator modelAnimator = Instantiate(skinConfig.Skins[i].SkinModelPrefab, transform);
                animator = modelAnimator;
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (ControllPc)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horizontalInput = joystick.Horizontal;
            verticalInput = joystick.Vertical;
        }

        MovePlayer();
        StateHandler();
    }

    public void ButtonJump()
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MyInput()
    {

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Air", false);
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded && Input.anyKey)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Air", false);
            idle = false;
            walking = true;
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else if (grounded && !Input.anyKey)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Air", false);
            idle = true;
            walking = false;
            state = MovementState.idle;
        }

        // Mode - Air
        else
        {
            animator.SetBool("Air", true);
            animator.SetBool("Walk", false);
            state = MovementState.air;
            rb.AddForce(transform.up * -airDrag, ForceMode.Impulse);
            startTimeInAir += Time.deltaTime;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);

            rb.position = Vector3.up;
        }

        // on ground
        if (grounded && !Input.GetKey(jumpKey))
        {
            rb.velocity = new Vector3(moveDirection.normalized.x * moveSpeed, rb.velocity.y, moveDirection.normalized.z * moveSpeed);
        }

        // in air
        else if (!grounded && state == MovementState.air)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed / 2f * 10f * airMultiplier, ForceMode.Force);
        }

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }


    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public void Jump()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        animator.SetTrigger("Jump");

        exitingSlope = true;
        rb.velocity = new Vector3(moveDirection.normalized.x * 6, 0f, moveDirection.normalized.z * 6); // Скинути вертикальну компоненту швидкості

        rb.AddForce(transform.up * jumpForce, ForceMode.Force);
        startTimeInAir = 0;
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Down"))
        {
            endTimeInAir = 0.65f;
            collision.gameObject.SetActive(false);
            startTimeInAir = 0;
            airDrag = startDrag;
        }
        if (collision.gameObject.CompareTag("Up"))
        {
            endTimeInAir = 20f;
            collision.gameObject.SetActive(false);
            airDrag = 0.05f;
        }
        if (collision.gameObject.CompareTag("hotCube") || collision.gameObject.CompareTag("Spikes"))
        {
            Dead();
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Checkpoint"))
        {
            vectorPoint = collider.transform.position;
            checkpointsList.Remove(collider.gameObject);
            passedLvls = checkpointsList.Count;
            Debug.Log(passedLvls);
        }
        if (collider.CompareTag("Complete"))
        {
            inGame.MapComplete();
        }
    }

    void Dead()
    {
        transform.position = vectorPoint + Vector3.up * 3;
        sfx.PlaySFX(sfx.death);
    }
}