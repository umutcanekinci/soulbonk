using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeOutSpeed = 1f;

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
        FloatingText floatingText = GetPooledFloatingText();
        floatingText.SetText(text);
        floatingText.SetColor(color);
        floatingText.SetPosition(position);
        floatingText.SetScale(scale);
        floatingText.gameObject.SetActive(true);
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
        newFloatingText.SetMoveSpeed(moveSpeed);
        newFloatingText.SetFadeOutSpeed(fadeOutSpeed);
        floatingTextPool.Add(newFloatingText);
        return newFloatingText;
    }
}