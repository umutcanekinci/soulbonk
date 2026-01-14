using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private Vector2 centerOffset = Vector2.zero; 

    public Vector2 GetClosestInteractionPoint(Vector2 fromPosition)
    {
        List<Vector2> points = GetInteractionPoints();
        
        Vector2 closestPoint = points[0];
        float closestDistance = Vector2.Distance(fromPosition, closestPoint);

        foreach (var point in points)
        {
            float distance = Vector2.Distance(fromPosition, point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    public void Interact()
    {
        if (GameManager.IsGameplay)
        {
            EventBus.PlayerInteraction.TriggerInteraction(this);
        }
    }

    public void Deinteract()
    {
        if (GameManager.IsInteraction)
        {
            EventBus.PlayerInteraction.TriggerDeinteraction();   
        }
    }

    public void ShowInteractionUI() => interactionUI.SetActive(true);
    public void HideInteractionUI() => interactionUI.SetActive(false);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        Vector2 center = (Vector2)transform.position + centerOffset;
        Gizmos.DrawWireSphere(center, 0.1f); 

        foreach (var point in GetInteractionPoints())
        {
            Gizmos.DrawSphere(point, 0.02f);
        }
    }

    private List<Vector2> GetInteractionPoints()
    {
        Vector2 center = (Vector2)transform.position + centerOffset;
        return new List<Vector2>
        {
            new Vector2(center.x + interactionDistance, center.y),
            new Vector2(center.x - interactionDistance, center.y), 
            new Vector2(center.x, center.y + interactionDistance), 
            new Vector2(center.x, center.y - interactionDistance)
        };
    }
}