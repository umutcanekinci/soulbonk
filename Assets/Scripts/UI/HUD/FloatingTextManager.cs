using System.Collections.Generic;
using UnityEngine;
using VectorViolet.Core.Utilities;

public class FloatingTextManager : Singleton<FloatingTextManager>
{
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.6f;
    [SerializeField] private float scaleFactor = 0.02f;
    [SerializeField] private Vector2 randomOffsetRange = new Vector2(0.1f, 0.1f);

    private ObjectPool<FloatingText> floatingTextPool;

    protected override void Awake()
    {
        base.Awake();
        floatingTextPool = new ObjectPool<FloatingText>(floatingTextPrefab, initialPoolSize, transform);
    }    

    public void ShowFloatingText(string text, Vector3 position, Color color, float scale = 1f)
    {
        float finalScale = Mathf.Clamp(Mathf.Abs(scale) * scaleFactor, minScale, maxScale);
        position.x += Random.Range(-randomOffsetRange.x, randomOffsetRange.x);
        position.y += Random.Range(-randomOffsetRange.y, randomOffsetRange.y);
        
        FloatingText floatingText = floatingTextPool.Get();
        floatingText.Initialize(text, color, finalScale, floatingTextPool.Return);
        floatingText.SetPosition(position);
    }

}