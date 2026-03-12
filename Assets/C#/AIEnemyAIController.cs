using System.Collections.Generic;
using UnityEngine;

public class AIEnemyAIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody2D enemyRb;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Health enemyHealth;

    [Header("Decision")]
    [SerializeField] private float decisionInterval = 0.1f;
    [SerializeField] private bool enableDebugLog = true;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private float dodgeCooldown = 0.8f;

    [Header("Arena Bounds")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4.5f;
    [SerializeField] private float maxY = 4.5f;

    [Header("Shoot")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireInterval = 1.5f;

    private AIShootBehaviourTree shootBehaviourTree;
    private AIShootActionSelector shootActionSelector;
    private AIShootActuator shootActuator;

    private AIMoveWeightTable moveWeightTable;

    private float decisionTimer;

    private AIStateJudge stateJudge;
    private AIMoveBehaviourTree moveBehaviourTree;
    private AIMoveActionSelector moveActionSelector;
    private AIMoveActuator moveActuator;
    private AIBlackboard blackboard;

    private void Awake()
    {
        if (enemyRb == null) enemyRb = GetComponent<Rigidbody2D>();
        if (enemyHealth == null) enemyHealth = GetComponent<Health>();

        stateJudge = new AIStateJudge();
        moveBehaviourTree = new AIMoveBehaviourTree();

        moveWeightTable = new AIMoveWeightTable();
        moveActionSelector = new AIMoveActionSelector(moveWeightTable, 0.15f);

        moveActuator = new AIMoveActuator(
            enemyRb,
            transform,
            playerTransform,
            moveSpeed,
            dodgeSpeed,
            dodgeDuration,
            dodgeCooldown,
            minX,
            maxX,
            minY,
            maxY
        );

        shootBehaviourTree = new AIShootBehaviourTree();
        shootActionSelector = new AIShootActionSelector();
        shootActuator = new AIShootActuator(firePoint, bulletPrefab, fireInterval);

        blackboard = new AIBlackboard();
    }

    private void Update()
    {
        if (!GameManager.roundActive) return;
        if (playerTransform == null) return;

        decisionTimer += Time.deltaTime;
        shootActuator.Tick(Time.deltaTime);

        if (decisionTimer >= decisionInterval)
        {
            decisionTimer = 0f;
            TickDecision();
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.roundActive) return;
        if (playerTransform == null) return;

        moveActuator.Tick(Time.fixedDeltaTime, blackboard);
    }

    private void TickDecision()
    {
        if (Time.time < blackboard.MoveActionLockUntil)
        {
            return;
        }

        AIDecisionContext context = BuildContext();

        AIState state = stateJudge.JudgeState(context);
        blackboard.CurrentState = state;

        List<AIMoveAction> candidates = moveBehaviourTree.GetCandidateActions(state,blackboard.PreviousMoveAction,context);
        List<AIShootAction> shootCandidates = shootBehaviourTree.GetCandidateActions(state, blackboard.CurrentMoveAction);

        AIMoveAction selectedAction = moveActionSelector.SelectAction(state.StateId, candidates, state);
        AIShootAction selectedShootAction = shootActionSelector.Select(shootCandidates, state, blackboard.CurrentMoveAction);

        blackboard.PreviousMoveAction = blackboard.CurrentMoveAction;
        blackboard.CurrentMoveAction = selectedAction;
        blackboard.PreviousShootAction = blackboard.CurrentShootAction;
        blackboard.CurrentShootAction = selectedShootAction;
        blackboard.LastDecisionTime = Time.time;
        blackboard.MoveActionLockUntil = Time.time + GetMoveActionDuration(selectedAction);
        
        moveActuator.SetAction(selectedAction, blackboard);
        shootActuator.Execute(selectedShootAction);

        if (enableDebugLog && selectedAction != blackboard.PreviousMoveAction)
        {
            string candidateText = string.Join(", ", candidates);

            Debug.Log(
                $"<color=cyan>[MoveAI]</color> {state} <color=yellow>{blackboard.PreviousMoveAction} -> {selectedAction}</color> Candidates=[{candidateText}]",
                this
            );
        }
    }

    private float GetMoveActionDuration(AIMoveAction action)
    {
        switch (action)
        {
            case AIMoveAction.Hold:
                return 0.15f;

            case AIMoveAction.Approach:
            case AIMoveAction.Retreat:
                return 0.35f;

            case AIMoveAction.LeftMove:
            case AIMoveAction.RightMove:
                return 0.30f;

            case AIMoveAction.Dodge:
                return dodgeDuration;

            default:
                return 0.2f;
        }
    }

    private AIDecisionContext BuildContext()
    {
        return new AIDecisionContext
        {
            EnemyTransform = transform,
            PlayerTransform = playerTransform,
            EnemyRb = enemyRb,
            PlayerRb = playerRb,
            EnemyHealth = enemyHealth,
            PlayerBullets = GetPlayerBullets(),

            ArenaMinX = minX,
            ArenaMaxX = maxX,
            ArenaMinY = minY,
            ArenaMaxY = maxY
        };
    }

    private List<Bullet> GetPlayerBullets()
    {
        Bullet[] allBullets = Object.FindObjectsOfType<Bullet>();
        List<Bullet> playerBullets = new List<Bullet>();

        for (int i = 0; i < allBullets.Length; i++)
        {
            if (allBullets[i] != null && allBullets[i].ownerTag == "Player")
            {
                playerBullets.Add(allBullets[i]);
            }
        }

        return playerBullets;
    }
}