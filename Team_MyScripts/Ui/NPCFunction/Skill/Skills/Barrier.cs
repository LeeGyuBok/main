using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : IamSkill
{
    public Sprite SkillImage { get; }  = Resources.Load<Sprite>("Skill/Barrier");
    public string SkillName { get; } = "Barrier";
    
    private float baseStatus;
    public float BaseStatus
    {
        get => baseStatus;
        private set
        {
            baseStatus = value;
        }
    }
    private float baseEnforceStatus;
    public float BaseEnforceStatus
    {
        get => baseEnforceStatus;
        private set
        {
            baseEnforceStatus = value;
        }
    }
    private float baseCoolTime;
    public float BaseCoolTime
    {
        get => baseCoolTime;
        private set
        {
            baseCoolTime = value;
        }
    }
    public float TotalStatus { get; private set; }
    public bool IsAvailable { get; private set; }
    public SkillNode SkillTree { get; private set; }
    public List<SkillNode> SkillTreeValue { get; private set; }
    public List<SkillSubOption> SubOptionList { get; private set; }

    public Barrier()
    {
        SubOptionList = new List<SkillSubOption>();
        SkillTreeValue = new List<SkillNode>();
        
        baseStatus = 20f;
        baseEnforceStatus = 3f;
        BaseCoolTime = 80f;
        TotalStatus = 0;
        IsAvailable = false;

        SkillTree = SkillNode.CreatedSkillNode(8);
        List<SkillSubOption> subOptions = new List<SkillSubOption>();
        for (int i = 0; i < 8 ; i++)
        {
            subOptions.Add(new SkillSubOption(8));
        }
        //헤드노드(1번노드)
        SkillTree.SetPossibleSubOption(subOptions[0]);
        //2번노드
        SkillTree.NextNode.SetPossibleSubOption(subOptions[1]);
        //3번노드
        SkillTree.NextNode.NextNode.SetPossibleSubOption(subOptions[2]);
        
        SkillSubOption subOption = new SkillSubOption(11);
        SkillTree.NextNode.NextNode.SetPossibleSubOption(subOption);
        //4번노드
        SkillTree.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[3]);
        
        subOption = new SkillSubOption(12);
        SkillTree.NextNode.NextNode.NextNode.SetPossibleSubOption(subOption);
        //5번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[4]);
        //6번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[5]);
        //7번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[6]);
        
        subOption = new SkillSubOption(13);
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOption);
        //8번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[7]);
        
        SetTotalStatus();
    }
    public IEnumerator UseSkill(GameObject gameObject)
    {
        if (UiManager.Instance.Player.IsGetBarrier)
        {
            //코루틴 중단 코드
            //이 코드는 그 뭐냐, 쿨타임이 적용되는 시점에 삭제하세요
            yield break;
        }
        GameObject barrier = SkillManager.Instance.InstantiateBarrierPrefabObject();
        barrier.SetActive(false);
        UiManager.Instance.Player.SetBarrier();
        barrier.SetActive(true);
        UiManager.Instance.SetPlayerHp();
        //보호막지속시간
        yield return new WaitForSeconds(3f);
        UiManager.Instance.Player.DestroyBarrier();
        UiManager.Instance.SetPlayerHp();
    }

    public void Enforce(SkillNode node)
    {
        SkillTreeValue.Add(node);
        if (SkillTreeValue.Count > 0)
        {
            UnLockSkill();
        }
        SetTotalStatus();
    }

    public void Withdraw(SkillNode node)
    {
        SkillTreeValue.Remove(node);
        if (SkillTreeValue.Count < 1)
        {
            LockSkill();
        }
        SetTotalStatus();
    }

    public void UnLockSkill()
    {
        if (!IsAvailable)
        {
            IsAvailable = true;
        }
    }

    public void LockSkill()
    {
        if (IsAvailable)
        {
            IsAvailable = false;
        }
    }
    
    private void SetTotalStatus()
    {
        if (SkillTreeValue.Count - 1 < 0)
        {
            TotalStatus = BaseStatus + BaseEnforceStatus * SkillTreeValue.Count;       
        }
        else
        {
            TotalStatus = BaseStatus + BaseEnforceStatus * (SkillTreeValue.Count - 1);
        }
    }
}
