using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class EquipItem_Weapon : Item_SO, IRepairable, IEquipable, IRarible, IWeaponUpgradeable
{
    //원거리무기는 어떡하지? 범위를 0으로 해야겠다.. 
    //실제 적용되는 능력치
    public int AttackRange { get; private set;}
    public int GuardEfficiency { get; private set;}
    public int AttackPower { get; private set;}
    public int AttackSpeed { get; private set;}
    
    //기본능력치. 이것보다 낮아질 수 없음.
    public WeaponStatus_SO WeaponStatus { get; private set; }
    
    //착용여부
    public bool IsEquip { get; set; }
    //현재내구도
    public int CurrentDurability { get; private set; }
    //최대내구도
    public int MaxDurability { get; private set; }
    //내구도차이에 의한 수리 가능 여부
    public bool PossibleRepair { get; private set; }
    
    //희귀도 계수
    public int coefficient { get; private set; } = 8;
    public EnumItemRarity Rarity { get; private set; }
    public EnumItemRarity MaxRarity { get; private set; }
    public EnumItemRarity MinRarity { get; private set; } = EnumItemRarity.Common;
    
    //업그레이드
    //업그레이드에 필요한 재료
    public Dictionary<ItemData_SO, int> UpgradeMaterialsAndNeedCount { get; private set; }
    //업그레이드 차수 당 추가능력치(1.내구도, 2.공격력)
    public Dictionary<int, AdditionalWeaponStatus> AdditionalStatusByUpgrade { get; private set; }
    //업그레이드 차수
    public Stack<AdditionalWeaponStatus> UpgradeCount { get; private set; }
    //업그레이드를 통해 추가된 내구도
    public int TotalAdditionalDurability { get; private set; } = 0;
    //업그레이드를 총해 추가된 공격력
    public int TotalAdditionalAttackPower { get; private set; } = 0;
    //업그레이드 가능 여부
    public bool IsUpgradeable { get; private set; } = true;
    //업그레이드 가능한 회 수
    public int PossibleUpgradeCount { get; private set; } = 0;
    //현재 강화 차수
    public int CurrentUpgradeCount { get; private set; } = 0;
    
    public bool IsCrash { get; private set; } = false;
    
    /// <summary>
    /// Crafted item status.
    /// </summary>
    /// <param name="data_SO"></param>
    /// <param name="weaponStatusSo"></param>
    /// <param name="rarity">MaxRarity's Max == Legendary</param>
    /// <param name="crafting"></param>
    public EquipItem_Weapon(ItemData_SO data_SO, WeaponStatus_SO weaponStatusSo, EnumItemRarity rarity, bool crafting) : base(data_SO)
    {
        if (crafting)
        {
            WeaponStatus = weaponStatusSo;
            Rarity = rarity;
            SetStatusByRarity(rarity);
            
            UpgradeMaterialsAndNeedCount = new Dictionary<ItemData_SO, int>();
            AdditionalStatusByUpgrade = new Dictionary<int, AdditionalWeaponStatus>();
            UpgradeCount = new Stack<AdditionalWeaponStatus>();
            
            PossibleUpgradeCount = (int)rarity + 3;
            
            SetUpgradeMaterialsAndNeedCount();
            //단 1부터. 왜냐하면 0강은 강화를 안한거니까.
            //스택.count == 0 -> 강화안함. 스택.count == 1 -> 강화 1회함.
            for (int i = 1; i <= PossibleUpgradeCount; i++)
            {
                //내구도는 10씩, 공격력은 기본공격력의 0.5배를 반올림.
                AdditionalStatusByUpgrade[i] = new AdditionalWeaponStatus(5, Mathf.RoundToInt(WeaponStatus.AttackPower * 0.5f));
            }
        }
        else
        {
            WeaponStatus = weaponStatusSo;
            Rarity = MinRarity;
            MaxRarity = rarity;
        }
        
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
            int itemPickCircuitCount = Random.Range(2, 4);
            ItemData_SO materialData = ItemPool_SO.Instance.ItemDataBase[itemPickCircuitCount];
            
            if (UpgradeMaterialsAndNeedCount.ContainsKey(materialData))
            {
                i--;
            }
            else
            {
                //개수 설정 로직
                int randomCount = Random.Range(1, 6);
                UpgradeMaterialsAndNeedCount[materialData] = randomCount;
            }
        }
    }

    public void RepairDurability(int restoreDegree)
    {
        CurrentDurability += restoreDegree;
        if (CurrentDurability >= MaxDurability)
        {
            CurrentDurability = MaxDurability;
        }
        SetPossibleRepair();

    }

    public void ConsumesDurability(int damage)
    {
        //여기는 내구도가 깎이는 로직
        CurrentDurability -= damage;
        SetPossibleRepair();
    }

    private void SetPossibleRepair()
    {
        if (MaxDurability > CurrentDurability)
        {
            PossibleRepair = true;
            if (CurrentDurability <= 0)
            {
                ImpossibleCrash();
            }
            else
            {
                PossibleEquip();
            }
        }
        else
        {
            PossibleRepair = false;
        }
    }

    public int Attack()
    {
        return AttackPower;
    }

    private void ImpossibleCrash()
    {
        IsCrash = true;
        if (IsCrash)
        {
            IsEquip = false;
        }
    }

    private void PossibleEquip()
    {
        IsCrash = false;
        if (!IsCrash)
        {
            IsEquip = true;    
        }
    }

    private void SetStatusByRarity(EnumItemRarity rarity)
    {
        float additional = (int)rarity * coefficient;
        MaxDurability = WeaponStatus.Durability + (int)(WeaponStatus.Durability*additional/10);
        AttackRange = WeaponStatus.AttackRange;
        GuardEfficiency = WeaponStatus.GuardEfficiency + (int)(WeaponStatus.GuardEfficiency*additional/10);
        AttackPower = WeaponStatus.AttackPower + (int)(WeaponStatus.AttackPower*additional/10);
        AttackSpeed = WeaponStatus.AttackSpeed + (int)(WeaponStatus.AttackSpeed*additional/10);
        CurrentDurability = MaxDurability;
    }

    public void ShowStatusByRarity(EnumItemRarity rarity)
    {
        SetStatusByRarity(rarity);
    }

    public void SetUpgradeState()
    {
        //차수 올리기
        UpgradeCount.Push(AdditionalStatusByUpgrade[CurrentUpgradeCount+1]);
        CurrentUpgradeCount += 1;
        PossibleUpgradeCount -= 1;
        
        if (PossibleUpgradeCount <= 0)
        {
            IsUpgradeable = false;
        }
        else
        {
            IsUpgradeable = true;
        }
        //강화로 인한 추가능력치 셋팅
        TotalAdditionalDurability += UpgradeCount.Peek().Durability;
        TotalAdditionalAttackPower += UpgradeCount.Peek().AttackPower;
        //강화로 인한 추가능력치를 더한 총 능력치 셋팅
        MaxDurability += TotalAdditionalDurability;
        
        //CurrentDurability = MaxDurability;
        
        AttackPower += TotalAdditionalAttackPower;

        SetPossibleRepair();
    }
}
