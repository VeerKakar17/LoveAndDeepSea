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
        if (GameManager.instance.inDialogueState) return;

        switch (currState)
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
        if (GameManager.instance.inDialogueState) return;

        // handle rotation here for later
    }

    // Function called on collision enter with the player
    public virtual void OnPlayerCollision() { }

    protected virtual void IdleStateUpdate() { }

    protected virtual void ChaseStateUpdate() { }

    protected virtual void CloseStateUpdate() { }
}
