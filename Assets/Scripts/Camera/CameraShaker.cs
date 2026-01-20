using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    private Vector3 originalPosition;

    void OnEnable()
    {
        EventBus.Camera.OnShake += Shake;
    }

    void OnDisable()
    {
        EventBus.Camera.OnShake -= Shake;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        originalPosition = transform.localPosition;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
