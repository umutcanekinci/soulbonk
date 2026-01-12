using UnityEngine;
using UnityEngine.AI;
using VectorViolet.Core.Stats;

public class ReturnState : EnemyState
{
    public override void Enter(EnemyAI enemy)
    {
        enemy.GoHome();
    }

    public override void Update(EnemyAI enemy)
    {
        // Geri dönerken oyuncuyu görürse tekrar kovala
        if (enemy.GetDistanceToTarget() < enemy.ChaseRange)
        {
            enemy.SwitchState(new ChaseState());
            return;
        }

        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance < 0.5f)
        {
            enemy.SwitchState(new IdleState());
        }
    }

    public override void Exit(EnemyAI enemy) { }
}

[RequireComponent(typeof(NavMeshAgent), typeof(StatHolder), typeof(EntityMovement))]
[RequireStat("AttackRange", "ChaseRange", "PatrolRange", "MoveSpeed")]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public EntityAttack EntityAttack;
    public EntityMovement EntityMovement;
    
    [Header("Settings")]
    public float PatrolWaitTime = 2f;

    // State Pattern
    private EnemyState _currentState;

    // Components & Data
    public NavMeshAgent Agent { get; private set; }
    public Transform _target { get; private set; }
    public Vector3 HomePosition { get; private set; }

    // Range Stats
    private StatBase _attackRangeStat, _chaseRangeStat, _patrolRangeStat, _moveSpeedStat;
    
    public float AttackRange => _attackRangeStat.GetValue();
    public float ChaseRange => _chaseRangeStat.GetValue();
    public float PatrolRange => _patrolRangeStat.GetValue();

    private void Awake()
    {
        EntityMovement = GetComponent<EntityMovement>();
        EntityMovement.usePhysicsMovement = false;
        
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    private void Start()
    {
        var statHolder = GetComponent<StatHolder>();
        _attackRangeStat = statHolder.GetStat("AttackRange");
        _chaseRangeStat = statHolder.GetStat("ChaseRange");
        _patrolRangeStat = statHolder.GetStat("PatrolRange");
        _moveSpeedStat = statHolder.GetStat("MoveSpeed");
        _moveSpeedStat.OnValueChanged += SetSpeed;

        HomePosition = transform.position;

        if (_target == null) DetectPlayer();

        SetSpeed(_moveSpeedStat);
        SwitchState(new IdleState());
    }

    private void OnDestroy()
    {
        _moveSpeedStat.OnValueChanged -= SetSpeed;
    }

    private void SetSpeed(StatBase stat)
    {
        Agent.speed = stat.GetValue();
    }

    private void Update()
    {
        if (_target == null || !Agent.isOnNavMesh) return;
        
        // Eğer saldırı animasyonu oynuyorsa hareket etme (Global kural)
        if (EntityAttack != null && EntityAttack.IsAttacking)
            return;

        _currentState?.Update(this);
        EntityMovement.SetMoveInput(Agent.velocity);
    }

    public void SwitchState(EnemyState newState)
    {
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState?.Enter(this);
    }

    // State'lerin sık kullandığı işlemleri buraya Helper method olarak koyabiliriz
    public float GetDistanceToTarget()
    {
        if (_target == null)
            return float.MaxValue;
        
        return Vector2.Distance(transform.position, _target.position);
    }

    public void GoToRandomPoint()
    {
        Vector2 randomDir = Random.insideUnitCircle * PatrolRange;
        Vector3 targetPos = HomePosition + new Vector3(randomDir.x, randomDir.y, 0);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 2f, NavMesh.AllAreas))
            Agent.SetDestination(hit.position);
        else
            Agent.SetDestination(HomePosition);
    }

    public void GoHome()
    {
        Agent.SetDestination(HomePosition);
    }

    private void DetectPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            SetTarget(playerObj.transform);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        if (EntityAttack != null)
            EntityAttack.target = target;
    }
}