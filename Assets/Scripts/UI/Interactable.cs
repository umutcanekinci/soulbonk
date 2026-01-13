using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private float interactionDistance = 1.5f;
    
    // HOCAM BURASI ÖNEMLİ: Ateşin görsel merkezini ayarlamak için offset ekledik
    [SerializeField] private Vector2 centerOffset = Vector2.zero; 

    // Listeyi sildik, noktaları anlık hesaplayacağız.
    // Böylece obje hareket etse bile noktalar hep doğru yerde olur.
    
    // Bu özellik (Property) bize her çağrıldığında o anki doğru noktaları verir
    private List<Vector2> GetInteractionPoints()
    {
        Vector2 center = (Vector2)transform.position + centerOffset;
        return new List<Vector2>
        {
            new Vector2(center.x + interactionDistance, center.y), // Sağ
            new Vector2(center.x - interactionDistance, center.y), // Sol
            new Vector2(center.x, center.y + interactionDistance), // Yukarı
            new Vector2(center.x, center.y - interactionDistance)  // Aşağı
        };
    }

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
        // EventBus yapını korudum, parametre olarak 'this' gönderdiğini varsayıyorum
        EventBus.TriggerPlayerInteraction(this);
    }

    public void Deinteract()
    {
        EventBus.TriggerPlayerDeinteraction();
    }

    public void ShowInteractionUI() => interactionUI.SetActive(true);
    public void HideInteractionUI() => interactionUI.SetActive(false);

    // Gizmos artık dinamik hesaplandığı için Editörde objeyi kaydırsan da,
    // offset ile oynasan da anlık olarak doğru yeri gösterecek.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        // Merkezi görelim (Referans için)
        Vector2 center = (Vector2)transform.position + centerOffset;
        Gizmos.DrawWireSphere(center, 0.1f); 

        // Noktaları çizelim
        foreach (var point in GetInteractionPoints())
        {
            Gizmos.DrawSphere(point, 0.02f);
        }
    }
}