using UnityEngine;
using VectorViolet.Core.Stats;
using System.Collections;

[RequireComponent(typeof(StatHolder))]
[RequireStat("AttackSpeed", "AttackRange", "AttackCount")]
public class CircleWeapon : MonoBehaviour
{
    [SerializeField] private GameObject circlePrefab;
    private StatHolder statHolder;
    StatBase attackSpeedStat, attackRangeStat, attackCountStat;

    private Coroutine updateCoroutine;

    private void Start()
    {
        statHolder = GetComponent<StatHolder>();
        attackSpeedStat = statHolder.GetStat("AttackSpeed");
        attackRangeStat = statHolder.GetStat("AttackRange");
        attackCountStat = statHolder.GetStat("AttackCount");

        attackSpeedStat.OnValueChanged += OnAttackCountChanged;
        attackRangeStat.OnValueChanged += OnAttackCountChanged;
        attackCountStat.OnValueChanged += OnAttackCountChanged;
        
        UpdateCirclesImmediate();
    }

    private void OnAttackCountChanged(StatBase stat = null)
    {
        if (Application.isPlaying && gameObject.activeInHierarchy)
        {
            if (updateCoroutine != null) StopCoroutine(updateCoroutine);
            updateCoroutine = StartCoroutine(UpdateCirclesRoutine());
        }
    }

    private IEnumerator UpdateCirclesRoutine()
    {
        yield return null; 
        
        UpdateCirclesImmediate();
    }

    private void UpdateCirclesImmediate()
    {
        DestroyChildCircles();
        CreateCircles(attackCountStat.GetValue(), attackSpeedStat.GetValue(), attackRangeStat.GetValue());
    }

    private void DestroyChildCircles()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateCircles(float count, float speed = 1f, float radius = 3f)
    {
        int intCount = Mathf.RoundToInt(count);
        if (intCount <= 0)
            return; 

        float angleStep = 360f / intCount;

        for (int i = 0; i < intCount; i++)
        {
            float angle = i * angleStep;
            GameObject child = Instantiate(circlePrefab, transform.position, Quaternion.identity, transform);
            
            CircleRotator rotator = child.GetComponent<CircleRotator>();
            if (rotator == null)
            {
                rotator = child.gameObject.AddComponent<CircleRotator>();
            }
            rotator.Initialize(radius, speed, angle);
        }
    }
}