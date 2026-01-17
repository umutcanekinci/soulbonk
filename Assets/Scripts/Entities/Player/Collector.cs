using UnityEngine;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(Collider2D))]
public class Collector : MonoBehaviour
{
    [SerializeField] private StatHolder statHolder;
    private CircleCollider2D col;
    private StatBase rangeStat;
    
    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        if (statHolder != null)
        {
            rangeStat = statHolder.GetStat("CollectRange");
            if (rangeStat != null)
            {
                rangeStat.OnValueChanged += UpdateRadius;
                UpdateRadius(rangeStat);
            }
                
        }
    }

    private void OnDestroy()
    {
        if (rangeStat != null)
        {
            rangeStat.OnValueChanged -= UpdateRadius;
        }
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