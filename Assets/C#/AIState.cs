using System;

public enum DistanceState
{
    Near = 0,
    Mid = 1,
    Far = 2
}

public enum FacingState
{
    No = 0,
    Yes = 1
}

public enum LateralMoveState
{
    Left = 0,
    None = 1,
    Right = 2
}

public enum HpState
{
    Low = 0,
    High = 1
}

public enum DangerBulletState
{
    No = 0,
    Yes = 1
}

[Serializable]
public struct AIState
{
    public DistanceState Distance;
    public FacingState Facing;
    public LateralMoveState LateralMove;
    public HpState Hp;
    public DangerBulletState DangerBullet;
    public int StateId;

    public override string ToString()
    {
        return $"StateId={StateId} Dist={Distance} Facing={Facing} Move={LateralMove} Hp={Hp} Danger={DangerBullet}";
    }
}