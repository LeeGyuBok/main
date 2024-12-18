using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FriendlyOperator : OperatorBattleStatus, ILevel, ITrustable
{
    private bool levelInitialzied = false;
    
    #region RelationshipInfo

    [SerializeField] private TrustableData baseTrustableData;
    public TrustableData TrustData => baseTrustableData;
    public int CurrentTrust { get; protected set; }
    
    public void AddTrust(int amount)
    {
        CurrentTrust += amount;
    }
    public void RemoveTrust(int amount)
    {
        CurrentTrust -= amount;
    }
    public void SetInitialTrustData()
    {
        CurrentTrust = TrustData.BaseTrust;
    }
    public void SetJsonTrustData(int jsonTrust)
    {
        CurrentTrust = jsonTrust;
    }
    #endregion

    #region LevelInfo

    private readonly int statusCoefficientByLevel = 7;
    
    /// <summary>
    /// 0 == Health, 1 == Attack, 2 == Defense, 3 == Speed
    /// </summary>
    public List<int> statusCoefficientsByLevel = new List<int>();
    public int Level { get; protected set; }
    
    public int CurrentExperiencePoints { get; protected set; }
    
    public int RequireExperiencePoints { get; protected set; }
    
    public Dictionary<int, int> LevelUpExperience { get; protected set; }

    public void SetExperience(int experience)
    {
        if (!levelInitialzied)
        {
            Debug.Log("Not Initializing Level");
            return;
        }
        //현재 경험치값복사
        int currentExperience = CurrentExperiencePoints;
        //획득한 경험치 합
        currentExperience += experience;
        //획득한 경험치 합과 요구 경험치량의 대소비교
        if (currentExperience >= RequireExperiencePoints)
        {
            //크거나 같으면 여분의 경험치 확인
            int lestExperience = currentExperience - RequireExperiencePoints;
            int nextLevel = 0;
            //여분의 경험치가 0보다크면
            if (lestExperience > 0)
            {
                //현재 레벨을 기점으로 반복문 시작
                for (int i = Level+1; i < LevelUpExperience.Count; i++)
                {
                    //여분의 경험치가 현재 레벨에서의 경험치량을 초과하면?
                    if (lestExperience - LevelUpExperience[i] >= 0)
                    {
                        lestExperience -= LevelUpExperience[i];
                        continue;
                    }
                    else
                    {
                        nextLevel = i;
                        break;
                    }
                }
            }
            
            Level = nextLevel;
            RequireExperiencePoints = nextLevel < LevelUpExperience.Count 
                ? LevelUpExperience[nextLevel] 
                : RequireExperiencePoints; // 리스트 범위 초과 방지
            //RequireExperiencePoints = LevelUpExperience[Level];    

            for (int i = 0; i < statusCoefficientsByLevel.Count; i++)
            {
                statusCoefficientsByLevel[i] *= Level;
            }
            
            CurrentExperiencePoints = lestExperience;
        }
        else
        {
            CurrentExperiencePoints = currentExperience;
        }
        
    }

    //일단 살려는 둔다.
    public void LevelUp()
    {
        if (!levelInitialzied)
        {
            Debug.Log("Not Initializing Level");
            return;
        }
        Level++;
        if (LevelUpExperience.TryGetValue(Level, out int requireExperiencePoints))
        {
            RequireExperiencePoints = LevelUpExperience[Level];    
        }
        else
        {
            RequireExperiencePoints = int.MaxValue;
        }
        
        CurrentExperiencePoints = 0;
    }

    /// <summary>
    /// initializing
    /// </summary>
    public void SetDictionaryAndInitialize()
    {
        if (!levelInitialzied)
        {
            LevelUpExperience = new Dictionary<int, int>(ExpPerLevel.expPerLevel);
            Level = 1;
            CurrentExperiencePoints = 0;
            RequireExperiencePoints = LevelUpExperience[Level];
            for (int i = 0; i < 4; i++)
            {
                statusCoefficientsByLevel.Add(statusCoefficientByLevel * Level);    
            }
            
        }
        levelInitialzied = true;
    }

    #endregion
    
    #region CombatInfo

    public int HealthPointCoefficientByLevel { get; private set; }
    public int AttackPointCoefficientByLevel { get; private set; }
    public int DefensePointCoefficientByLevel { get; private set; }
    public int SpeedPointCoefficientByLevel { get; private set; }
    
    public override void SetBaseStatus(CharacterOverrideData data = default)
    {
        
        //dna코이피션트 적용
        if (data == default)
        {
            base.SetBaseStatus(default);    
        }
        else
        {
            base.SetBaseStatus(data);
        }
        
        //최초 데이터 설정
        //이즈이즈지스트제이슨파일이 항상 false가 나왔나본데?
        
        /*if (Dna.GeneList.Count < 1)
        {
            if (!JSONWriter.Instance.IsExistJsonFile())
            {
                Dna.TryInsertGene(new GeneType(GeneType, true));
                Debug.Log("No gene in list, and no json file");
            }
            
        }*/
    }

    #endregion
   

    #region FriendlyCombatState
    
    public float moveSpeed { get; private set; } = 8f;
    
    //이 스테이트들은 아군적군으로 구분시켜야함.
    public FriendlyMoveState MoveState { get; protected set; }
    
    public FriendlyAssaultState AssaultState{ get; protected set; }
    public FriendlyAttackState AttackState{ get; protected set; }
    
    public FriendlySearchState SearchState{ get; protected set; }
    public FriendlyTakeCoverState TakeCoverState{ get; protected set; }
    public FriendlyAreaHoldState AreaHoldState{ get; protected set; }
    public FriendlyDeadState DeadState{ get; protected set; }
    

    public int EnemyLayerMask { get; private set; }//아군용
    
    public float Sight { get; } = 6f;//아군만
    
    public float SightAngle { get; } = 170f;
    
    //기즈모용
    private Collider[] hitColliders = new Collider[10];
    
    public SignPoint LastSignPoint{ get; protected set; }

    public void SetLastSignPoint(SignPoint sign)
    {
        LastSignPoint = sign;
    }
    public SignPoint CurrentSignPoint{ get; protected set; }
    
    public void SetCurrentSignPoint(SignPoint sign)
    {
        CurrentSignPoint = sign;
    }
    
    protected override void Awake()
    {
        base.Awake();
        MoveState = new FriendlyMoveState(this);
        AssaultState = new FriendlyAssaultState(this);
        AttackState = new FriendlyAttackState(this);
        TakeCoverState = new FriendlyTakeCoverState(this);
        AreaHoldState = new FriendlyAreaHoldState(this);
        DeadState = new FriendlyDeadState(this);
        SearchState = new FriendlySearchState(this);
    }
    
    protected override void Start()
    {
        currentState = MoveState;
        // 처음 상태는 이동 상태로 설정
        Debug.Log("Friendly Start");
        EnemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");//멤버변수로 못들고있음 ㅋㅋ
        
        CombatDetectingCapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        if (CombatDetectingCapsuleCollider.isTrigger)
        {
            CombatDetectingCapsuleCollider.isTrigger = false;
        }
        if (CombatDetectingCapsuleCollider.enabled)
        {
            CombatDetectingCapsuleCollider.enabled = false;
        }
        
        CharacterRigidbody = gameObject.GetComponent<Rigidbody>();
        if (CharacterRigidbody.constraints != RigidbodyConstraints.FreezeRotation)
        {
            CharacterRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")));
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))//아 어렵네
        {
            /*TargetEnemy = other.gameObject.GetComponent<EnemyOperator>();*/
            NowAttachToTarget = true;
            Debug.Log(NowAttachToTarget);
            //Debug.Log(other.gameObject.name);
        }
    }

    public bool GetAttachToTarget()
    {
        return NowAttachToTarget;
    }
    
    void OnDrawGizmos()
    {
        Physics.OverlapSphereNonAlloc(CharacterRigidbody.position, Sight, hitColliders, EnemyLayerMask);
            
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(CharacterRigidbody.position, Sight);

        Vector3 rightLimit = Quaternion.Euler(0, SightAngle / 2, 0) * transform.forward;
        Vector3 leftLimit = Quaternion.Euler(0, -SightAngle / 2, 0) * transform.forward;
        Gizmos.DrawLine(CharacterRigidbody.position, CharacterRigidbody.position + rightLimit * Sight);
        Gizmos.DrawLine(CharacterRigidbody.position, CharacterRigidbody.position + leftLimit * Sight);
    }
    #endregion
    
}
