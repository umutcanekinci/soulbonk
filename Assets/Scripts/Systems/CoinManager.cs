    using UnityEngine;
    using VectorViolet.Core.Utilities;

    public class CoinManager : Singleton<CoinManager>
    {
        public int currentCoins = 0;
        public static int CurrentCoins => Instance != null ? Instance.currentCoins : 0;
        public event System.Action<int> OnCoinAmountChange; 

        [Header("Pool Settings")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private int poolInitialSize = 20;
        
        private ObjectPool<Transform> _coinPool;

        protected override void Awake()
        {
            base.Awake();
            _coinPool = new ObjectPool<Transform>(coinPrefab.transform, poolInitialSize, transform);
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
                Transform coin = _coinPool.Get();
                coin.position = position + Random.insideUnitSphere * 0.5f;
            }
        }

        public void CollectCoin(GameObject coin)
        {
            _coinPool.Return(coin.transform);
            currentCoins += 1;
            OnCoinAmountChange?.Invoke(currentCoins);
        }

        public bool CanAfford(int cost)
        {
            return currentCoins >= cost;
        }

        public bool SpendCoins(int cost)
        {
            if (CanAfford(cost))
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