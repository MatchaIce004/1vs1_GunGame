using System.Collections.Generic;

public class AIBehaviourTree
{
    public List<AIActionType> GetCandidateActions(AIState state, AIActionType previousAction)
    {
        List<AIActionType> candidates;

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
                    candidates = new List<AIActionType> { AIActionType.Approach };
                    break;
            }
        }

        AdjustCandidatesByPreviousAction(candidates, previousAction, state);

        return candidates;
    }

    private void AdjustCandidatesByPreviousAction(List<AIActionType> candidates, AIActionType previousAction, AIState state)
    {
        if (state.DangerBullet == DangerBulletState.Yes)
        {
            if (previousAction == AIActionType.Dodge && candidates.Contains(AIActionType.Dodge))
            {
                candidates.Remove(AIActionType.Dodge);
            }

            if (previousAction == AIActionType.Retreat && candidates.Contains(AIActionType.Retreat))
            {
                candidates.Remove(AIActionType.Retreat);
            }

            if (candidates.Count == 0)
            {
                candidates.Add(AIActionType.LeftMove);
                candidates.Add(AIActionType.RightMove);
            }
        }
    }

    private List<AIActionType> GetDangerCandidates()
    {
        return new List<AIActionType>
        {
            AIActionType.LeftMove,
            AIActionType.RightMove,
            AIActionType.Retreat,
            AIActionType.Dodge
        };
    }

    private List<AIActionType> GetNearCandidates()
    {
        return new List<AIActionType>
        {
            AIActionType.Retreat,
            AIActionType.LeftMove,
            AIActionType.RightMove,
            AIActionType.MoveShoot
        };
    }

    private List<AIActionType> GetMidCandidates()
    {
        return new List<AIActionType>
        {
            AIActionType.LeftMove,
            AIActionType.RightMove,
            AIActionType.StopShoot,
            AIActionType.MoveShoot,
            AIActionType.Approach,
            AIActionType.Retreat
        };
    }

    private List<AIActionType> GetFarCandidates()
    {
        return new List<AIActionType>
        {
            AIActionType.Approach,
            AIActionType.LeftMove,
            AIActionType.RightMove,
            AIActionType.MoveShoot
        };
    }
}