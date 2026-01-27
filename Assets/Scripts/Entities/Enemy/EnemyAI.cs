using UnityEngine;
using UnityEngine.AI;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(NavMeshAgent), typeof(StatHolder), typeof(EntityMovement))]
[RequireStat("ChaseRange", "PatrolRange", "MoveSpeed")]
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public float PatrolWaitTime = 2f;

    private WeaponController weaponController;
    private EntityMovement entityMovement;
    private EnemyState _currentState;
    public NavMeshAgent Agent { get; private set; }
    public Transform _target { get; private set; }
    public Vector3 HomePosition { get; private set; }

    // Range Stats
    private StatBase _attackRangeStat, _chaseRangeStat, _patrolRangeStat, _moveSpeedStat;
    
    public WeaponController WeaponController => weaponController;
    public EntityMovement EntityMovement => entityMovement;
    public float AttackRange => _attackRangeStat?.Value ?? 0f;
    public float ChaseRange => _chaseRangeStat.Value;
    public float PatrolRange => _patrolRangeStat.Value;

    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
        entityMovement = GetComponent<EntityMovement>();
        entityMovement.usePhysicsMovement = false;
        
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleGameStateChanged;
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;

        if (_moveSpeedStat != null)
        {
            _moveSpeedStat.OnValueChanged -= SetSpeed;
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        bool canMove = newState == GameState.Gameplay;

        if (Agent != null && Agent.isOnNavMesh)
            Agent.isStopped = !canMove;

        if (!canMove)
        {
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
        if (weaponController != null && weaponController.CurrentWeapon != null)
        {
            return weaponController.CurrentWeapon.GetAttackRangeStat();
        }
        return null;
    }

    private void SetSpeed(StatBase stat)
    {
        Agent.speed = stat.Value;
    }

    private void Update()
    {
        if (_target == null || !Agent.isOnNavMesh || !GameManager.IsGameplay)
            return;
        
        if (weaponController != null && weaponController.IsAttacking)
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
        if (weaponController != null)
            weaponController.target = target;
    }

    public void ResetPath()
    {
        if (Agent.isOnNavMesh)
            Agent.ResetPath();
    }
}