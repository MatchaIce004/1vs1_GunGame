using UnityEngine;

public class AIMoveWeightTable
{
    public const int StateCount = 72;
    public const int ActionCount = 6;

    private readonly float[,] weights = new float[StateCount, ActionCount];

    public AIMoveWeightTable()
    {
        Initialize(1.0f);
    }

    public void Initialize(float defaultWeight)
    {
        for (int s = 0; s < StateCount; s++)
        {
            for (int a = 0; a < ActionCount; a++)
            {
                weights[s, a] = defaultWeight;
            }
        }
    }

    public float GetWeight(int stateId, AIMoveAction action)
    {
        return weights[stateId, (int)action];
    }

    public void SetWeight(int stateId, AIMoveAction action, float value)
    {
        weights[stateId, (int)action] = value;
    }
}