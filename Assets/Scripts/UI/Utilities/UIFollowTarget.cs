using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; 
    public Vector3 offset = new Vector3(0, 1.5f, 0); 

    [Header("Performance")]
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        gameObject.SetActive(target != null);
    }
    
    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject); 
            return;
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z > 0) 
        {
            transform.position = screenPos;
        }
    }

    private void OnValidate()
    {
        _mainCamera = Camera.main;
        SetTarget(target);
        UpdatePosition();
    }
}