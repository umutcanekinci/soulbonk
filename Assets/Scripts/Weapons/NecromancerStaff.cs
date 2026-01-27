using UnityEngine;

class NecromancerStaff : WeaponBase
{
    [SerializeField] private EnemyAI spawnObject;

    public override void Attack(Vector3 direction)
    {
        if (EnemySpawner.Instance == null || spawnObject == null)
            return;

        Vector3 position = transform.position + AttackRange * direction.normalized;
        EnemySpawner.Instance.SpawnEnemy(spawnObject, position);
    }
}