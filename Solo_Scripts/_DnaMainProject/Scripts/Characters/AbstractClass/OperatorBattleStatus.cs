using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Role
{
    Assassin,
    Attacker,
    Defender,
    Supporter
}

public struct HealthPoint
{
    public int CurrentPoint;
    public readonly int InitialPoint;

    public HealthPoint(int initialCurrentPoint)
    {
        CurrentPoint = initialCurrentPoint;
        InitialPoint = initialCurrentPoint;
    }

    public void SetHealthPoint(int coefficient)
    {
       CurrentPoint = InitialPoint * (int)(100 + coefficient)/100;
    }

    public void Heal(int amount, int maxHp)
    {
       CurrentPoint += amount;
        if (CurrentPoint > maxHp)
        {
            CurrentPoint = maxHp;
        }
    }

    public void TakeDamage(int amount)
    {
        CurrentPoint -= amount;
        if (CurrentPoint < 0)
        {
            CurrentPoint = 0;
        }
    }
}

public struct AttackPoint
{
    public int CurrentPoint;
    public readonly int InitialPoint;

    public AttackPoint(int initialCurrentPoint)
    {
        CurrentPoint = initialCurrentPoint;
        InitialPoint = initialCurrentPoint;
    }
    
    public void SetAttackPoint(int coefficient)
    {
        CurrentPoint = InitialPoint * (int)(100 + coefficient)/100;
        
    }
}

public struct DefensePoint
{
    public int CurrentPoint;
    public readonly int InitialPoint;

    public DefensePoint(int initialCurrentPoint)
    {
        CurrentPoint = initialCurrentPoint;
        InitialPoint = initialCurrentPoint;
    }
    
    public void SetDefensePoint(int coefficient)
    {
        CurrentPoint = InitialPoint * (int)(100 + coefficient)/100;
    }
}

public struct SpeedPoint
{
    public int CurrentPoint;
    public readonly int InitialPoint;

    public SpeedPoint(int initialCurrentPoint)
    {
        CurrentPoint = initialCurrentPoint;
        InitialPoint = initialCurrentPoint;
    }
    
    public void SetSpeedPoint(int coefficient)
    {
        CurrentPoint = InitialPoint * (int)(100 + coefficient)/100;
    }
}

//적도 필요함
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class OperatorBattleStatus : MonoBehaviour
{
    [SerializeField] protected BaseOperatorBattleStatusData baseBattleStatusData;
    public BaseOperatorBattleStatusData BattleStatusData => baseBattleStatusData;

    public HealthPoint Health;
    public AttackPoint Attack;
    public DefensePoint Defense;
    public SpeedPoint Speed;
    public Role Role { get; protected set; }
    public GeneType GeneType { get; protected set; }
    
    public Dna Dna { get; private set; }

    public int HealthPointCoefficient { get; protected set; }
    public int AttackPointCoefficient { get; protected set; }
    public int DefensePointCoefficient { get; protected set; }
    public int SpeedPointCoefficient { get; protected set; }
    
    public int MaxHealthPoint { get; protected set; }
    public int MaxAttackPoint{ get; protected set;}
    public int MaxDefensePoint { get; protected set; }
    public int MaxSpeedPoint { get; protected set; }

    protected virtual void Awake()
    {
        SetBaseStatus();
        SetMaxStatus();
    }

    //초기 능력치 셋팅
    public virtual void SetBaseStatus(CharacterOverrideData data = default)
    {
        if (data == default)
        {
            Health = new HealthPoint(baseBattleStatusData.HealthPoint);
            Attack = new AttackPoint(baseBattleStatusData.AttackPower);
            Defense = new DefensePoint(baseBattleStatusData.DefensePower);
            Speed = new SpeedPoint(baseBattleStatusData.Speed);
            Role = baseBattleStatusData.Role;
            GeneType = baseBattleStatusData.Gene;
            Dna = new Dna(GeneType);    
        }
        else
        {
            Health = new HealthPoint(baseBattleStatusData.HealthPoint);
            Attack = new AttackPoint(baseBattleStatusData.AttackPower);
            Defense = new DefensePoint(baseBattleStatusData.DefensePower);
            Speed = new SpeedPoint(baseBattleStatusData.Speed);
            
            Role = baseBattleStatusData.Role;
            GeneType = baseBattleStatusData.Gene;
            Dna = new Dna(GeneType);

            for (int i = 0; i < data.GeneIDList.Count; i++)
            {
                Gene gene = GameImmortalManager.Instance.Genes.Find(mathGene => mathGene.GeneID == data.GeneIDList[i]);
                Dna.TryInsertGene(gene);
            }
            
        }


        SetCoefficient();
        //StartCoroutine(Dna.CycleSkill.ActivateCycleSkill());
    }
    
    #region SetStatus

    //Dna,  Role과 Dna 상관관계 는 나중에
    
    public void SetCoefficient()
    {
        HealthPointCoefficient = Dna.TotalHealthCoefficient;
        Health.SetHealthPoint(HealthPointCoefficient);
        
        AttackPointCoefficient = Dna.TotalAttackCoefficient;
        Attack.SetAttackPoint(AttackPointCoefficient);
        
        DefensePointCoefficient = Dna.TotalDefenceCoefficient;
        Defense.SetDefensePoint(DefensePointCoefficient);
        
        SpeedPointCoefficient = Dna.TotalSpeedCoefficient;
        Speed.SetSpeedPoint(SpeedPointCoefficient);

        SetMaxStatus();
    }
    
    private void SetMaxStatus()
    {
        SetMaxHealth();
        SetMaxAttack();
        SetMaxDefense();
        SetMaxSpeed();
    }
    
    private void SetMaxHealth()
    {
        MaxHealthPoint = Health.CurrentPoint;
    }
    private void SetMaxAttack()
    {
        MaxAttackPoint = Attack.CurrentPoint;
    }
    private void SetMaxDefense()
    {
        MaxDefensePoint = Defense.CurrentPoint;
    }
    private void SetMaxSpeed()
    {
        MaxSpeedPoint = Speed.CurrentPoint;
    }
    #endregion
    

    #region State

    protected IState currentState; // 현재 상태. 적도 공유하는 변수.
    public bool NowAttachToTarget { get; set; } //적도
    protected CapsuleCollider CombatDetectingCapsuleCollider;//적도
    public Rigidbody CharacterRigidbody { get; protected set; }//적도

    public int ObstacleLayerMask { get; private set; }//아군용

    protected virtual void Start()
    {
        ObstacleLayerMask = 1 << LayerMask.NameToLayer("Obstacles");
        currentState.Enter();
    }

    protected virtual void Update()
    {
        // 현재 상태에서 해야 할 작업을 실행
        if (Health.CurrentPoint < 1)
        {
            DestroyImmediate(gameObject);
        }
        currentState.Execute();
    }

    // 상태 변경 메서드
    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(); // 기존 상태 종료'
        }

        currentState = newState;
        currentState.Enter(); // 새로운 상태 시작
    }

    public CapsuleCollider GetCombatCapsuleCollider()
    {
        return CombatDetectingCapsuleCollider;
    }
    
    #endregion
}

