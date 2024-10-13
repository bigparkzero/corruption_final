using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDestructable : Character
{
    protected override void Start()
    {
        GetComponent<StatsComponent>().OnDead += Die;
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

    public override void StopEveryCoroutines()
    {

    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
