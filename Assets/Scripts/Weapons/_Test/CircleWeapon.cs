using UnityEngine;
using VectorViolet.Core.Stats;
using System.Collections; // Coroutine için gerekli

[RequireComponent(typeof(StatHolder))]
[RequireStat("AttackSpeed", "AttackRange", "AttackCount")]
public class CircleWeapon : MonoBehaviour
{
    [SerializeField] private GameObject circlePrefab;
    private StatHolder statHolder;
    StatBase attackSpeedStat, attackRangeStat, attackCountStat;

    // Coroutine'i takip etmek için bir referans (üst üste binmemesi için)
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
        
        // İlk açılışta direkt çalışabilir, Start güvenlidir.
        UpdateCirclesImmediate();
    }

    private void OnAttackCountChanged(StatBase stat = null)
    {
        // Eğer oyun çalışıyorsa ve obje aktifse Coroutine başlat
        if (Application.isPlaying && gameObject.activeInHierarchy)
        {
            if (updateCoroutine != null) StopCoroutine(updateCoroutine);
            updateCoroutine = StartCoroutine(UpdateCirclesRoutine());
        }
    }

    // Bu fonksiyon bir frame bekleyip sonra işi yapar
    private IEnumerator UpdateCirclesRoutine()
    {
        // Bir frame bekle (Böylece OnValidate biter)
        yield return null; 
        
        UpdateCirclesImmediate();
    }

    // Asıl işi yapan fonksiyonu ayırdık
    private void UpdateCirclesImmediate()
    {
        DestroyChildCircles();
        CreateCircles(attackCountStat.GetValue(), attackSpeedStat.GetValue(), attackRangeStat.GetValue());
    }

    private void DestroyChildCircles()
    {
        // Not: Transform enumerator kullanırken içinde Destroy yapmak bazen sorun olabilir.
        // Ters döngü veya listeye alıp silmek daha güvenlidir ama bu da çalışır.
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Instead of creating circles we can update them if they already exist. But for simplicity, we recreate them here.
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