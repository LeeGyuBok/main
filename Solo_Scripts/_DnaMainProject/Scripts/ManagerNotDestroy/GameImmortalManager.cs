using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameImmortalManager : ImmortalObject<GameImmortalManager>, IObserver
{
    [SerializeField] private List<TrustableData> commanderData;
    
    //아군 오퍼레이터들이 이곳에 할당될 예정이다.
    [SerializeField] private List<OperatorBattleStatus> characterPrefab;
    public List<OperatorBattleStatus> CharacterPrefab => characterPrefab;

    private Queue<string /*나중에 새로운 타입을 정의할 예정*/> interviewData { get; set; }
    private List<CommanderCharacter> commanderCharacters { get; set; }
    
    public OperatorBattleStatus selectedOperator { get; private set; }
    public Dictionary<OperatorBattleStatus, FriendlyOperator> FriendlyByBattleStatus { get; private set; }
    
    public List<FriendlyOperator> FriendlyOperators { get; private set;}
    
    public List<Gene> Genes { get; private set; } = new List<Gene>();

    protected override void Awake() 
    {
        base.Awake();
        
        //커맨더 캐릭터 초기화
        if (commanderCharacters == null)
        {
            commanderCharacters = new List<CommanderCharacter>();
            FriendlyByBattleStatus = new Dictionary<OperatorBattleStatus, FriendlyOperator>();
            FriendlyOperators = new List<FriendlyOperator>();
            
            for (int i = 0; i < characterPrefab.Count; i++)
            {
                FriendlyByBattleStatus[characterPrefab[i]] = characterPrefab[i] as FriendlyOperator;
            }
            
            for (int i = 0; i < commanderData.Count; i++)
            {
                commanderCharacters.Add(new CommanderCharacter(commanderData[i]));
            }
            interviewData = new Queue<string>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        /*GameObject test = Instantiate(characterPrefab[0].gameObject, transform.position, Quaternion.identity);
        Debug.Log(test.GetComponent<OperatorBattleStatus>().Role.ToString());*/
        Genes = JSONWriter.Instance.ReadGeneData();
        /*if (Genes != null)
        {
            for (int i = 0; i < Genes.Count; i++)
            {
                Debug.Log(Genes[i].GeneType.ToString());
            }    
        }*/
        SceneImmortalManager.Instance.RegisterObserver(Instance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region GeneDataWrapping

    public void AddGene(Gene gene)
    {
        Genes ??= new List<Gene>();
        Genes.Add(gene);
    }

    public void RemoveGene(Gene gene)
    {
        if (Genes.Contains(gene))
        {
            Genes.Remove(gene);    
        }
    }

    public void RenewGenes()
    {
        List<Gene> newGenes = new List<Gene>(JSONWriter.Instance.ReadGeneData());
        Genes = newGenes;
    }

    #endregion

    #region InterviewDataWrapping

    /// <summary>
    /// random Commander and Players ActivationLog
    /// </summary>
    /// <returns></returns>
    public (CommanderCharacter, Queue<string>) GetInterviewData()
    {
        int randInt = Random.Range(0, commanderData.Count);
        CommanderCharacter selectedCommander = commanderCharacters[randInt];
        return (selectedCommander, interviewData);
    }
    
        
    public void DataRenew(string data)
    {
        if (interviewData == null)
        {
            Debug.Log($"Null?? = {interviewData is null}");
            return;
        }
        
        if (interviewData.Count <= 3)
        {
            if (interviewData.Count == 3)
            {
                interviewData.Dequeue();
            }
            interviewData.Enqueue(data);
        }
    }
    
    //이게 필요하긴한데 어떻게하지? > 그냥 메인으로 보내버려야겠다
    public void ClearInterviewData()
    {
        interviewData.Clear();
        //Debug.Log(interviewData.Count);
    }

    #endregion

    #region OperatorDataWrapping

    /// <summary>
    /// return Original List's Copy
    /// </summary>
    /// <returns></returns>
    public List<OperatorBattleStatus> GetOperatorDataList()
    {
        List<OperatorBattleStatus> list = new List<OperatorBattleStatus>(characterPrefab);
        return list;
    }

    public void SetOperatorData(OperatorBattleStatus character)
    {
        selectedOperator = FriendlyByBattleStatus[character];
    }

    public FriendlyOperator GetOperatorData()
    {
        if (selectedOperator is FriendlyOperator friendlyOperator)
        {
            if (JSONWriter.Instance.IsExistJsonFile())
            {
                
            }
            /*friendlyOperator.SetBaseStatus();
            friendlyOperator.SetInitialTrustData();*/
            return friendlyOperator;    
        }
        Debug.LogError("selectedOperator is not FriendlyOperator");
        return null;
    }
 
    public void SetFriendlyOperators(FriendlyOperator friendlyOperator)
    {
        FriendlyOperators.Add(friendlyOperator);
    }

    #endregion

    #region CampaignDataWrapping

    public GameObject campaignMapObject { get; private set; }
    public List<FriendlyOperator> selectedOperators { get; private set; } = new List<FriendlyOperator>();

    public void SetCampaignMapObject(GameObject mapObject)
    {
        campaignMapObject = mapObject;
    }

    public void SetSelectedOperators(List<FriendlyOperator> operators)
    {
        selectedOperators = operators;
    }

    #endregion
    
    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // PlayMode 종료
#else
                Application.Quit(); // 빌드된 환경에서 게임 종료
#endif
    }

    
}
