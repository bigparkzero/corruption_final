using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    private Character owner;

    [Header("[Skill Component]")]
    [SerializeField] private List<Skill> bindingSkills;
    [SerializeField] private List<Skill> activeSkills;
    [SerializeField] private List<Skill> inactiveSkills;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        owner = GetComponent<Character>();
        GameObject skillSlot = new GameObject("Skill Slot");
        skillSlot.transform.SetParent(owner.transform);

        foreach (var skill in bindingSkills)
        {
            Skill spawnSkill = Instantiate(skill, skillSlot.transform);
            spawnSkill.owner = owner;
            activeSkills.Add(spawnSkill);
        }
    }

    public void Play(string skillName, Character target = null)
    {
        Skill skill = activeSkills.Find(data => data.skillName == skillName);
        if (skill != null)
        {
            if (skill.IsSkillReady())
            {
                skill.Active(target);
                skill.StartCooldown();

                //if (owner.statsComponent)
                //{
                //    owner.statsComponent.DecreaseAttribute(EAttributeType.Mana, skill.mana);
                //}
            }
        }
        else
        {
            Debug.LogError("Skill is null");
        }
    }
}
