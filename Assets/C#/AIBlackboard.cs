public class AIBlackboard
{
    public AIState CurrentState;

    public AIMoveAction CurrentMoveAction = AIMoveAction.Hold;
    public AIMoveAction PreviousMoveAction = AIMoveAction.Hold;

    public AIShootAction CurrentShootAction = AIShootAction.NoFire;
    public AIShootAction PreviousShootAction = AIShootAction.NoFire;

    public float LastDecisionTime;
    public float LastDodgeTime;

    public float MoveActionLockUntil;
}