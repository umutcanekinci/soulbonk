using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    private Vector3 defaultPosition;

    private void OnEnable()
    {
        EventBus.Camera.OnMove += MoveCameraToPosition;
    }

    private void OnDisable()
    {
        EventBus.Camera.OnMove -= MoveCameraToPosition;
    }

    private IEnumerator MoveCameraToPosition(Vector3 targetPosition, float duration)
    {
        defaultPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(defaultPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}