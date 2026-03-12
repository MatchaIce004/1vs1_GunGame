using UnityEngine;

public class AIEnemyActuator
{
    private readonly Rigidbody2D rb;
    private readonly Transform enemyTransform;
    private readonly Transform playerTransform;
    private readonly Transform firePoint;
    private readonly GameObject bulletPrefab;

    private readonly float moveSpeed;
    private readonly float dodgeSpeed;
    private readonly float dodgeDuration;
    private readonly float dodgeCooldown;
    private readonly float fireInterval;

    private float dodgeTimer;
    private bool isDodging;
    private Vector2 dodgeDirection;

    private float fireTimer;

    private AIActionType currentAction;
    private bool hasAction;

    public AIEnemyActuator(
        Rigidbody2D rb,
        Transform enemyTransform,
        Transform playerTransform,
        Transform firePoint,
        GameObject bulletPrefab,
        float moveSpeed = 2f,
        float dodgeSpeed = 8f,
        float dodgeDuration = 0.25f,
        float dodgeCooldown = 0.8f,
        float fireInterval = 1.5f)
    {
        this.rb = rb;
        this.enemyTransform = enemyTransform;
        this.playerTransform = playerTransform;
        this.firePoint = firePoint;
        this.bulletPrefab = bulletPrefab;
        this.moveSpeed = moveSpeed;
        this.dodgeSpeed = dodgeSpeed;
        this.dodgeDuration = dodgeDuration;
        this.dodgeCooldown = dodgeCooldown;
        this.fireInterval = fireInterval;
    }

    public void SetAction(AIActionType action, AIBlackboard blackboard)
    {
        if (isDodging)
        {
            return;
        }

        currentAction = action;
        hasAction = true;

        if (action == AIActionType.Dodge)
        {
            StartDodge(blackboard);
        }
    }

    public void Tick(float deltaTime, AIBlackboard blackboard)
    {
        fireTimer += deltaTime;

        if (isDodging)
        {
            dodgeTimer -= deltaTime;
            rb.MovePosition(rb.position + dodgeDirection * dodgeSpeed * deltaTime);

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
            case AIActionType.Approach:
                DoApproach(deltaTime);
                break;

            case AIActionType.Retreat:
                DoRetreat(deltaTime);
                break;

            case AIActionType.LeftMove:
                DoLeftMove(deltaTime);
                break;

            case AIActionType.RightMove:
                DoRightMove(deltaTime);
                break;

            case AIActionType.StopShoot:
                DoStopShoot();
                break;

            case AIActionType.MoveShoot:
                DoMoveShoot(deltaTime);
                break;
        }
    }

    private void LookAtPlayer()
    {
        Vector2 dir = playerTransform.position - enemyTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        enemyTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void TryShoot()
    {
        if (fireTimer < fireInterval) return;
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.ownerTag = "Enemy";
        }

        fireTimer = 0f;
    }

    private void DoApproach(float deltaTime)
    {
        LookAtPlayer();
        Vector2 dir = ((Vector2)playerTransform.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * deltaTime);
    }

    private void DoRetreat(float deltaTime)
    {
        LookAtPlayer();
        Vector2 dir = (rb.position - (Vector2)playerTransform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * deltaTime);
    }

    private void DoLeftMove(float deltaTime)
    {
        LookAtPlayer();
        Vector2 side = -(Vector2)enemyTransform.right;
        rb.MovePosition(rb.position + side * moveSpeed * deltaTime);
    }

    private void DoRightMove(float deltaTime)
    {
        LookAtPlayer();
        Vector2 side = (Vector2)enemyTransform.right;
        rb.MovePosition(rb.position + side * moveSpeed * deltaTime);
    }

    private void DoStopShoot()
    {
        LookAtPlayer();
        TryShoot();
    }

    private void DoMoveShoot(float deltaTime)
    {
        LookAtPlayer();

        float dir = Random.value < 0.5f ? -1f : 1f;
        Vector2 side = (Vector2)enemyTransform.right * dir;

        rb.MovePosition(rb.position + side * moveSpeed * deltaTime);
        TryShoot();
    }

    private void StartDodge(AIBlackboard blackboard)
    {
        if (Time.time - blackboard.LastDodgeTime < dodgeCooldown)
        {
            currentAction = AIActionType.Retreat;
            return;
        }

        LookAtPlayer();

        float dir = Random.value < 0.5f ? -1f : 1f;
        dodgeDirection = (Vector2)enemyTransform.right * dir;

        isDodging = true;
        dodgeTimer = dodgeDuration;
        blackboard.LastDodgeTime = Time.time;
    }
}