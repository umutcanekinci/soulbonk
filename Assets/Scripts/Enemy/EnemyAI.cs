using UnityEngine;
using UnityEngine.AI;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(NavMeshAgent), typeof(StatHolder), typeof(EntityMovement))]
[RequireStat("ChaseRange", "PatrolRange", "MoveSpeed")]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EntityAttack entityAttack;
    private EntityMovement entityMovement;
    
    [Header("Settings")]
    public float PatrolWaitTime = 2f;

    private EnemyState _currentState;
    public NavMeshAgent Agent { get; private set; }
    public Transform _target { get; private set; }
    public Vector3 HomePosition { get; private set; }

    // Range Stats
    private StatBase _attackRangeStat, _chaseRangeStat, _patrolRangeStat, _moveSpeedStat;
    
    public EntityAttack EntityAttack => entityAttack;
    public EntityMovement EntityMovement => entityMovement;
    public float AttackRange => _attackRangeStat != null ? _attackRangeStat.GetValue() : 0f;
    public float ChaseRange => _chaseRangeStat.GetValue();
    public float PatrolRange => _patrolRangeStat.GetValue();

    private void Awake()
    {
        entityMovement = GetComponent<EntityMovement>();
        entityMovement.usePhysicsMovement = false;
        
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        Debug.Log(GameManager.Instance != null ? "GameManager instance found." : "GameManager instance NOT found.");
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

        if (_moveSpeedStat != null)
        {
            _moveSpeedStat.OnValueChanged -= SetSpeed;
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        Agent.isStopped = newState == GameState.Cutscene;

        if (newState != GameState.Gameplay)
        {
            Debug.Log("GameState changed to non-Gameplay. Stopping Enemy AI.");
            _currentState = new IdleState();
            entityMovement.Stop();
        }
    }

    private void Start()
    {
        var statHolder = GetComponent<StatHolder>();
        _attackRangeStat = GetAttackRangeStat();
        _chaseRangeStat = statHolder.GetStat("ChaseRange");
        _patrolRangeStat = statHolder.GetStat("PatrolRange");
        _moveSpeedStat = statHolder.GetStat("MoveSpeed");
        
        HomePosition = transform.position;
        SwitchState(new IdleState());
        _moveSpeedStat.OnValueChanged += SetSpeed;
        SetSpeed(_moveSpeedStat);
    }

    private StatBase GetAttackRangeStat()
    {
        if (entityAttack != null && entityAttack.CurrentWeapon != null)
        {
            return entityAttack.CurrentWeapon.GetAttackRangeStat();
        }
        return null;
    }

    private void SetSpeed(StatBase stat)
    {
        Agent.speed = stat.GetValue();
    }

    private void Update()
    {
        if (_target == null || !Agent.isOnNavMesh || !GameManager.Instance.IsGameplay()) return;
        
        if (entityAttack != null && entityAttack.IsAttacking)
            return;
            
        _currentState?.Update(this);
        entityMovement.SetMoveInput(Agent.velocity);
    }

    public void SwitchState(EnemyState newState)
    {
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState?.Enter(this);
    }

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

    public void SetTarget(Transform target)
    {
        _target = target;
        if (entityAttack != null)
            entityAttack.target = target;
    }
}