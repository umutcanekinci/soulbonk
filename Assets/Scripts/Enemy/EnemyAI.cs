using UnityEngine;
using UnityEngine.AI;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(NavMeshAgent), typeof(StatHolder), typeof(EntityMovement))]
[RequireStat("AttackRange", "ChaseRange", "PatrolRange", "MoveSpeed")]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public EntityAttack EntityAttack { get; private set; }
    public EntityMovement EntityMovement { get; private set; }
    
    [Header("Settings")]
    public float PatrolWaitTime = 2f;

    private EnemyState _currentState;
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

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        Agent.isStopped = newState == GameState.Cutscene;

        if (newState != GameState.Gameplay)
        {
            EntityMovement.SetMoveInput(Vector2.zero);
        }
    }

    private void Start()
    {
        var statHolder = GetComponent<StatHolder>();
        _attackRangeStat = statHolder.GetStat("AttackRange");
        _chaseRangeStat = statHolder.GetStat("ChaseRange");
        _patrolRangeStat = statHolder.GetStat("PatrolRange");
        _moveSpeedStat = statHolder.GetStat("MoveSpeed");
        

        HomePosition = transform.position;

        if (_target == null)
            DetectPlayer();
        
        SwitchState(new IdleState());

        _moveSpeedStat.OnValueChanged += SetSpeed;
        SetSpeed(_moveSpeedStat);
    }

    private void OnDestroy()
    {
        if (_moveSpeedStat != null)
            _moveSpeedStat.OnValueChanged -= SetSpeed;
    }

    private void SetSpeed(StatBase stat)
    {
        Agent.speed = stat.GetValue();
    }

    private void Update()
    {
        if (_target == null || !Agent.isOnNavMesh || !GameManager.Instance.IsGameplay()) return;
        
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