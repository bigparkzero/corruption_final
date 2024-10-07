using UnityEngine;

public interface IAnimationComponent
{
    public abstract void PlayFalling();
    public abstract void PlayLanding();
    public abstract void PlayClip(AnimationClip clip);
    public abstract void PlayStagger();
    public abstract void PlayKnockback();
    public abstract void PlayDead();

    public abstract void PlayHitStop();
}
