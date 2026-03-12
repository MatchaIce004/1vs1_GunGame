using System.Collections.Generic;
using UnityEngine;

public class AIMoveBehaviourTree
{
    private const float edgeMargin = 1.0f;

    public List<AIMoveAction> GetCandidateActions(
        AIState state,
        AIMoveAction previousAction,
        AIDecisionContext context)
    {
        List<AIMoveAction> candidates;

        if (state.DangerBullet == DangerBulletState.Yes)
        {
            candidates = GetDangerCandidates();
        }
        else
        {
            switch (state.Distance)
            {
                case DistanceState.Near:
                    candidates = GetNearCandidates();
                    break;

                case DistanceState.Mid:
                    candidates = GetMidCandidates();
                    break;

                case DistanceState.Far:
                    candidates = GetFarCandidates();
                    break;

                default:
                    candidates = new List<AIMoveAction> { AIMoveAction.Hold };
                    break;
            }
        }

        AdjustCandidatesByPreviousAction(candidates, previousAction, state);
        AdjustCandidatesByArenaEdge(candidates, context);

        if (candidates.Count == 0)
        {
            candidates.Add(AIMoveAction.Hold);
        }

        return candidates;
    }

    private void AdjustCandidatesByPreviousAction(List<AIMoveAction> candidates, AIMoveAction previousAction, AIState state)
    {
        if (state.DangerBullet == DangerBulletState.Yes)
        {
            if (previousAction == AIMoveAction.Dodge && candidates.Contains(AIMoveAction.Dodge))
            {
                candidates.Remove(AIMoveAction.Dodge);
            }

            if (previousAction == AIMoveAction.Retreat && candidates.Contains(AIMoveAction.Retreat))
            {
                candidates.Remove(AIMoveAction.Retreat);
            }

            if (candidates.Count == 0)
            {
                candidates.Add(AIMoveAction.LeftMove);
                candidates.Add(AIMoveAction.RightMove);
            }
        }
    }

    private void AdjustCandidatesByArenaEdge(List<AIMoveAction> candidates, AIDecisionContext context)
    {
        if (context == null || context.EnemyTransform == null)
        {
            return;
        }

        Vector2 enemyPos = context.EnemyTransform.position;

        bool nearLeft = enemyPos.x <= context.ArenaMinX + edgeMargin;
        bool nearRight = enemyPos.x >= context.ArenaMaxX - edgeMargin;
        bool nearBottom = enemyPos.y <= context.ArenaMinY + edgeMargin;
        bool nearTop = enemyPos.y >= context.ArenaMaxY - edgeMargin;

        if (nearLeft)
        {
            candidates.Remove(AIMoveAction.LeftMove);
        }

        if (nearRight)
        {
            candidates.Remove(AIMoveAction.RightMove);
        }

        // 端に近い時は、その場維持しやすくする
        if ((nearLeft || nearRight || nearTop || nearBottom) && !candidates.Contains(AIMoveAction.Hold))
        {
            candidates.Add(AIMoveAction.Hold);
        }

        // 上下端では外側に逃げ続けるのを少し抑える
        if ((nearTop || nearBottom) && candidates.Contains(AIMoveAction.Dodge))
        {
            // Dodgeは残してもよいが、最初は端で暴れやすいので一旦消す
            candidates.Remove(AIMoveAction.Dodge);
        }
    }

    private List<AIMoveAction> GetDangerCandidates()
    {
        return new List<AIMoveAction>
        {
            AIMoveAction.LeftMove,
            AIMoveAction.RightMove,
            AIMoveAction.Retreat,
            AIMoveAction.Dodge
        };
    }

    private List<AIMoveAction> GetNearCandidates()
    {
        return new List<AIMoveAction>
        {
            AIMoveAction.Retreat,
            AIMoveAction.LeftMove,
            AIMoveAction.RightMove
        };
    }

    private List<AIMoveAction> GetMidCandidates()
    {
        return new List<AIMoveAction>
        {
            AIMoveAction.Approach,
            AIMoveAction.Retreat,
            AIMoveAction.LeftMove,
            AIMoveAction.RightMove,
            AIMoveAction.Hold
        };
    }

    private List<AIMoveAction> GetFarCandidates()
    {
        return new List<AIMoveAction>
        {
            AIMoveAction.Approach,
            AIMoveAction.LeftMove,
            AIMoveAction.RightMove,
            AIMoveAction.Hold
        };
    }
}