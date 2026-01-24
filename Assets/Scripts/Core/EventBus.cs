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
        public static event Func<Vector3, Vector2, float, float, IEnumerator> OnFocus;
        public static event Func<float, IEnumerator> OnDefocus;
        public static event Func<float, IEnumerator> OnResetPosition;

        public static void TriggerShake(float duration, float magnitude) => OnShake?.Invoke(duration, magnitude);
        public static IEnumerator TriggerZoomIn(float amount, float duration) => OnZoomIn?.Invoke(amount, duration);
        public static IEnumerator TriggerResetZoom(float duration) => OnResetZoom?.Invoke(duration);
        public static IEnumerator TriggerMove(Vector3 targetPosition, float duration) => OnMove?.Invoke(targetPosition, duration);
        public static IEnumerator TriggerFocus(Vector3 targetPosition, Vector2 cameraOffset, float zoomAmount, float duration) => OnFocus?.Invoke(targetPosition, cameraOffset, zoomAmount, duration);
        public static IEnumerator TriggerDefocus(float duration) => OnDefocus?.Invoke(duration);
        public static IEnumerator TriggerResetPosition(float duration) => OnResetPosition?.Invoke(duration);
    }

    public static class PlayerInteraction
    {
        public static event Action<Interactable, GameObject> OnInteractionRequestWith;
        public static event Action<Interactable, GameObject> OnDeinteractionRequestWith;
        public static event Action OnDeinteractionRequest;

        public static void TriggerInteractionWith(Interactable interactable, GameObject player) => OnInteractionRequestWith?.Invoke(interactable, player);
        public static void TriggerDeinteractionWith(Interactable interactable, GameObject player) => OnDeinteractionRequestWith?.Invoke(interactable, player);
        public static void RequestDeinteraction() => OnDeinteractionRequest?.Invoke();
    }

    public static class Enemy
    {
        public static event Action<Vector3, int> OnDeath;

        public static void TriggerDeath(Vector3 position, int coinAmount) => OnDeath?.Invoke(position, coinAmount);
    }

}