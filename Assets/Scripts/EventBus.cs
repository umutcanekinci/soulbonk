using UnityEngine;
using System;
using System.Collections;

public class EventBus : MonoBehaviour
{
    public static event Action<Vector3, int> OnEnemyDeath;
    public static event Action<Interactable> OnPlayerInteraction;
    public static event Action OnPlayerDeinteraction;
    public static event Action<float, float> OnShake;
    public static event Func<float, float, IEnumerator> OnZoomIn;
    public static event Func<float, IEnumerator> OnResetZoom;
    
    public static IEnumerator TriggerZoomIn(float amount, float duration) => OnZoomIn?.Invoke(amount, duration);
    public static IEnumerator TriggerResetZoom(float duration) => OnResetZoom?.Invoke(duration);
    public static void TriggerShake(float duration, float magnitude) => OnShake?.Invoke(duration, magnitude);
    public static void TriggerEnemyDeath(Vector3 position, int coinAmount) => OnEnemyDeath?.Invoke(position, coinAmount);
    public static void TriggerPlayerInteraction(Interactable interactable) => OnPlayerInteraction?.Invoke(interactable);
    public static void TriggerPlayerDeinteraction() => OnPlayerDeinteraction?.Invoke();
}