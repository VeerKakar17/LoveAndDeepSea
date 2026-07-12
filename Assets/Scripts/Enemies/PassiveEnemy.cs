using UnityEngine;

public class PassiveMob : Enemy
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float acceleration = 4f;

    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;

    [SerializeField] private float minMoveTime = 0.3f;
    [SerializeField] private float maxMoveTime = 1f;

    private enum WanderState
    {
        Waiting,
        Moving,
        Stopping
    }

    private WanderState wanderState;
    private Vector2 moveDirection;
    private Vector2 currentVelocity;
    private float timer;
    private bool initialized;

    protected override void IdleStateUpdate()
    {
        if (!initialized)
        {
            initialized = true;
            StartWaiting();
        }

        Vector2 targetVelocity = Vector2.zero;

        if (wanderState == WanderState.Waiting)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                StartMoving();
            }
        }
        else if (wanderState == WanderState.Moving)
        {
            targetVelocity = moveDirection * moveSpeed;
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                wanderState = WanderState.Stopping;
            }
        }
        else
        {
            if (currentVelocity.sqrMagnitude < 0.001f)
            {
                currentVelocity = Vector2.zero;
                StartWaiting();
            }
        }

        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        rb.linearVelocity = currentVelocity;
    }

    private void StartWaiting()
    {
        wanderState = WanderState.Waiting;
        timer = Random.Range(minWaitTime, maxWaitTime);
    }

    private void StartMoving()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        timer = Random.Range(minMoveTime, maxMoveTime);
        wanderState = WanderState.Moving;
    }

    public override void OnPlayerCollision()
    {
        base.OnPlayerCollision();
    }
}