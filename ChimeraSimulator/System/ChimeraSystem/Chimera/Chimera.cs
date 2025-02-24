using System;
using System.Collections.Generic;
using UnityEngine;

public class Chimera : MonoBehaviour
{
    [SerializeField] private bool isMutant;
    public bool IsMutant => isMutant;
    [SerializeField] private GeneType geneType;
    public GeneType GeneType => geneType;
    
    public MainDna MainDna { get; private set; }
    public DnaMainSkill MainSkill { get; private set; }
    public List<DnaSubSkill> SubSkills { get; private set; }
    
    public float MaxHealthPoint { get; private set; }
    public float CurrentHealthPoint { get; private set; }
    public float AttackPoint { get; private set; }
    public float DefencePoint { get; private set; }
    public float AgilityPoint { get; private set; }
    
    
    public IChimeraState CurrentState { get; private set; }
    public IChimeraState LastState { get; private set; }
    public SpawnAndWaitState SpawnAndWaitState { get; private set; }
    
    public StandingState StandingState { get; private set; }
    
    public BasicAttackState BasicAttackState { get; private set; }
    public FirstSkillState FirstSkillState { get; private set; }
    public SecondSkillState SecondSkillState { get; private set; }
    public ThirdSkillState ThirdSkillState { get; private set; }
    public MainSkillState MainSkillState { get; private set; }
    
    public GroggyState GroggyState { get; private set; }
    public VulnerableState VulnerableState { get; private set; }
    public DodgeState DodgeState { get; private set; }
    public SurvivalState SurvivalState { get; private set; }
    public DeathState DeathState { get; private set; }
    
    public Dictionary<IDnaSkill, IChimeraState> SkillStates { get; private set; }

    public bool IsPoisoned;

    private float _decreasedAttackPoint;
    private float _decreasedDefencePoint;
    private float _decreasedAgilityPoint;
    
    private void Awake()
    {

    }

    private void Start()
    {
        CurrentState = SpawnAndWaitState;
        CurrentState?.EnterState();
    }
    
    public void ChangeState(IChimeraState state)
    {
        LastState = CurrentState;
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();
    }
    
    // Update is called once per frame
    void Update()
    {
        CurrentState?.Execute();
    }

    public void Initialize(ChimeraData chimeraData)
    {
        MainDna = chimeraData.MainDna;
        MainSkill = chimeraData.MainSkill;
        SubSkills = chimeraData.SubSkills;
        
        MaxHealthPoint = chimeraData.MaxHealthPoint;
        CurrentHealthPoint = chimeraData.CurrentHealthPoint;
        AttackPoint = chimeraData.AttackPoint;
        DefencePoint = chimeraData.DefencePoint;
        AgilityPoint = chimeraData.AgilityPoint;
        
        SpawnAndWaitState = new SpawnAndWaitState(this);
        StandingState = new StandingState(this);
        BasicAttackState = new BasicAttackState(this);
        SkillStates = new Dictionary<IDnaSkill, IChimeraState>();
        switch (SubSkills.Count)
        {
            case 0:
                break;
            case 1:
                FirstSkillState = new FirstSkillState(this, SubSkills[0]);
                SkillStates[SubSkills[0]] = FirstSkillState;
                break;
            case 2:
                FirstSkillState = new FirstSkillState(this, SubSkills[0]);
                SkillStates[SubSkills[0]] = FirstSkillState;
                SecondSkillState = new SecondSkillState(this, SubSkills[1]);
                SkillStates[SubSkills[1]] = SecondSkillState;
                break;
            case 3:
                FirstSkillState = new FirstSkillState(this, SubSkills[0]);
                SkillStates[SubSkills[0]] = FirstSkillState;
                SecondSkillState = new SecondSkillState(this, SubSkills[1]);
                SkillStates[SubSkills[1]] = SecondSkillState;
                ThirdSkillState = new ThirdSkillState(this, SubSkills[2]);
                SkillStates[SubSkills[2]] = ThirdSkillState;
                break;
        }
        MainSkillState = new MainSkillState(this);
        SkillStates[MainSkill] = MainSkillState;

        
        GroggyState = new GroggyState(this);
        VulnerableState = new VulnerableState(this);
        DodgeState = new DodgeState(this);
        SurvivalState = new SurvivalState(this);

        
        DeathState = new DeathState(this);
    }


    #region OnCombat

    public void TakeDamage(float damage, float skillCoefficient = 0)
    {
        if (skillCoefficient == 0)
        {
            float byDefencePoint = damage - DefencePoint * 0.2f;
            if (byDefencePoint < 0)
            {
                byDefencePoint = 50f;
            }
            CurrentHealthPoint -= byDefencePoint;    
        }
        else
        {
            float byDefencePoint = damage - DefencePoint * 0.2f;
            if (byDefencePoint < 0)
            {
                byDefencePoint = 50f;
            }
            CurrentHealthPoint -= byDefencePoint * skillCoefficient;
        }

        if (CurrentHealthPoint <= MaxHealthPoint * 0.4)
        {
            //ChangeState(SurvivalState);
            if (IsPoisoned)
            {
                Refresh();    
            }
            AttackPoint += AttackPoint * 0.3f;
            DefencePoint += DefencePoint * 0.3f;
            AgilityPoint += AgilityPoint * 0.3f;
            
        }
        
        if (CurrentHealthPoint <= 0)
        {
            CurrentHealthPoint = 0;
        }
    }
    
    public void Refresh()
    {
        ResetPoisoned();
    }

    public void SetPoisoned(float strength)
    {
        _decreasedAttackPoint = AttackPoint * (1 - strength);
        _decreasedDefencePoint = DefencePoint * (1 - strength);
        _decreasedAgilityPoint = AgilityPoint * (1 - strength);
        
        AttackPoint *= strength;
        DefencePoint *= strength;
        AgilityPoint *= strength;
        IsPoisoned = true;
    }

    private void ResetPoisoned()
    {
        if (!IsPoisoned) return;
        AttackPoint += _decreasedAttackPoint;
        DefencePoint += _decreasedDefencePoint;
        AgilityPoint += _decreasedAgilityPoint;

        _decreasedAttackPoint = 0;
        _decreasedDefencePoint = 0;
        _decreasedAgilityPoint = 0;
        IsPoisoned = false;
    }

    public void SetAgility(float agility)
    {
        AgilityPoint += agility;
    }

    #endregion
    
}
