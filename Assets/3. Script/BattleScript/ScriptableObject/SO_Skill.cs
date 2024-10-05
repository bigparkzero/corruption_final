using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SkillData
{
    [Header("[Skill Data]")]
    public string skillName;
    public List<EKeyName> inputs;
    public List<AnimationClip> clips;

    [Header("[Filter Option]")]
    public EMovementTypeState movementTypeState;
    public bool isSprint;

    [Header("[Hit Reaction Data]")]
    public List<HitReactionDatas> hitReactionDatas;
}

[CreateAssetMenu(fileName = "new Skill Data", menuName = "Scriptable Object/Skill Data")]
public class SO_Skill : ScriptableObject
{
    public SkillData skillData;
}
