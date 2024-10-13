using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : CharacterBase, IDamagable
{
    [HideInInspector] public StatsComponent statsComponent;

    public string characterTag;

    public List<GameObject> bloods = new List<GameObject>();
    public bool useBlood = true;

    protected override void Awake()
    {
        statsComponent = GetComponent<StatsComponent>();
    }

    public bool HasTag(string tag)
    {
        return characterTag.Equals(tag);
    }

    public void SetCharacterTag(string tag)
    {
        characterTag = tag;
    }

    public void TakeDamage(DamageInfo damageInfo, Vector3 hitDirection)
    {
        statsComponent.Damaged(damageInfo.damageAmount);
        if (statsComponent.GetStatData.hp <= 0)
        {
            statsComponent.OnDeadAction?.Invoke(damageInfo);
        }

        damageInfo.OnDamage?.Invoke();

        if (useBlood && bloods.Count > 0)
        {
            int rand = Random.Range(0, bloods.Count);
            Quaternion bloodRotation = Quaternion.LookRotation(-hitDirection);
            GameObject blood = Instantiate(bloods[rand], damageInfo.hitPoint, bloodRotation);
            blood.transform.localScale = Vector3.one * 2f;
            Destroy(blood, 1f);
        }

        if (damageInfo.causer.characterTag == "Player")
        {
            var player = damageInfo.causer as CharacterPlayer;
            if (player != null)
            {
                player.hitTraceComponent.PlayHitStop();
            }
        }
    }

    public virtual void Dead(DamageInfo damageInfo)
    {
        gameObject.SetActive(false);
    }

    public override void StopEveryCoroutines()
    {

    }

    protected override void AirControl()
    {

    }

    protected override void ApplyGravity()
    {

    }

    protected override void CheckGround()
    {

    }
}
