using UnityEngine;
using System;

public class EventBus : MonoBehaviour
{
    public static event Action<Vector3, int> OnEnemyDeath;
    public static event Action<Interactable> OnPlayerInteraction;
    public static event Action OnPlayerDeinteraction;

    public static void TriggerEnemyDeath(Vector3 position, int coinAmount) => OnEnemyDeath?.Invoke(position, coinAmount);
    public static void TriggerPlayerInteraction(Interactable interactable) => OnPlayerInteraction?.Invoke(interactable);
    public static void TriggerPlayerDeinteraction() => OnPlayerDeinteraction?.Invoke();
}