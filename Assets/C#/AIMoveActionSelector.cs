using System.Collections.Generic;
using UnityEngine;

public class AIMoveActionSelector
{
    private readonly AIMoveWeightTable weightTable;
    private readonly float explorationRate;

    public AIMoveActionSelector(AIMoveWeightTable weightTable, float explorationRate = 0.15f)
    {
        this.weightTable = weightTable;
        this.explorationRate = explorationRate;
    }

    public AIMoveAction SelectAction(int stateId, List<AIMoveAction> candidates, AIState state)
    {
        if (candidates == null || candidates.Count == 0)
        {
            return AIMoveAction.Hold;
        }

        if (Random.value < explorationRate)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            return candidates[randomIndex];
        }

        return SelectWeighted(stateId, candidates, state);
    }

    private AIMoveAction SelectWeighted(int stateId, List<AIMoveAction> candidates, AIState state)
    {
        float totalWeight = 0f;
        List<float> adjustedWeights = new List<float>();

        for (int i = 0; i < candidates.Count; i++)
        {
            AIMoveAction action = candidates[i];
            float weight = weightTable.GetWeight(stateId, action);

            weight *= GetContextMultiplier(state, action);

            adjustedWeights.Add(weight);
            totalWeight += weight;
        }

        if (totalWeight <= 0f)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            return candidates[randomIndex];
        }

        float r = Random.Range(0f, totalWeight);

        for (int i = 0; i < candidates.Count; i++)
        {
            r -= adjustedWeights[i];
            if (r <= 0f)
            {
                return candidates[i];
            }
        }

        return candidates[candidates.Count - 1];
    }

    private float GetContextMultiplier(AIState state, AIMoveAction action)
    {
        float m = 1.0f;

        if (state.DangerBullet == DangerBulletState.Yes)
        {
            if (action == AIMoveAction.Dodge) m *= 1.3f;
            if (action == AIMoveAction.Retreat) m *= 1.15f;
            if (action == AIMoveAction.Hold) m *= 0.4f;
        }

        switch (state.Distance)
        {
            case DistanceState.Near:
                if (action == AIMoveAction.Retreat) m *= 1.3f;
                if (action == AIMoveAction.Approach) m *= 0.5f;
                if (action == AIMoveAction.Hold) m *= 0.7f;
                break;

            case DistanceState.Mid:
                if (action == AIMoveAction.LeftMove || action == AIMoveAction.RightMove) m *= 1.1f;
                break;

            case DistanceState.Far:
                if (action == AIMoveAction.Approach) m *= 1.35f;
                if (action == AIMoveAction.Hold) m *= 0.7f;
                break;
        }

        return m;
    }
}