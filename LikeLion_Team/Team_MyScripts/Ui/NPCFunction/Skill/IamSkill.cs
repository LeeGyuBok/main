using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IamSkill
{
    public Sprite SkillImage { get; }
    public string SkillName { get; }
    public float BaseStatus { get; }
    public float BaseEnforceStatus { get; }
    public float BaseCoolTime { get; }
    public float TotalStatus { get; }
    public bool IsAvailable { get; }
    
    public SkillNode SkillTree { get; }
    public List<SkillNode> SkillTreeValue { get; }
    public List<SkillSubOption> SubOptionList { get; }

    public IEnumerator UseSkill(GameObject gameObject);
    public void Enforce(SkillNode node);
    public void Withdraw(SkillNode node);
    public void UnLockSkill();
    public void LockSkill();
}
