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
        public static event Func<Vector3, float, IEnumerator> OnMove;

        public static IEnumerator TriggerZoomIn(float amount, float duration) => OnZoomIn?.Invoke(amount, duration);
        public static IEnumerator TriggerResetZoom(float duration) => OnResetZoom?.Invoke(duration);
        public static IEnumerator TriggerMove(Vector3 targetPosition, float duration) => OnMove?.Invoke(targetPosition, duration);
        public static void TriggerShake(float duration, float magnitude) => OnShake?.Invoke(duration, magnitude);
    }

    public static class PlayerInteraction
    {
        public static event Action<Interactable, GameObject> OnInteractionRequest;
        public static event Action<Interactable, GameObject> OnDeinteractionRequest;

        public static void TriggerRequest(Interactable interactable, GameObject player) => OnInteractionRequest?.Invoke(interactable, player);
        public static void TriggerDeinteraction(Interactable interactable, GameObject player) => OnDeinteractionRequest?.Invoke(interactable, player);
    }

    public static class Enemy
    {
        public static event Action<Vector3, int> OnDeath;

        public static void TriggerDeath(Vector3 position, int coinAmount) => OnDeath?.Invoke(position, coinAmount);

    }
}