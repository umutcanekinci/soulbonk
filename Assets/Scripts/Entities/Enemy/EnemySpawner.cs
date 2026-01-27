using System.Collections.Generic;
using UnityEngine;
using VectorViolet.Core.Utilities;

[System.Serializable]
public class EnemyType
{
    public EnemyAI prefab;
    public float spawnRate;
}

public class EnemySpawner : Singleton<EnemySpawner>
{
    public int CurrentEnemyCount => EnemyContainer.childCount;

    [Header("References")]
    [SerializeField] private Transform player;
    
    [Header("Settings")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private EnemyType[] enemyTypes;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies = 50;
    
    [Header("Spawn Locations")]
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private Transform _enemyContainer;
    private Dictionary<EnemyType, ObjectPool<EnemyAI>> enemyPools = new Dictionary<EnemyType, ObjectPool<EnemyAI>>();

    private Transform EnemyContainer
    {
        get
        {
            if (_enemyContainer == null)
            {
                GameObject containerObj = GameObject.Find("Entities");

                if (containerObj == null)
                {
                    containerObj = new GameObject("Entities");
                }
                _enemyContainer = containerObj.transform;
            }
            return _enemyContainer;
        }
    }

    private float currentTimer;

    private void Start()
    {
        foreach (Transform enemy in EnemyContainer)
        {
            enemy.GetComponent<EnemyAI>()?.SetTarget(player);
        }

        foreach (EnemyType type in enemyTypes)
        {
            enemyPools[type] = new ObjectPool<EnemyAI>(type.prefab, 100, EnemyContainer);
        }
    }

    void Update()
    {
        if (!spawnOnStart || !GameManager.IsGameplay)
            return;

        currentTimer -= Time.deltaTime;
        if (currentTimer <= 0)
        {
            SpawnEnemy();
            currentTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyTypes == null || enemyTypes.Length == 0)
        {
            Debug.LogWarning("Spawn points or enemy rates not set up correctly.");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform targetSpawnPoint = spawnPoints[randomIndex];
        EnemyAI enemyPrefab = enemyTypes[Random.Range(0, enemyTypes.Length)].prefab;
        SpawnEnemy(enemyPrefab, targetSpawnPoint.position);
    }

    public void SpawnEnemy(EnemyAI enemyPrefab, Vector3 position)
    {
        if (enemyPrefab == null)
            return;

        if (CurrentEnemyCount >= maxEnemies)
            return;

        EnemyAI newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity, EnemyContainer);
        newEnemy.SetTarget(player);
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        if (spawnPoints != null)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    // Write spawn point index above the point
                    int index = System.Array.IndexOf(spawnPoints, point);
                    string label = "Spawn Point " + index;

                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(point.position + Vector3.up * 0.2f - Vector3.right * 0.2f, label);
                    #endif

                    Gizmos.DrawWireSphere(point.position, 0.1f);
                }
            }
        }
    }

}