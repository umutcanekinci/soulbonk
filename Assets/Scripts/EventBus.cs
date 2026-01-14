using UnityEngine;
using System;
using System.Collections;

public static class EventBus
{
    public static class Camera
    {
        public static event Action<float, float> OnShake;
        public static event Func<float, float, IEnumerator> OnZoomIn;
        public static event Func<float, IEnumerator> OnResetZoom;
        public static event Func<Vector3, float, IEnumerator> OnCameraMove;

        public static IEnumerator TriggerZoomIn(float amount, float duration) => OnZoomIn?.Invoke(amount, duration);
        public static IEnumerator TriggerResetZoom(float duration) => OnResetZoom?.Invoke(duration);
        public static IEnumerator TriggerCameraMove(Vector3 targetPosition, float duration) => OnCameraMove?.Invoke(targetPosition, duration);
        public static void TriggerShake(float duration, float magnitude) => OnShake?.Invoke(duration, magnitude);
    }

    public static class PlayerInteraction
    {
        public static event Action<Interactable> OnInteraction;
        public static event Action OnDeinteraction;

        public static void TriggerInteraction(Interactable interactable) => OnInteraction?.Invoke(interactable);
        public static void TriggerDeinteraction() => OnDeinteraction?.Invoke();
    }

    public static event Action<Vector3, int> OnEnemyDeath;

    public static void TriggerEnemyDeath(Vector3 position, int coinAmount) => OnEnemyDeath?.Invoke(position, coinAmount);

}