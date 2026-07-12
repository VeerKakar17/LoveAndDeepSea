using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class StunEnemy : Enemy
{
    [SerializeField] private float stunTime;
    [SerializeField] private float ChaseStateEnterDist;
    [SerializeField] private float CloseStateEnterDist;
    [SerializeField] private float CloseStateLeaveDist;
    [SerializeField] private float IdleStateEnterDist;
    [SerializeField] private float moveSpeed;

    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDistance = 1.5f;

    private enum CloseMoveState
    {
        Charging,
        Dashing
    }

    private CloseMoveState closeMoveState;
    private float dashTimer;
    private Vector2 dashTargetPosition;
    private bool hitPlayer = false;

    public override void OnPlayerCollision()
    {
        GameManager.instance.player.StunForSeconds(stunTime);
        hitPlayer = true;
    }

    protected override void ChaseStateUpdate()
    {
        Vector2 playerPosition = GameManager.instance.player.transform.position;
        float distToPlayer = Vector2.Distance(playerPosition, transform.position);

        if (distToPlayer < CloseStateEnterDist)
        {
            EnterCloseState(playerPosition);
            currState = Enemy.EnemyMoveState.CloseState;
            return;
        }
        else if (distToPlayer > IdleStateEnterDist)
        {
            currState = Enemy.EnemyMoveState.IdleState;
            return;
        }

        MoveTowards(playerPosition, moveSpeed);
    }

    protected override void CloseStateUpdate()
    {
        Vector2 playerPosition = GameManager.instance.player.transform.position;
        float distToPlayer = Vector2.Distance(playerPosition, transform.position);

        if (distToPlayer > CloseStateLeaveDist)
        {
            currState = Enemy.EnemyMoveState.ChaseState;
            return;
        }

        if (closeMoveState == CloseMoveState.Charging)
        {
            Vector2 directionToPlayer = playerPosition - (Vector2)transform.position;

            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(
                    directionToPlayer.y,
                    directionToPlayer.x
                ) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            dashTimer += Time.deltaTime;

            if (dashTimer >= dashCooldown * (hitPlayer ? 2 : 1))
            {
                Vector2 dashDirection = transform.right;

                dashTargetPosition =
                    (Vector2)transform.position +
                    dashDirection * dashDistance;

                closeMoveState = CloseMoveState.Dashing;
                hitPlayer = false;
            }
        }
        else
        {
            MoveTowards(dashTargetPosition, dashSpeed);

            if (Vector2.Distance(transform.position, dashTargetPosition) < 0.001f)
            {
                dashTimer = 0f;
                closeMoveState = CloseMoveState.Charging;
            }
        }
    }

    protected override void IdleStateUpdate()
    {
        if (Vector2.Distance(GameManager.instance.player.transform.position, transform.position) < ChaseStateEnterDist)
        {
            currState = Enemy.EnemyMoveState.ChaseState;
            return;
        }
    }

    private void EnterCloseState(Vector2 playerPosition)
    {
        dashTimer = 0f;
        closeMoveState = CloseMoveState.Charging;
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 newPosition = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        transform.position = new Vector3(
            newPosition.x,
            newPosition.y,
            transform.position.z
        );
    }
}
