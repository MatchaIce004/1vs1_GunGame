using UnityEngine;

public class AIMoveActuator
{
    private readonly Rigidbody2D rb;
    private readonly Transform enemyTransform;
    private readonly Transform playerTransform;

    private readonly float moveSpeed;
    private readonly float dodgeSpeed;
    private readonly float dodgeDuration;
    private readonly float dodgeCooldown;

    private readonly float minX;
    private readonly float maxX;
    private readonly float minY;
    private readonly float maxY;

    private float dodgeTimer;
    private bool isDodging;
    private Vector2 dodgeDirection;

    private AIMoveAction currentAction = AIMoveAction.Hold;
    private bool hasAction;

    public AIMoveActuator(
        Rigidbody2D rb,
        Transform enemyTransform,
        Transform playerTransform,
        float moveSpeed = 4f,
        float dodgeSpeed = 8f,
        float dodgeDuration = 0.25f,
        float dodgeCooldown = 0.8f,
        float minX = -8f,
        float maxX = 8f,
        float minY = -4.5f,
        float maxY = 4.5f)
    {
        this.rb = rb;
        this.enemyTransform = enemyTransform;
        this.playerTransform = playerTransform;
        this.moveSpeed = moveSpeed;
        this.dodgeSpeed = dodgeSpeed;
        this.dodgeDuration = dodgeDuration;
        this.dodgeCooldown = dodgeCooldown;

        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    public void SetAction(AIMoveAction action, AIBlackboard blackboard)
    {
        if (isDodging)
        {
            return;
        }

        currentAction = action;
        hasAction = true;

        if (action == AIMoveAction.Dodge)
        {
            StartDodge(blackboard);
        }
    }

    public void Tick(float deltaTime, AIBlackboard blackboard)
    {
        if (isDodging)
        {
            dodgeTimer -= deltaTime;
            MoveBy(dodgeDirection, dodgeSpeed, deltaTime);

            if (dodgeTimer <= 0f)
            {
                isDodging = false;
            }

            return;
        }

        if (!hasAction)
        {
            return;
        }

        ExecuteCurrentAction(deltaTime);
    }

    private void ExecuteCurrentAction(float deltaTime)
    {
        switch (currentAction)
        {
            case AIMoveAction.Hold:
                DoHold();
                break;

            case AIMoveAction.Approach:
                DoApproach(deltaTime);
                break;

            case AIMoveAction.Retreat:
                DoRetreat(deltaTime);
                break;

            case AIMoveAction.LeftMove:
                DoLeftMove(deltaTime);
                break;

            case AIMoveAction.RightMove:
                DoRightMove(deltaTime);
                break;
        }
    }

    private void LookAtPlayer()
    {
        Vector2 dir = playerTransform.position - enemyTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        enemyTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void DoHold()
    {
        LookAtPlayer();
    }

    private void DoApproach(float deltaTime)
    {
        LookAtPlayer();
        Vector2 dir = ((Vector2)playerTransform.position - rb.position).normalized;
        MoveBy(dir, moveSpeed, deltaTime);
    }

    private void DoRetreat(float deltaTime)
    {
        LookAtPlayer();
        Vector2 dir = (rb.position - (Vector2)playerTransform.position).normalized;
        MoveBy(dir, moveSpeed, deltaTime);
    }

    private void DoLeftMove(float deltaTime)
    {
        LookAtPlayer();

        Vector2 radial = (rb.position - (Vector2)playerTransform.position).normalized;
        Vector2 tangentLeft = new Vector2(-radial.y, radial.x);

        Vector2 moveDir = tangentLeft;
        float distance = Vector2.Distance(rb.position, playerTransform.position);

        if (distance < 2.2f)
        {
            moveDir = (tangentLeft + radial * 0.6f).normalized;
        }

        MoveBy(moveDir, moveSpeed, deltaTime);
    }

    private void DoRightMove(float deltaTime)
    {
        LookAtPlayer();

        Vector2 radial = (rb.position - (Vector2)playerTransform.position).normalized;
        Vector2 tangentRight = new Vector2(radial.y, -radial.x);

        Vector2 moveDir = tangentRight;
        float distance = Vector2.Distance(rb.position, playerTransform.position);

        if (distance < 2.2f)
        {
            moveDir = (tangentRight + radial * 0.6f).normalized;
        }

        MoveBy(moveDir, moveSpeed, deltaTime);
    }

    private void StartDodge(AIBlackboard blackboard)
    {
        if (Time.time - blackboard.LastDodgeTime < dodgeCooldown)
        {
            currentAction = AIMoveAction.Retreat;
            return;
        }

        LookAtPlayer();

        Vector2 radial = (rb.position - (Vector2)playerTransform.position).normalized;
        Vector2 tangent = Random.value < 0.5f
            ? new Vector2(-radial.y, radial.x)
            : new Vector2(radial.y, -radial.x);

        dodgeDirection = tangent.normalized;

        isDodging = true;
        dodgeTimer = dodgeDuration;
        blackboard.LastDodgeTime = Time.time;
    }

    private void MoveBy(Vector2 direction, float speed, float deltaTime)
    {
        Vector2 nextPos = rb.position + direction.normalized * speed * deltaTime;
        nextPos = ClampToArena(nextPos);
        rb.MovePosition(nextPos);
    }

    private Vector2 ClampToArena(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }
}