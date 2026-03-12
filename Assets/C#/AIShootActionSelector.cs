using System.Collections.Generic;
using UnityEngine;

public class AIShootActionSelector
{
    public AIShootAction Select(List<AIShootAction> candidates, AIState state, AIMoveAction moveAction)
    {
        if (candidates == null || candidates.Count == 0)
        {
            return AIShootAction.NoFire;
        }

        if (!candidates.Contains(AIShootAction.Fire))
        {
            return AIShootAction.NoFire;
        }

        float fireChance = GetFireChance(state, moveAction);
        return Random.value < fireChance ? AIShootAction.Fire : AIShootAction.NoFire;
    }

    private float GetFireChance(AIState state, AIMoveAction moveAction)
    {
        float chance = 0.5f;

        if (state.DangerBullet == DangerBulletState.Yes)
        {
            return 0f;
        }

        switch (state.Distance)
        {
            case DistanceState.Near:
                chance = 0.35f;
                break;
            case DistanceState.Mid:
                chance = 0.75f;
                break;
            case DistanceState.Far:
                chance = 0.45f;
                break;
        }

        switch (moveAction)
        {
            case AIMoveAction.Hold:
                chance += 0.2f;
                break;

            case AIMoveAction.Retreat:
                chance -= 0.2f;
                break;

            case AIMoveAction.Dodge:
                chance = 0f;
                break;
        }

        return Mathf.Clamp01(chance);
    }
}