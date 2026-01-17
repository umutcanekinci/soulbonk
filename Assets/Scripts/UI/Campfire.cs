using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Campfire : Interactable
{
    [SerializeField] private float interactionDistance = 1.5f;
    
    public override IEnumerator OnInteractSequence(GameObject player)
    {
        EntityMovement playerMovement = player.GetComponent<EntityMovement>();
        if (playerMovement != null)
        {
            Vector2 targetPosition = GetInteractionPoint(player.transform.position);
            Coroutine movementRoutine   = StartCoroutine(playerMovement.MoveToPositionCoroutine(targetPosition));
            yield return movementRoutine; // Wait for player to reach the interaction point
            playerMovement.Sit();
        }

        EntityHP playerHP = player.GetComponent<EntityHP>();
        if (playerHP != null)
        {
            playerHP.HealToFull();
        }
    }

    protected override Vector2 GetInteractionPoint(Vector2 fromPosition)
    {
        Vector2 center = base.GetInteractionPoint(fromPosition);
        float minDistance = Mathf.Infinity;
        Vector2 closestPoint = center;
        foreach (var point in GetInteractionPoints(center))
        {
            float distance = Vector2.Distance(fromPosition, point);
            if (distance <= minDistance)
            {
                minDistance = distance;
                closestPoint = point;
            }
        }
        return closestPoint;
    }

    private IEnumerable<Vector2> GetInteractionPoints(Vector2 center)
    {
        yield return new Vector2(center.x + interactionDistance, center.y);
        yield return new Vector2(center.x - interactionDistance, center.y);
        yield return new Vector2(center.x, center.y + interactionDistance);
        yield return new Vector2(center.x, center.y - interactionDistance);
    }

    public override IEnumerator OnDeinteractSequence(GameObject player)
    {
        EntityMovement playerMovement = player.GetComponent<EntityMovement>();
        if (playerMovement != null)
        {
            playerMovement.StandUp();
        }
        yield return null;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        Vector2 center = base.GetInteractionPoint(Vector2.zero);
        Gizmos.DrawWireSphere(center, 0.1f); 

        foreach (var point in GetInteractionPoints(center))
        {
            Gizmos.DrawSphere(point, 0.02f);
        }
    }


}