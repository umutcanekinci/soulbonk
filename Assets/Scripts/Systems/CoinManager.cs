using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject coinPrefab;
    
    private Queue<GameObject> coinPool = new Queue<GameObject>();
    public event System.Action<int> OnCoinAmountChange; 
    public int currentCoins = 0;
    public static int CurrentCoins => Instance != null ? Instance.currentCoins : 0;
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
        currentCoins += 1;
        OnCoinAmountChange?.Invoke(currentCoins);
    }

    public bool IsEnoughCoins(int cost)
    {
        return currentCoins >= cost;
    }

    public bool SpendCoins(int cost)
    {
        if (IsEnoughCoins(cost))
        {
            currentCoins -= cost;
            OnCoinAmountChange?.Invoke(currentCoins);
            return true;
        }
        return false;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            OnCoinAmountChange?.Invoke(currentCoins);
        }
    }
    #endif
}