using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;

public enum ESkillState
{
    Stop,
    Playing,
}

public class AttackComponent : MonoBehaviour
{
    public CharacterActor owner;

    [Header("[Input]")]
    EKeyName currentKey = EKeyName.None;
    List<EKeyName> currentInputs;
    int inputCount;

    [Header("[Time]")]
    public float resetTime = 0.5f;
    public float inputDelayTime = 0.1f;
    public float transitionDuration = 0.1f;
    float inputTimer = 0f;

    [Header("[Skill Data]")]
    public ESkillState skillState = ESkillState.Stop;
    [SerializeField] List<SO_Skill> allSkills;
    List<SO_Skill> filteredSkills;
    List<AnimationClip> savedSkills;
    SkillData currentSkillData;
    public SkillData CurrentSkillData { get { return currentSkillData; } }
    int currentSkillIndex = 0;
    public int CurrentSkillIndex { get { return currentSkillIndex <= 0 ? 0 : currentSkillIndex - 1; } }

    [Header("[Coroutine]")]
    Coroutine C_ComboSequence;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        owner = GetComponent<CharacterActor>();
        currentInputs = new List<EKeyName>();
        savedSkills = new List<AnimationClip>();
    }

    public void SetInput(EKeyName key)
    {
        if (key != EKeyName.Attack && key != EKeyName.Skill1 &&
            key != EKeyName.Skill2 && key != EKeyName.Skill3 &&
            key != EKeyName.Skill4) return;

        if (inputTimer <= Time.time)
        {
            inputTimer = Time.time + inputDelayTime;
            currentKey = key;
            currentInputs.Add(key);

            SaveCurrentSkills();
        }
    }

    List<SO_Skill> FilterCombo()
    {
        filteredSkills = allSkills.ToList();
        filteredSkills.RemoveAll(data => data.skillData.movementTypeState != owner.locomotionData.movementTypeState ||
                                    data.skillData.isSprint != owner.locomotionData.isSprint);
        filteredSkills.RemoveAll(data => data.skillData.inputs.Count < currentInputs.Count ||
                                    data.skillData.inputs[inputCount] != currentInputs[inputCount]);

        return filteredSkills;
    }

    void SaveCurrentSkills() // for Player
    {
        List<SO_Skill> filtered = FilterCombo();
        
        if (filtered.Count > 0)
        {
            if (filtered[0].skillData.inputs[inputCount] == currentKey)
            {
                currentSkillData = filtered[0].skillData;
                savedSkills.Add(filtered[0].skillData.clips[inputCount]);
                inputCount++;
            }

            PlaySkill();
        }
        else
        {
            if (inputCount <= 0 && currentInputs.Count > 0)
            {
                ResetSkill();
            }
        }
    }

    void SaveCurrentSkills(SkillData skillData) // for AI
    {
        ResetSkill();

        currentSkillData = skillData;
        for (int i = 0; i < skillData.clips.Count; i++)
        {
            savedSkills.Add(skillData.clips[i]);
        }

        PlaySkill();
    }

    void PlaySkill()
    {
        if (C_ComboSequence != null) StopCoroutine(C_ComboSequence);
        C_ComboSequence = StartCoroutine(SkillSequence());
    }

    IEnumerator SkillSequence()
    {
        yield return new WaitWhile(() => skillState == ESkillState.Playing);

        while (savedSkills.Count > 0)
        {
            owner.locomotionData.isSprint = false;

            owner.animator.CrossFadeInFixedTime(savedSkills[0].name, transitionDuration);
            owner.hitReactionComponent.SetMaxIndex(savedSkills[0]);
            savedSkills.RemoveAt(0);
            currentSkillIndex++;

            yield return new WaitForFixedUpdate();
            yield return new WaitWhile(() => skillState == ESkillState.Playing);
        }

        yield return new WaitForSeconds(resetTime);
        ResetSkill();
    }

    public void ResetSkill()
    {
        skillState = ESkillState.Stop;
        owner.animator.applyRootMotion = false;
        owner.hitReactionComponent.ResetIndex();
        currentKey = EKeyName.None;
        currentInputs.Clear();
        currentSkillData = default;
        currentSkillIndex = 0;
        savedSkills.Clear();
        inputCount = 0;
    }

    public void OnSkillBegin()
    {
        skillState = ESkillState.Playing;
    }

    public void OnSkillEnd(int skillEnd)
    {
        skillState = ESkillState.Stop;

        if (skillEnd > 0 || savedSkills.Count <= 0)
        {
            ResetSkill();
        }
    }

    public void SetSaveCombo(SO_Skill skill)
    {
        SaveCurrentSkills(skill.skillData);
    }
}
