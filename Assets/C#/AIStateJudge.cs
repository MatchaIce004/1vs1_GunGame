using UnityEngine;

public class AIStateJudge
{
    private readonly float nearDistance;
    private readonly float midDistance;
    private readonly float facingAngleThreshold;
    private readonly float lateralMoveThreshold;
    private readonly float lowHpThreshold;

    private readonly float dangerAbsDx;
    private readonly float dangerMinDy;
    private readonly float dangerMaxDy;

    public AIStateJudge(
        float nearDistance = 4.0f,
        float midDistance = 10.0f,
        float facingAngleThreshold = 60.0f,
        float lateralMoveThreshold = 1.0f,
        float lowHpThreshold = 0.4f,
        float dangerAbsDx = 1.0f,
        float dangerMinDy = 0.0f,
        float dangerMaxDy = 3.0f)
    {
        this.nearDistance = nearDistance;
        this.midDistance = midDistance;
        this.facingAngleThreshold = facingAngleThreshold;
        this.lateralMoveThreshold = lateralMoveThreshold;
        this.lowHpThreshold = lowHpThreshold;
        this.dangerAbsDx = dangerAbsDx;
        this.dangerMinDy = dangerMinDy;
        this.dangerMaxDy = dangerMaxDy;
    }

    public AIState JudgeState(AIDecisionContext context)
    {
        AIState state = new AIState
        {
            Distance = JudgeDistance(context),
            Facing = JudgeFacing(context),
            LateralMove = JudgeLateralMove(context),
            Hp = JudgeHp(context),
            DangerBullet = JudgeDangerBullet(context)
        };

        state.StateId = BuildStateId(state);
        return state;
    }

    private DistanceState JudgeDistance(AIDecisionContext context)
    {
        float d = Vector2.Distance(context.EnemyTransform.position, context.PlayerTransform.position);

        if (d <= nearDistance) return DistanceState.Near;
        if (d <= midDistance) return DistanceState.Mid;
        return DistanceState.Far;
    }

    private FacingState JudgeFacing(AIDecisionContext context)
    {
        Vector2 playerForward = GetPlayerForward(context);
        Vector2 toEnemy = ((Vector2)context.EnemyTransform.position - (Vector2)context.PlayerTransform.position).normalized;
        float angle = Vector2.Angle(playerForward, toEnemy);

        return angle <= facingAngleThreshold ? FacingState.Yes : FacingState.No;
    }

    private Vector2 GetPlayerForward(AIDecisionContext context)
    {
        return context.PlayerTransform.up;
    }

    private LateralMoveState JudgeLateralMove(AIDecisionContext context)
    {
        if (context.PlayerRb == null) return LateralMoveState.None;

        float vx = context.PlayerRb.velocity.x;

        if (vx <= -lateralMoveThreshold) return LateralMoveState.Left;
        if (vx >= lateralMoveThreshold) return LateralMoveState.Right;
        return LateralMoveState.None;
    }

    private HpState JudgeHp(AIDecisionContext context)
    {
        if (context.EnemyHealth == null || context.EnemyHealth.maxHP <= 0)
        {
            return HpState.High;
        }

        float hpRate = (float)context.EnemyHealth.currentHP / context.EnemyHealth.maxHP;
        return hpRate < lowHpThreshold ? HpState.Low : HpState.High;
    }

    private DangerBulletState JudgeDangerBullet(AIDecisionContext context)
    {
        if (context.PlayerBullets == null || context.PlayerBullets.Count == 0)
        {
            return DangerBulletState.No;
        }

        Vector2 enemyPos = context.EnemyTransform.position;

        for (int i = 0; i < context.PlayerBullets.Count; i++)
        {
            Bullet bullet = context.PlayerBullets[i];
            if (bullet == null || !bullet.gameObject.activeInHierarchy) continue;

            if (IsDangerBullet(bullet, enemyPos))
            {
                return DangerBulletState.Yes;
            }
        }

        return DangerBulletState.No;
    }

    private bool IsDangerBullet(Bullet bullet, Vector2 enemyPos)
    {
        Vector2 bulletPos = bullet.transform.position;
        Vector2 bulletDir = ((Vector2)bullet.transform.up).normalized;
        Vector2 toEnemy = enemyPos - bulletPos;

        if (toEnemy.magnitude > 10.0f)
        {
            return false;
        }

        float forwardDistance = Vector2.Dot(toEnemy, bulletDir);
        if (forwardDistance < 0f || forwardDistance > 5.0f)
        {
            return false;
        }

        Vector2 closestPoint = bulletPos + bulletDir * forwardDistance;
        float sideDistance = Vector2.Distance(enemyPos, closestPoint);

        return sideDistance <= 4.0f;
    }

    public int BuildStateId(AIState state)
    {
        int distance = (int)state.Distance;
        int facing = (int)state.Facing;
        int move = (int)state.LateralMove;
        int hp = (int)state.Hp;
        int danger = (int)state.DangerBullet;

        int stateId =
            (distance * 2 * 3 * 2 * 2) +
            (facing * 3 * 2 * 2) +
            (move * 2 * 2) +
            (hp * 2) +
            danger;

        return stateId;
    }
}