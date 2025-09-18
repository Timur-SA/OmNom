using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float wallJumpForce = 4f; // Сила отталкивания от стены
    public float tiltAngle = 15f; // Угол наклона при прыжке
    public float tiltSpeed = 5f; // Скорость наклона
    public float slideSpeed = 2f; // Скорость скольжения по стене
    private bool isGrounded;
    private bool isTouchingWall;
    private bool canWallJump = true; // Можно ли отскакивать от стены
    private Quaternion targetRotation; // Целевая ориентация (для возврата)
    private float wallDirection; // Направление стены относительно игрока

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetRotation = Quaternion.Euler(0, 0, 0); // Исходное положение (без наклона)
    }

    void Update()
    {
        // Движение вправо-влево (WASD)
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Наклон при движении в прыжке
        if (!isGrounded)
        {
            if (moveInput > 0) // Вправо
            {
                targetRotation = Quaternion.Euler(0, 0, -tiltAngle); // Наклон влево
            }
            else if (moveInput < 0) // Влево
            {
                targetRotation = Quaternion.Euler(0, 0, tiltAngle); // Наклон вправо
            }
        }
        else
        {
            targetRotation = Quaternion.Euler(0, 0, 0); // Возврат в исходное положение
        }

        // Плавный поворот
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * Time.deltaTime);

        // Прыжок (пробел) с земли
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            canWallJump = true; // Разрешаем отскок от стены после прыжка
        }
        // Прыжок от стены
        else if (Input.GetKeyDown(KeyCode.Space) && isTouchingWall && !isGrounded && canWallJump)
        {
            rb.velocity = new Vector2(wallDirection * wallJumpForce, jumpForce);
            canWallJump = false; // Запрещаем повторный отскок до касания земли
        }

        // Скольжение по стене
        if (isTouchingWall && !isGrounded)
        {
            // Проверяем, движется ли игрок в сторону стены
            bool movingIntoWall = (moveInput > 0 && wallDirection < 0) || (moveInput < 0 && wallDirection > 0);
            if (movingIntoWall)
            {
                // Игрок скользит вниз, даже если движется в стену
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -slideSpeed));
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isTouchingWall = false;
            canWallJump = true; // Сбрасываем возможность отскока при касании земли
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
            // Определяем направление стены (относительно игрока)
            wallDirection = collision.transform.position.x > transform.position.x ? -1f : 1f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }
}