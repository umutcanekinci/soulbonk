using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    public static CoinManager Instance { get; private set; }
    private Queue<GameObject> coinPool = new Queue<GameObject>();
    public event System.Action<int> OnCoinCollected; 
    public int coinAmount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventBus.Enemy.OnDeath += SpawnCoins;
    }

    private void OnDisable()
    {
        EventBus.Enemy.OnDeath -= SpawnCoins;
    }

    private void SpawnCoins(Vector3 position, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject coin = GetCoinFromPool();
            coin.transform.position = position + Random.insideUnitSphere * 0.5f;
            coin.SetActive(true);
        }
    }

    private GameObject GetCoinFromPool()
    {
        if (coinPool.Count > 0)
        {
            return coinPool.Dequeue();
        }
        else
        {
            return Instantiate(coinPrefab, transform);
        }
    }
    
    public void ReturnCoinToPool(GameObject coin)
    {
        coin.SetActive(false);
        coinPool.Enqueue(coin);
        coinAmount += 1;
        OnCoinCollected?.Invoke(coinAmount);
    }
}