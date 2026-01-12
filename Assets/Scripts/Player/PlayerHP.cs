using UnityEngine;

[RequireComponent(typeof(EntityHP))]
public class PlayerHP : MonoBehaviour
{
    private EntityHP entityHP;

    private void Awake()
    {
        entityHP = GetComponent<EntityHP>();
        if (entityHP != null)
        {
            entityHP.OnDeath += OnDeath;
        }
    }

    private void OnDestroy()
    {
        if (entityHP != null)
        {
            entityHP.OnDeath -= OnDeath;    
        }
    }
    
    private void OnDeath()
    {
        Destroy(gameObject);
    }
}