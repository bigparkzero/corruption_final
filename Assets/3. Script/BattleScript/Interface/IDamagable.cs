using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DamageInfo
{
    [Header("[Damage Info]")]
    public Character causer;
    public float damageAmount;
    public UnityEvent OnDamage;
    public Vector3 hitPoint;

    public DamageInfo(Character causer, float damageAmount, UnityEvent onDamage, Vector3 hitPoint)
    {
        this.causer = causer;
        this.damageAmount = damageAmount;
        this.OnDamage = onDamage;
        this.hitPoint = hitPoint;
    }
}

public interface IDamagable
{
    public void TakeDamage(DamageInfo damageInfo, Vector3 hitDirection);

    public void Dead(DamageInfo damageInfo);
}
