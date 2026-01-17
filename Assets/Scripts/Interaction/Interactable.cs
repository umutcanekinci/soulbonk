using UnityEngine;
using TMPro;
using System.Collections;

public abstract class Interactable : MonoBehaviour
{
    [Header("UI Settings")]
    public string interactionText = "Interact";
    public string deinteractionText = "Deinteract";
    public Vector3 uiOffset = new Vector3(0, 1f, 0);

    [SerializeField] private Vector2 localInteractionPoint = Vector2.zero;
    
    public virtual Vector2 GetInteractionCenter()
    {
        return (Vector2)transform.position + localInteractionPoint;
    }

    public abstract Vector2 GetMoveTargetPoint(Vector2 fromPosition);

    public Vector3 GetUIPosition()
    {
        return transform.position + uiOffset;
    }

    public abstract IEnumerator OnInteractSequence(GameObject player);
    public abstract IEnumerator OnDeinteractSequence(GameObject player);

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 center = GetInteractionCenter();
        Gizmos.DrawSphere(center, 0.04f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetUIPosition(), 0.04f);
    }
}