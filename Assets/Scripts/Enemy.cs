using UnityEngine;

public class Enemy : MapElement
{
    protected EnemyMoveState currState;
    protected Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        currState = EnemyMoveState.IdleState;
        
        base.Start();
    }

    protected enum EnemyMoveState
    {
        IdleState,
        ChaseState,
        CloseState,
    }

    private void Update()
    {
        switch(currState)
        {
            case EnemyMoveState.IdleState:
                IdleStateUpdate();
                break;
            case EnemyMoveState.ChaseState:
                ChaseStateUpdate();
                break;
            case EnemyMoveState.CloseState:
                CloseStateUpdate();
                break;
        }
    }

    private void FixedUpdate()
    {
        // handle rotation here for later
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            OnPlayerCollision();
        }
    }

    // Function called on collision enter with the player
    protected virtual void OnPlayerCollision() { }

    protected virtual void IdleStateUpdate() { }

    protected virtual void ChaseStateUpdate() { }

    protected virtual void CloseStateUpdate() { }
}
