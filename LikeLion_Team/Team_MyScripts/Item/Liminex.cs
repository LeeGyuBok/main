using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNode//점멸, 배리어, 해킹 등의 스킬들이 모두 공유하는 스킬 트리 구조.
{
    public SkillNode FrontNode;
    public SkillNode NextNode;

    public int name;
    public SkillSubOption SelectedOption { get; private set; }
    public List<SkillSubOption> PossibleSubOptions { get; private set; }

    public bool IsInvested { get; private set; } = false;
    /*
     * 스킬: 스킬이름, 스킬효과(능력 등)
     *
     */
    public static SkillNode CreatedSkillNode(int size)
    {
        SkillNode head = new SkillNode();
        head.name = 0;
        head.FrontNode = head;

        List<SkillNode> list = new List<SkillNode> { head };
        for (int i = 1; i <= size; i++)
        {
            SkillNode newNode = new SkillNode();
            list.Add(newNode);

            // 현재 노드와 다음 노드 설정
            list[i - 1].NextNode = newNode;
        
            newNode.FrontNode = list[i - 1];
            newNode.name = newNode.FrontNode.name + 1;
        }

        // 마지막 노드가 자기 자신을 참조
        SkillNode lastNode = list[size-1];
        lastNode.NextNode = lastNode;

        return head;
    }

    public SkillNode()
    {
        PossibleSubOptions = new List<SkillSubOption>();
    }

    public void InvestSkillPoint()
    {
        if (!IsInvested)
        {
            IsInvested = true;
        }
        else
        {
            Debug.Log("Already invested");
        }
    }

    public void WithdrawSkillPoint()
    {
        if (IsInvested)
        {
            IsInvested = false;
        }
        else
        {
            Debug.Log("not invested");
        }
    }

    //나중에 프라이빗으로 바꾸던가 해야함.
    public void SetPossibleSubOption(SkillSubOption option)
    {
        if (PossibleSubOptions != null)
        {
            if (PossibleSubOptions.Count < 3)
            {
                PossibleSubOptions.Add(option);    
            }
            else
            {
                //Debug.Log("Full");
                option = null;
            }
        }
        else
        {
            PossibleSubOptions = new List<SkillSubOption> { option };
        }
    }

    public void SetSubOption(SkillSubOption option)
    {
        if (IsInvested)
        {
            SelectedOption = option;    
        }
    }

    public void RemoveSubOption()
    {
        if (!IsInvested)
        {
            SelectedOption = null;    
        }
    }


    public List<int> GetNextPossibleSubOptionsCount(SkillNode node, List<int> values = null)
    {
        if (values == null)
            values = new List<int>();
        
        values.Add(node.PossibleSubOptions.Count);
        
        
        if (!node.NextNode.Equals(node))
        {
            GetNextPossibleSubOptionsCount(node.NextNode, values);
        }
        else
        {
            return values;
        }
       
        return values;
    }

    public List<int> GetFrontPossibleSubOptionsCount(SkillNode node, List<int> values = null)
    {
        if (values == null)
            values = new List<int>();
        
        values.Add(node.PossibleSubOptions.Count);
       
        if (!node.FrontNode.Equals(node))
        {
            GetFrontPossibleSubOptionsCount(node.FrontNode, values);
        }
        else
        {
            return values;
        }

        return values;
    }
    
    public List<SkillSubOption> GetSubOptions(SkillNode node, List<SkillSubOption> values = null)
    {
        if (values == null)
            values = new List<SkillSubOption>();

        for (int i = 0; i < node.PossibleSubOptions.Count; i++)
        {
            values.Add(node.PossibleSubOptions[i]);    
        }
        
        
        if (!node.NextNode.Equals(node))
        {
            GetSubOptions(node.NextNode, values);
        }

        return values;
    }

    public List<SkillNode> CircuitNextNodes(SkillNode node, List<SkillNode> values = null)
    {
        if (values == null)
            values = new List<SkillNode>();
        
        values.Add(node);
        if (!node.NextNode.Equals(node))
        {
            CircuitNextNodes(node.NextNode, values);
        }
        else
        {
            return values;
        }

        return values;
    }
    
    public List<SkillNode> CircuitFrontNodes(SkillNode node, List<SkillNode> values = null)
    {
        if (values == null)
            values = new List<SkillNode>();
        
        values.Add(node);
        if (!node.FrontNode.Equals(node))
        {
            CircuitFrontNodes(node.FrontNode, values);
        }
        else
        {
            return values;
        }

        return values;
    }
}

public class Liminex : Item_SO, IUpgradeable
{
    public Liminex(ItemData_SO data_SO) : base(data_SO)
    {
        //스킬 데이터를 미리 생성해주세요. 나중에 추가되는 2개의 스킬도
        Blink = new Blink();
        Barrier = new Barrier();
        skills = new List<IamSkill> { Blink, Barrier };
        UpgradeMaterialsAndNeedCount = new Dictionary<ItemData_SO, int>();
        SetUpgradeMaterialsAndNeedCount();
    }

    public Dictionary<ItemData_SO, int> UpgradeMaterialsAndNeedCount { get; private set; }
    public bool IsUpgradeable { get; private set; } = false;
    public int PossibleUpgradeCount { get; private set; } = 30;
    public int CurrentUpgradeCount { get; private set; } = 0;
    public int SkillPoint { get; private set; } = 0;

    public bool IsActivate { get; private set; } = false;

    public Blink Blink { get; private set; }
    public Barrier Barrier { get; private set; }

    public List<IamSkill> skills { get; private set; }

    private void SetSkillPoint()
    {
        SkillPoint += 1;
        Debug.Log($"point: {SkillPoint}");
    }

    public void InvestSkillPoint()
    {
        SkillPoint -= 1;
        Debug.Log($"point: {SkillPoint}");
    }

    public void WithdrawSkillPoint()
    {
        SkillPoint += 1;
        Debug.Log($"point: {SkillPoint}");
    }
    
    public void SetUpgradeState()
    {
        if (IsActivate)
        {
            CurrentUpgradeCount += 1;
            if (CurrentUpgradeCount == PossibleUpgradeCount)
            {
                if (IsUpgradeable)
                {
                    LimitUpgrade();   
                }
            }
            SetSkillPoint();    
        }
        else
        {
            Debug.Log("Not Activate");
        }
    }

    //리미넥스 수리를 통한 활성화.
    public void RepairLiminex()
    {
        IsActivate = true;
        IsUpgradeable = true;
    }

    private void LimitUpgrade()
    {
        IsUpgradeable = false;
    }

    private void SetUpgradeMaterialsAndNeedCount()
    {
        //재료아이템의 개수 정하기 2~4종류
        int circuitCount = 2;
        //여기도 테스트용
        /*int circuitCount = Random.Range(2, 5);*/
        int maxCount = ItemPool_SO.Instance.ItemDataBase.Count;

        for (int i = 0; i < circuitCount; i++)
        {
            //블랭크, 라스트블랭크 빼고 (0, 1) 두번째아이템부터
            //마지막 숫자는 포함안하므로 +1
            //여기는 랜덤로직
            /*int itemPickCircuitCount = Random.Range(2, maxCount + 1);
            ItemData_SO materialData = ItemManager_SO.Instance.Items.ItemDataBase[itemPickCircuitCount];*/

            //여기는 테스트용 로직
            //얼로이랑 일렉트로니컬 서킷 1개씩
            int itemPickCircuitCount = Random.Range(2, 4);
            ItemData_SO materialData = ItemPool_SO.Instance.ItemDataBase[itemPickCircuitCount];

            if (UpgradeMaterialsAndNeedCount.ContainsKey(materialData))
            {
                i--;
            }
            else
            {
                //개수 설정 로직
                //int randomCount = Random.Range(1, 4);
                int randomCount = 1;
                UpgradeMaterialsAndNeedCount[materialData] = randomCount;
            }
        }
    }

    public IEnumerator UseSkill(GameObject gameObject, IamSkill skillAble)
    {
        return skillAble.UseSkill(gameObject);
    }

    public int RemainSkillPoint()
    {
        return CurrentUpgradeCount - SkillPoint;
    }
}
