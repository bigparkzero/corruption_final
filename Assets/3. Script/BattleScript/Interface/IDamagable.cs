using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DamageInfo
{
    [Header("[Damage Info]")]
    public Character causer;
    public float damageAmount;
    public UnityEvent OnDamage;

    public DamageInfo(Character causer, float damageAmount, UnityEvent onDamage)
    {
        this.causer = causer;
        this.damageAmount = damageAmount;
        this.OnDamage = onDamage;
    }
}

public interface IDamagable
{
    public void TakeDamage(DamageInfo damageInfo);

    public void Dead(DamageInfo damageInfo);
}
