using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject coinPrefab;
    
    private Queue<GameObject> coinPool = new Queue<GameObject>();
    public event System.Action<int> OnCoinAmountChange; 
    public int coinAmount = 0;
    public static CoinManager Instance { get; private set; }

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
        return (coinPool.Count > 0) ? coinPool.Dequeue() : Instantiate(coinPrefab, transform);
    }    
    
    public void ReturnCoinToPool(GameObject coin)
    {
        coin.SetActive(false);
        coinPool.Enqueue(coin);
        coinAmount += 1;
        OnCoinAmountChange?.Invoke(coinAmount);
    }

    public bool IsEnoughCoins(int cost)
    {
        return coinAmount >= cost;
    }

    public bool SpendCoins(int cost)
    {
        if (IsEnoughCoins(cost))
        {
            coinAmount -= cost;
            OnCoinAmountChange?.Invoke(coinAmount);
            return true;
        }
        return false;
    }
}