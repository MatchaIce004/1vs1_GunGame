using System.Collections.Generic;
using UnityEngine;

public class AIActionSelector
{
    public AIActionType SelectRandom(List<AIActionType> candidates)
    {
        if (candidates == null || candidates.Count == 0)
        {
            return AIActionType.Approach;
        }

        int index = Random.Range(0, candidates.Count);
        return candidates[index];
    }
}