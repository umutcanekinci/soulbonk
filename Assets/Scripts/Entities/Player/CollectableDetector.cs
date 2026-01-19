using UnityEngine;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(Collider2D))]
public class CollectableDetector : MonoBehaviour
{
    [SerializeField] private StatHolder statHolder;
    private CircleCollider2D col;
    private StatBase rangeStat;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        if (statHolder != null)
            rangeStat = statHolder.GetStat("CollectRange");
    }

    private void OnEnable() {
        if (rangeStat != null)
        {
            rangeStat.OnValueChanged += UpdateRadius;
            UpdateRadius(rangeStat);
        }
            
    }

    private void OnDisable() {
        if (rangeStat != null)
            rangeStat.OnValueChanged -= UpdateRadius;
    }

    private void UpdateRadius(StatBase stat)
    {
        col.radius = stat.GetValue();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Coin"))
            return;

        CoinManager.Instance.ReturnCoinToPool(collision.gameObject);    
    }
}