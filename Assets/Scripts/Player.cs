using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private float maxMoveSpeed = 5f;
    private float acceleration = 10f;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 moveInput;

    [SerializeField] private GameObject camera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");
    }

    private void Update()
    {
        if (moveAction == null)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
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