using UnityEngine;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(StatHolder))]
[RequireStat("AttackDamage", "AttackRange")]
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    protected StatBase attackDamageStat;
    protected StatBase attackRangeStat;

    protected virtual void Start()
    {
        StatHolder statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            attackDamageStat = statHolder.GetStat("AttackDamage");
            attackRangeStat = statHolder.GetStat("AttackRange");
        }
    }

    public StatBase GetAttackRangeStat()
    {
        return attackRangeStat;
    }

    public abstract void Attack(Vector3 direction);
}