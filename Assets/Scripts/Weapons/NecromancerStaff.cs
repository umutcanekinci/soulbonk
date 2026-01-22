using UnityEngine;

class NecromancerStaff : WeaponBase
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject spawnObject;

    public override void Attack(Vector3 direction)
    {
        if (enemySpawner == null || spawnObject == null)
            return;

        Vector3 position = transform.position + AttackRange * direction.normalized;
        enemySpawner.SpawnEnemy(spawnObject, position);
    }
}