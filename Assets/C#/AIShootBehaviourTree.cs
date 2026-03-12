using System.Collections.Generic;

public class AIShootBehaviourTree
{
    public List<AIShootAction> GetCandidateActions(AIState state, AIMoveAction currentMoveAction)
    {
        if (state.DangerBullet == DangerBulletState.Yes)
        {
            return new List<AIShootAction>
            {
                AIShootAction.NoFire
            };
        }

        switch (currentMoveAction)
        {
            case AIMoveAction.Dodge:
                return new List<AIShootAction>
                {
                    AIShootAction.NoFire
                };

            case AIMoveAction.Retreat:
                return new List<AIShootAction>
                {
                    AIShootAction.NoFire,
                    AIShootAction.Fire
                };

            case AIMoveAction.Hold:
                return new List<AIShootAction>
                {
                    AIShootAction.Fire,
                    AIShootAction.NoFire
                };

            case AIMoveAction.Approach:
            case AIMoveAction.LeftMove:
            case AIMoveAction.RightMove:
            default:
                return new List<AIShootAction>
                {
                    AIShootAction.Fire,
                    AIShootAction.NoFire
                };
        }
    }
}