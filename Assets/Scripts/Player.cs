using System.Runtime.CompilerServices;
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

    public Vector2 externalForce;
    private Treasure heldTreasure;

    [SerializeField] private GameObject camera;

    private float timer;
    private float stunUntilTime;
    private bool isStunned;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        facingAngle = rb.rotation + (spriteRenderer.flipX ? 180f : 0f);
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");
        isStunned = false;
        timer = 0;
        stunUntilTime = 0;
    }

    private void Update()
    {
        if (isStunned)
        {
            timer += Time.deltaTime;
            if (timer < stunUntilTime)
            {
                return;
            }

            Debug.Log("Player un-stunned");
            isStunned = false;
            stunUntilTime = 0;
            timer = 0;
        }

        // Get movement direction
        if (moveAction == null)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            if (heldTreasure != null)
            {
                heldTreasure.OnDropped();
                heldTreasure = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isStunned) return;
        // Update velocity
        Vector2 inputDirection = new Vector2(moveInput.x, moveInput.y);

        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector2 targetVelocity = inputDirection * maxMoveSpeed;

        Vector2 movementVelocity = Vector2.MoveTowards(
            rb.linearVelocity - externalForce,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = movementVelocity + externalForce;

        externalForce = Vector2.zero;

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
        // clamp pos
        Vector3 pos = transform.position;

        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        float margin = 0.5f;

        pos.x = Mathf.Clamp(pos.x, min.x + margin, max.x - margin);
        pos.y = Mathf.Clamp(pos.y, min.y + margin, max.y - margin);
        transform.position = pos;
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

    public void HoldTreasure(Treasure treasure)
    {
        heldTreasure = treasure;
    }

    public void StunForSeconds(float seconds)
    {
        Debug.Log("STUNNING PLAYER");
        timer = 0;
        stunUntilTime = seconds;
        isStunned = true;
        rb.linearVelocity = new Vector3(0f, 0f, 0f);
        moveInput = Vector2.zero;
    }
}