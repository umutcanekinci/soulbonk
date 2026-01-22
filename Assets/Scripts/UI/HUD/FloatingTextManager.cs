using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.6f;
    [SerializeField] private float scaleFactor = 0.02f;
    [SerializeField] private Vector2 randomOffsetRange = new Vector2(0.1f, 0.1f);

    private List<FloatingText> floatingTextPool = new List<FloatingText>();
    public static FloatingTextManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewFloatingText();
        }
    }    

    public void ShowFloatingText(string text, Vector3 position, Color color, float scale = 1f)
    {
        float finalScale = Mathf.Clamp(Mathf.Abs(scale) * scaleFactor, minScale, maxScale);
        position.x += Random.Range(-randomOffsetRange.x, randomOffsetRange.x);
        position.y += Random.Range(-randomOffsetRange.y, randomOffsetRange.y);
        
        FloatingText floatingText = GetPooledFloatingText();
        floatingText.Initialize(text, color, finalScale);
        floatingText.SetPosition(position);
    }

    private FloatingText GetPooledFloatingText()
    {
        foreach (var ft in floatingTextPool)
        {
            if (!ft.gameObject.activeInHierarchy)
            {
                return ft;
            }
        }

        return CreateNewFloatingText();
    }

    private FloatingText CreateNewFloatingText()
    {
        FloatingText newFloatingText = Instantiate(floatingTextPrefab, transform);
        floatingTextPool.Add(newFloatingText);
        return newFloatingText;
    }
}