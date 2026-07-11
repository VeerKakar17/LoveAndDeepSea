using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float maxMoveSpeed = 4f;
    private float acceleration = 8f;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private float rotationSpeed = 360f;
    private float facingAngle;
    private float lastTurnDirection = 1f;

    [SerializeField] private GameObject camera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        facingAngle = rb.rotation + (spriteRenderer.flipX ? 180f : 0f);
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");
    }

    private void Update()
    {
        // Get movement direction
        if (moveAction == null)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Update velocity
        Vector2 inputDirection = new Vector2(moveInput.x, moveInput.y);

        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector2 targetVelocity = inputDirection * maxMoveSpeed;

        Vector2 currentHorizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            rb.linearVelocity.y
        );

        Vector2 newHorizontalVelocity = Vector2.MoveTowards(currentHorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newHorizontalVelocity.x, newHorizontalVelocity.y);

        // Handle direction
        if (rb.linearVelocity.sqrMagnitude > 0.001f || moveInput.sqrMagnitude > 0.001f)
        {
            Vector2 facingDirection = moveInput.sqrMagnitude > 0.001f
                ? moveInput.normalized
                : rb.linearVelocity.normalized;

            float targetAngle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
            float angleDifference = Mathf.DeltaAngle(facingAngle, targetAngle);

            if (Mathf.Abs(angleDifference) >= 179.9f)
            {
                angleDifference = -lastTurnDirection * 180f;
            }

            float angleChange = Mathf.Clamp(
                angleDifference,
                -rotationSpeed * Time.fixedDeltaTime,
                rotationSpeed * Time.fixedDeltaTime
            );

            if (Mathf.Abs(angleChange) > 0.001f)
            {
                lastTurnDirection = Mathf.Sign(angleChange);
            }

            facingAngle += angleChange;

            spriteRenderer.flipX = Mathf.Cos(facingAngle * Mathf.Deg2Rad) < 0f;

            rb.MoveRotation(
                spriteRenderer.flipX
                    ? facingAngle - 180f
                    : facingAngle
            );
        }
    }

    // Move positions when time swap happens
    private void HandleTimeSwap(GameManager.Time newTime)
    {
        if (newTime == GameManager.Time.AncientTime)
        {
            transform.position += new Vector3(0f, 10000f, 0f);
            camera.transform.position += new Vector3(0f, 10000f, 0f);
        } else
        {
            transform.position -= new Vector3(0f, 10000f, 0f);
            camera.transform.position -= new Vector3(0f, 10000f, 0f);
        }
    }

    private void OnEnable()
    {
        GameManager.OnTimeSwap += HandleTimeSwap;
    }

    private void OnDisable()
    {
        GameManager.OnTimeSwap -= HandleTimeSwap;
    }
}