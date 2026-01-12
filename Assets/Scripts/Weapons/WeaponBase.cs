using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public abstract void Attack(Vector3 direction);
}