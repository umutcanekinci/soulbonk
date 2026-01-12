using UnityEngine;

public abstract class EnemyState {
    public abstract void Enter(EnemyAI enemy);
    public abstract void Update(EnemyAI enemy);
    public abstract void Exit(EnemyAI enemy);
}

public class IdleState : EnemyState
{
    private float _timer;

    public override void Enter(EnemyAI enemy)
    {
        enemy.Agent.ResetPath();
        _timer = enemy.PatrolWaitTime; // EnemyAI'dan ayarı al
    }

    public override void Update(EnemyAI enemy)
    {
        // 1. Oyuncu yakında mı?
        if (enemy.GetDistanceToTarget() < enemy.ChaseRange)
        {
            enemy.SwitchState(new ChaseState());
            return;
        }

        // 2. Bekleme süresi bitti mi?
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            enemy.SwitchState(new PatrolState());
        }
    }

    public override void Exit(EnemyAI enemy) { }
}

public class PatrolState : EnemyState
{
    public override void Enter(EnemyAI enemy)
    {
        enemy.GoToRandomPoint();
    }

    public override void Update(EnemyAI enemy)
    {
        // 1. Oyuncu yakında mı?
        if (enemy.GetDistanceToTarget() < enemy.ChaseRange)
        {
            enemy.SwitchState(new ChaseState());
            return;
        }

        // 2. Hedefe vardı mı?
        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance < 0.5f)
        {
            enemy.SwitchState(new IdleState());
        }
    }

    public override void Exit(EnemyAI enemy) { }
}

public class ChaseState : EnemyState
{
    public override void Enter(EnemyAI enemy) { }

    public override void Update(EnemyAI enemy)
    {
        float distance = enemy.GetDistanceToTarget();

        if (distance <= enemy.AttackRange)
        {
            enemy.SwitchState(new AttackState());
            return;
        }
        
        if (distance > enemy.ChaseRange * 1.5f)
        {
            enemy.SwitchState(new ReturnState());
            return;
        }

        if(enemy._target != null)
            enemy.Agent.SetDestination(enemy._target.position);
    }

    public override void Exit(EnemyAI enemy) 
    {
        enemy.Agent.ResetPath();
    }
}

public class AttackState : EnemyState
{
    public override void Enter(EnemyAI enemy)
    {
        enemy.Agent.ResetPath();
        enemy.EntityMovement.SetMoveInput(Vector2.zero);
    }

    public override void Update(EnemyAI enemy)
    {
        float distance = enemy.GetDistanceToTarget();

        if (distance > enemy.AttackRange)
        {
            enemy.SwitchState(new ChaseState());
            return;
        }

        if (enemy.EntityAttack != null)
        {
            enemy.EntityAttack.AttackLogic();
        }
    }

    public override void Exit(EnemyAI enemy) { }
}