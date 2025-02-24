using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : IamSkill
{
    //워프시 이동거리
    public Sprite SkillImage { get; private set; } = Resources.Load<Sprite>("Skill/Blink");
    public string SkillName { get; private set; } = "Blink";

    //기본 이동거리
    private float baseStatus;
    public float BaseStatus
    {
        get => baseStatus;
        private set
        {
            baseStatus = value;
        }
    }
    
    //강화시 증가하는 이동거리
    private float baseEnforceStatus;
    public float BaseEnforceStatus
    {
        get => baseEnforceStatus;
        private set
        {
            baseEnforceStatus = value;
        }
    }
    
    //기본 쿨타임
    private float baseCoolTime;

    public float BaseCoolTime
    {
        get => baseCoolTime;
        private set
        {
            baseCoolTime = value;
        }
    }

    /// <summary>
    /// actually using value in skill
    /// </summary>
    public float TotalStatus { get; private set; }
    
    public bool IsAvailable { get; private set; }

    /// <summary>
    /// it is head node.
    /// </summary>
    public SkillNode SkillTree { get; private set;}
    
    /// <summary>
    /// if IsInvested == true => Add().
    /// </summary>
    public List<SkillNode> SkillTreeValue { get; private set; }
    
    //새로운 클래스인 서브스킬 클래스로 바꿀수도? -> 바꿈..
    public List<SkillSubOption> SubOptionList { get; private set;}
    
    

    public Blink()
    {
        SubOptionList = new List<SkillSubOption>();
        SkillTreeValue = new List<SkillNode>();
        
        baseStatus = 3f;
        baseEnforceStatus = 0.4f;
        BaseCoolTime = 65f;
        TotalStatus = 0;
        IsAvailable = false;

        SkillTree = SkillNode.CreatedSkillNode(8);
        List<SkillSubOption> subOptions = new List<SkillSubOption>();
        for (int i = 0; i < 8 ; i++)
        {
            subOptions.Add(new SkillSubOption(1));
        }
        //헤드노드(1번노드)
        SkillTree.SetPossibleSubOption(subOptions[0]);
        //2번노드
        SkillTree.NextNode.SetPossibleSubOption(subOptions[1]);
        //3번노드
        SkillTree.NextNode.NextNode.SetPossibleSubOption(subOptions[2]);
        
        SkillSubOption subOption = new SkillSubOption(3);
        SkillTree.NextNode.NextNode.SetPossibleSubOption(subOption);
        //4번노드
        SkillTree.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[3]);
        
        subOption = new SkillSubOption(4);
        SkillTree.NextNode.NextNode.NextNode.SetPossibleSubOption(subOption);
        //5번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[4]);
        //6번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[5]);
        //7번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[6]);
        
        subOption = new SkillSubOption(7);
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOption);
        //8번노드
        SkillTree.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode.SetPossibleSubOption(subOptions[7]);
        
        SetTotalStatus();
    }
    
    //스킬사용시 사용하는 변수
    public bool IsFlashing { get; private set; } = false;
    public Rigidbody Rigidbody { get; private set; }
    public Camera mainCamera { get; private set; }
    public float FlashDuration { get; private set; } = 0.1f;

    public IEnumerator UseSkill(GameObject gameObject)
    {
        Debug.Log(gameObject.name);
        foreach (SkillSubOption option in SubOptionList)
        {
            if (option.ActiveTime == ActiveTime.Start)
            {
                option.Activate("Start");
            }
        }
        
        if (Rigidbody is null)
        {
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            mainCamera = gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        }
        
        IsFlashing = true;
        //flashVector 는 아래 두개 중 하나만 살려서 사용
        //얘는 이동벡터에 따라서
        /*Vector3 flashVector = gameObject.transform.GetChild(0).gameObject.transform.forward.normalized;*/
        
        //애는 카메라가 바라보는 방향으로 
        //점멸쓸 때, 아래의 버그가남.
        /*MissingComponentException: There is no 'Camera' attached to the "GroundCheckPosition" game object, but a script is trying to access it.
            You probably need to add a Camera to the game object "GroundCheckPosition". Or your script needs to check if the component is attached before using it.*/
        Vector3 flashVector = mainCamera.gameObject.transform.forward.normalized;
        
        if (flashVector.y <= 0)
        {
            flashVector.y = 0f;
        }
        
        
        Vector3 startPosition = gameObject.transform.position;
        
        float elapsedTime = 0f;

        while (elapsedTime < FlashDuration)
        {
            Rigidbody.MovePosition(startPosition + flashVector * (TotalStatus * (elapsedTime / FlashDuration)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        foreach (SkillSubOption option in SubOptionList)
        {
            if (option.ActiveTime == ActiveTime.Middle)
            {
                option.Activate("Middle");
            }
        }
        IsFlashing = false;
        foreach (SkillSubOption option in SubOptionList)
        {
            if (option.ActiveTime == ActiveTime.End)
            {
                option.Activate("End");
            }
        }
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
    
    //수정예정. 증가하는 능력치의 연산식 - 스킬트리에서 스킬포인트 투자부분
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
