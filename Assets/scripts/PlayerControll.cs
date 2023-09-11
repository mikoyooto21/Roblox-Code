using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControll : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    public float extraGravityForce = 5.0f;
    public float climbForce = 10.0f;
    public float rotationSpeed = 10.0f;
    public float angleRayCast = 15f;
    public float rayDistance = 1f;

    private Animator animator;

    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    public LayerMask whatIsStaircase;
    public LayerMask whatIsClimbable;
    public bool ControllPc = true;

    private bool isGrounded;
    private Rigidbody rb;
    private float moveX;
    private float moveZ;
    private Vector3 move;
    [SerializeField] private Joystick joystick;

    void Start()
    {
        // speed = bust;
        rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Получение ввода от пользователя
        if (ControllPc)
        {
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
        }
        else
        {
            moveX = joystick.Horizontal;
            moveZ = joystick.Vertical;
        }
        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;
        inputDirection.Normalize();

        // Вычисление направления движения относительно камеры
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // обеспечиваем движение только по горизонтали
        cameraForward.Normalize();
        Vector3 cameraRight = Camera.main.transform.right;

        Vector3 moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
        moveDirection.Normalize();


        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }


        // Прыжок игрока
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (inputDirection != Vector3.zero)
            animator.SetBool("run", true);
        else
            animator.SetBool("run", false);
    }
    /////////change
    void FixedUpdate()
    {

        if (ControllPc)
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }
        else
        {
            move = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        }
        move.Normalize();
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        Vector3 cameraRight = Camera.main.transform.right;

        Vector3 direction = cameraForward * move.z + cameraRight * move.x;
        direction.Normalize();

        Vector3 footPosition = rb.position - new Vector3(0, 0.5f, 0); // Положение у ног
        Vector3 headPosition = rb.position + new Vector3(0, 0.5f, 0); // Положение у головы

        bool isBlockedCenter = CheckRaycasts(rb.position, transform.forward, rayDistance, out bool onStair, whatIsGround);
        bool isBlockedFoot = CheckRaycasts(footPosition, transform.forward, rayDistance, out _, whatIsGround);
        bool isBlockedHead = CheckRaycasts(headPosition, transform.forward, rayDistance, out _, whatIsGround);

        // Новые проверки для "карабкания"
        bool isBlockedCenterClimb = CheckRaycasts(rb.position, transform.forward, rayDistance, out bool _, whatIsClimbable);
        bool isBlockedFootClimb = CheckRaycasts(footPosition, transform.forward, rayDistance, out _, whatIsClimbable);
        bool isBlockedHeadClimb = CheckRaycasts(headPosition, transform.forward, rayDistance, out _, whatIsClimbable);

        if (!isBlockedHeadClimb && (isBlockedCenterClimb || isBlockedFootClimb) && !isGrounded)
        {
            rb.AddForce(Vector3.up * climbForce, ForceMode.Impulse);
        }
        if (onStair && move.z > 0)
        {
            rb.useGravity = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.MovePosition(rb.position + Vector3.up * speed * Time.fixedDeltaTime);
        }
        else
        {
            rb.useGravity = true;
        }

        if ((isBlockedCenter || isBlockedFoot || isBlockedHead) && move.z > 0)
        {
            move.z = 0; // Если перед игроком препятствие и он пытается двигаться вперед
        }

        direction = cameraForward * move.z + cameraRight * move.x; // Пересчитываем направление после изменения move.z

        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * direction);

        // Проверка на столкновение с землей
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, whatIsGround);
        animator.SetBool("checkground", isGrounded);
        if (!isGrounded && !onStair)
        {
            Vector3 extraGravity = Vector3.down * extraGravityForce;
            rb.AddForce(extraGravity, ForceMode.Acceleration);
        }
    }

    bool CheckRaycasts(Vector3 position, Vector3 direction, float distance, out bool isStair, LayerMask layerToCheck)
    {
        Vector3 leftDirection = Quaternion.Euler(0, -angleRayCast, 0) * direction;
        Vector3 rightDirection = Quaternion.Euler(0, angleRayCast, 0) * direction;

        bool centerHit = Physics.Raycast(position, direction, distance, layerToCheck);
        bool leftHit = Physics.Raycast(position, leftDirection, distance, layerToCheck);
        bool rightHit = Physics.Raycast(position, rightDirection, distance, layerToCheck);

        // Проверка на лестницу
        bool centerStair = Physics.Raycast(position, direction, distance, whatIsStaircase);
        bool leftStair = Physics.Raycast(position, leftDirection, distance, whatIsStaircase);
        bool rightStair = Physics.Raycast(position, rightDirection, distance, whatIsStaircase);
        isStair = centerStair || leftStair || rightStair;

        // Визуализация лучей
        Debug.DrawLine(position, position + direction * distance, Color.red);
        Debug.DrawLine(position, position + leftDirection * distance, Color.green);
        Debug.DrawLine(position, position + rightDirection * distance, Color.blue);

        return centerHit || leftHit || rightHit;
    }
    public void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            //animator.SetTrigger("jump");
        }
    }
    public void SetPosition(Transform currentTransform)
    {
        transform.position = currentTransform.position;
    }
}