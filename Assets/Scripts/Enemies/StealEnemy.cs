using Unity.VisualScripting;
using UnityEngine;

public class StealEnemy : Enemy
{
    [SerializeField] private float chaseEnterDist = 5f;
    [SerializeField] private float maxChaseDistFromSpawn = 10f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float returnSpeedMultiplier = 1.25f;
    [SerializeField] private float treasureChaseSpeedMultiplier = 1.15f;
    [SerializeField] private float spawnPositionThreshold = 0.05f;

    private Vector2 spawnPosition;

    private bool hasTreasure;
    private Treasure currTreasure;

    private float dropWaitTimer = 0;
    private float dropWaitCooldown = 0.75f;
    private float chaseDropWaitTimer = 1;
    private float chaseDropWaitCooldown = 1;

    private void Start()
    {
        spawnPosition = transform.position;
        currState = Enemy.EnemyMoveState.IdleState;
        currTreasure = null;
    }

    protected override void IdleStateUpdate()
    {
        float speed = hasTreasure
            ? moveSpeed * returnSpeedMultiplier
            : moveSpeed;

        if (Vector2.Distance(transform.position, spawnPosition) > spawnPositionThreshold)
        {
            MoveTowards(spawnPosition, speed);
            return;
        }

        transform.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            transform.position.z
        );

        if (!hasTreasure &&
            Vector2.Distance(
                GameManager.instance.player.transform.position,
                transform.position
            ) < chaseEnterDist &&
            GameManager.instance.player.HasTreasure)
        {
            currState = Enemy.EnemyMoveState.ChaseState;
        }
    }

    protected override void ChaseStateUpdate()
    {
        chaseDropWaitTimer += Time.deltaTime;
        if (chaseDropWaitTimer < chaseDropWaitCooldown)
        {
            return;
        }

        float distFromSpawn = Vector2.Distance(
            transform.position,
            spawnPosition
        );

        if (distFromSpawn >= maxChaseDistFromSpawn ||
            !GameManager.instance.player.HasTreasure)
        {
            currState = Enemy.EnemyMoveState.IdleState;
            return;
        }

        currTreasure = GameManager.instance.player.heldTreasure;

        MoveTowards(
            GameManager.instance.player.transform.position,
            moveSpeed
        );
    }

    protected override void CloseStateUpdate()
    {
        if (currTreasure == null || GameManager.instance.player.HasTreasure)
        {
            currState = Enemy.EnemyMoveState.ChaseState;
            return;
        }

        dropWaitTimer += Time.deltaTime;
        if (dropWaitTimer < dropWaitCooldown)
        {
            return;
        }

        MoveTowards(
            currTreasure.transform.position,
            moveSpeed * treasureChaseSpeedMultiplier
        );
    }

    protected override void OnPlayerCollision()
    {
        if (hasTreasure)
        {
            currTreasure.gameObject.SetActive(true);
            GameManager.instance.player.HoldTreasure(currTreasure);

            Treasure treasure = currTreasure.GetComponent<Treasure>();
            treasure.TryPickUp(GameManager.instance.player.gameObject);

            hasTreasure = false;
            currState = Enemy.EnemyMoveState.ChaseState;
            chaseDropWaitTimer = 0;
        }
        else if (
            currState == Enemy.EnemyMoveState.ChaseState &&
            GameManager.instance.player.HasTreasure
        )
        {
            GameManager.instance.player.DropTreasure();

            currState = Enemy.EnemyMoveState.CloseState;
            dropWaitTimer = 0;
        }

        if (!hasTreasure)
        {
            return;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (currState != Enemy.EnemyMoveState.CloseState)
        {
            return;
        }

        if (!other.CompareTag("Treasure"))
        {
            return;
        }

        Treasure treasure = other.GetComponent<Treasure>();

        if (treasure == null || treasure != currTreasure)
        {
            return;
        }

        if (!treasure.TryPickUp(gameObject))
        {
            return;
        }

        hasTreasure = true;
        currState = Enemy.EnemyMoveState.IdleState;
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