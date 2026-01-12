using UnityEngine;

[System.Serializable]
public class EnemyRate
{
    public GameObject enemyPrefab;
    public float spawnRate;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    
    [Header("Settings")]
    [SerializeField] private EnemyRate[] enemyRates;
    public float spawnInterval = 2f;
    public int maxEnemies = 50;
    
    [Header("Spawn Locations")]
    public Transform[] spawnPoints;

    private float currentTimer;

    void Update()
    {
        currentTimer -= Time.deltaTime;
        if (currentTimer <= 0)
        {
            SpawnEnemy();
            currentTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyRates == null || enemyRates.Length == 0)
        {
            Debug.LogWarning("Spawn points or enemy rates not set up correctly.");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform targetSpawnPoint = spawnPoints[randomIndex];
        GameObject enemyPrefab = enemyRates[Random.Range(0, enemyRates.Length)].enemyPrefab;
        SpawnEnemy(enemyPrefab, targetSpawnPoint.position);
    }

    public void SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        int currentEnemyCount = transform.childCount;
        if (currentEnemyCount >= maxEnemies)
        {
            return;
        }

        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.GetComponent<EnemyAI>().SetTarget(player);
        newEnemy.transform.parent = transform;
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