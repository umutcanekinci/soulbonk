using UnityEngine;
class NecromancerStaff : WeaponBase
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private float offset = 1.0f;

    public override void Attack(Vector3 direction)
    {
        enemySpawner.SpawnEnemy(spawnObject, transform.position + direction * offset);
    }
}