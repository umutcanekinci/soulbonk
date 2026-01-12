using UnityEngine;

public class OneShotEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    void Start()
    {
        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animationDuration); 
    }
}