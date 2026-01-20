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
        _timer = enemy.PatrolWaitTime;
    }

    public override void Update(EnemyAI enemy)
    {
        if (enemy.GetDistanceToTarget() < enemy.ChaseRange)
        {
            enemy.SwitchState(new ChaseState());
            return;
        }

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
        enemy.EntityMovement.Stop();
    }

    public override void Update(EnemyAI enemy)
    {
        float distance = enemy.GetDistanceToTarget();

        if (distance > enemy.AttackRange)
        {
            enemy.SwitchState(new ChaseState());
            return;
        }
        if (enemy.WeaponController != null)
        {
            enemy.WeaponController.ActiveAttack();
        }
    }

    public override void Exit(EnemyAI enemy) { }
}

public class ReturnState : EnemyState
{
    public override void Enter(EnemyAI enemy)
    {
        enemy.GoHome();
    }

    public override void Update(EnemyAI enemy)
    {
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
