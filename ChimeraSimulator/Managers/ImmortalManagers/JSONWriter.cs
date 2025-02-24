using System;
using System.Collections.Generic;
using System.IO; // 파일입출력기능
using UnityEngine;


public class CharacterOverrideData
{
    public string CharacterName;
    public int Trust;
    public List<int> GeneIDList;
    
    public CharacterOverrideData(string name, int currentTrust, List<int> geneIDList)
    {
        CharacterName = name;
        Trust = currentTrust;
        GeneIDList = geneIDList;
    }
}

public class GeneJsonData
{
    public int GeneType;
    public int GeneGrade;
    /// <summary>
    /// 0 == Health, 1 == Attack, 2 == Defense, 3 == Speed
    /// </summary>
    public List<int> StatusCoefficient;
    public int StabilizationDegree;
    public int GeneID;
    public int staticIndex;
    public bool IsActivation;

    public GeneJsonData(Gene gene)
    {
        //GeneType = (int)gene.GeneType;
        GeneGrade = (int)gene.GeneGrade;
        //StatusCoefficient = gene.RandomStatusCoefficient;
        StabilizationDegree = gene.StabilizationDegree;
        GeneID = gene.GeneID;
        staticIndex = Gene.DataId;
        IsActivation = gene.IsActivation;
    }
}

public class JSONWriter : ImmortalObject<JSONWriter>
{

    protected override void Awake()
    {
        base.Awake();
    }

    /*private void Start()
    {
        List<OperatorBattleStatus> battleStatusList = GameImmortalManager.Instance.CharacterPrefab;
        Dictionary<OperatorBattleStatus, FriendlyOperator> friendlyOperators = GameImmortalManager.Instance.FriendlyByBattleStatus;
        
        //두 문자열을 하나의 경로로 결합합니다. Path.Combine(string1, string2). 알겠는데, 왜 어떤건 폴더고 어떤건 파일이냐고. .json 때문인가? <- 맞다. .json때문이다... ㅋㅋ;
        string characterFolderPath = Path.Combine(Application.persistentDataPath, "Json");
        for (int i = 0; i < battleStatusList.Count; i++)
        {
            OperatorBattleStatus operatorBattleStatus = battleStatusList[i];
            FriendlyOperator operatorCharacter = friendlyOperators[operatorBattleStatus];
            string filePath = Path.Combine(characterFolderPath, $"{operatorCharacter.TrustData.CharacterName}.json");
            //이 캐릭터 이름으로 파일이 있으면
            if (IsExistJsonFile(filePath))
            {
                //Debug.Log("file exist");
                //리드프렌들리오퍼레이터데이터 자체가 내부에서 해당 캐릭터의 데이터파일을 검사한다.
                FriendlyOperator friendlyOperator = ReadFriendlyOperatorData(operatorBattleStatus);
                GameImmortalManager.Instance.SetFriendlyOperators(friendlyOperator);
            }
            else//없으면
            {
                operatorCharacter.SetInitialTrustData();
                //오후 10:54 2024-12-09
                //이놈이다. 여기서 셋하고, 리드에서 셋한다. -> 리드에서 셋하는것으로 통일해보자
                //셋팅을 먼저해야한다!
                operatorCharacter.SetBaseStatus(default);
                SaveFriendlyOperatorData(operatorCharacter);//<- 기본셋팅으로 그냥 해버리네
                FriendlyOperator friendlyOperator = ReadFriendlyOperatorData(operatorBattleStatus);
                GameImmortalManager.Instance.SetFriendlyOperators(friendlyOperator);

            }
        }
    }

    public void SaveFriendlyOperatorData(FriendlyOperator friendlyOperator)
    { 
        //인자 확인
        if (friendlyOperator == null)
        {
            Debug.Log("null operator or enemy operator");
            return;
        }

        //확인되면 정보 추출
        string characterName = friendlyOperator.TrustData.CharacterName;
        List<int> characterStatusCoefficient = new List<int>
        {
            friendlyOperator.Dna.TotalHealthCoefficient,
            friendlyOperator.Dna.TotalAttackCoefficient,
            friendlyOperator.Dna.TotalDefenceCoefficient,
            friendlyOperator.Dna.TotalAgilityCoefficient
        };
        
        List<int> geneIDs = new List<int>();
        List<Gene> geneList = new List<Gene>(friendlyOperator.Dna.GeneList);
        for (int i = 0; i < geneList.Count; i++)
        {
            geneIDs.Add(geneList[i].GeneID);
        }

        int currentTrust = friendlyOperator.CurrentTrust;
        
        //데이터로 래핑
        CharacterOverrideData data = new CharacterOverrideData(characterName, currentTrust, geneIDs);
        
        //string filepath = Path.Combine(CustomPath, $"{characterName}.json");//1경로를 기준으로 2경로에.. 아니다. 틀렸따.
        
        //폴더 경로 설정
        string folderPath = Path.Combine(Application.persistentDataPath, "Json");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"경로 생성 완료: {folderPath}");
        }
        
        //파일 경로 설정
        string filePath = Path.Combine(folderPath, $"{characterName}.json");
        
        //파일 내부 형식? 설정
        string jsonContent = JsonUtility.ToJson(data, true); // true = 보기 좋은 포맷(들여쓰기)
        
        //파일 생성
        File.WriteAllText(filePath, jsonContent); // 파일 저장. 파일 패스와 폴더패스가 따로있네
        //Debug.Log($"JSON 저장 완료: {filepath}/{characterName}.json");
    }

    private FriendlyOperator ReadFriendlyOperatorData(OperatorBattleStatus operatorBattleStatus)
    {
        FriendlyOperator friendlyOperator = operatorBattleStatus as FriendlyOperator;
        if (friendlyOperator == null)
        {
            Debug.Log("no friendly operator");
            return null;
        }
        //string filepath = Path.Combine(CustomPath, $"{friendlyOperator.TrustData.ScenarioName}.json");//1경로에 2이름으로 저장한다? 틀렸다
        string characterName = friendlyOperator.TrustData.CharacterName;
        string folderPath = Path.Combine(Application.persistentDataPath, "Json");
        string filePath = Path.Combine(folderPath, $"{characterName}.json");
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            
            CharacterOverrideData charactersData = JsonUtility.FromJson<CharacterOverrideData>(jsonContent);
          
            /*Debug.Log($"Name: {charactersData.ScenarioName},\n" +
                      $"GeneIDsCount: {charactersData.GeneIDList.Count}" + 
                      $"Trust: {charactersData.Trust}");#1#
            
            friendlyOperator.SetJsonTrustData(charactersData.Trust);
            friendlyOperator.SetBaseStatus(charactersData);
            
            //그럼 얘를 지워야되나?
            //friendlyOperator.SetBaseStatus();
            /*if (charactersData.StatusCoefficients == null)
            {
                Debug.LogError("StatusCoefficients is null!");
            }
            if (charactersData.OverExSkillCoefficients == null)
            {
                Debug.LogError("OverExSkillCoefficients is null!");
            }
            if (friendlyOperator == null)
            {
                Debug.LogError("friendlyOperator is null!");
            }
            if (friendlyOperator.Dna == null)
            {
                Debug.LogError("Dna is null!");
            }
            if (friendlyOperator.Dna.DnaSubSkill == null)
            {
                Debug.LogError("DnaSubSkill is null!");
            }#1#
            //Dna가 널이라고?
            return friendlyOperator;
        }
        return null;
    }*/

    public void SaveGeneData(Gene gene)
    {
        if (gene == null)
        {
            return;
        }
        //정보추출.. 할것도없네 이렇게하면?
        GeneJsonData jsonData = new GeneJsonData(gene);
        
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneJson");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"경로 생성 완료: {folderPath}");
        }
        
        //파일 경로 설정
        string filePath = Path.Combine(folderPath, $"{jsonData.GeneID}{(GeneType)jsonData.GeneType}.json");
        gene.SetFilePath(filePath);
        
        //파일 내부 형식? 설정
        string jsonContent = JsonUtility.ToJson(jsonData, true); // true = 보기 좋은 포맷(들여쓰기)
        
        //파일 생성
        File.WriteAllText(filePath, jsonContent); // 파일 저장. 파일 패스와 폴더패스가 따로있네
    }

    public List<Gene> ReadGeneData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneJson");
        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"폴더 내에 JSON 파일이 없습니다: {folderPath}");
            return null;
        }
        List<GeneJsonData> geneDataList = new List<GeneJsonData>();
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            try
            {
                // 파일 내용 읽기
                string jsonContent = File.ReadAllText(jsonFiles[i]);

                // JSON 데이터를 클래스 객체로 변환
                GeneJsonData jsonData = JsonUtility.FromJson<GeneJsonData>(jsonContent);

                if (jsonData != null)
                {
                    geneDataList.Add(jsonData);
                    //Debug.Log($"파일 읽기 성공: {jsonFiles[i]}");
                }
                else
                {
                    Debug.LogWarning($"JSON 데이터를 변환할 수 없습니다: {jsonFiles[i]}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        List<Gene> geneList = new List<Gene>();
        for (int i = 0; i < geneDataList.Count; i++)
        {
            //geneList.Add(new Gene(geneDataList[i]));
            Gene.SetDataIDbyJson(geneDataList[i].staticIndex);
        }
        return geneList;
    }

    public void SaveRenewalGeneData(Gene gene)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneJson");
        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"폴더 내에 JSON 파일이 없습니다: {folderPath}");
            return;
        }
        
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            // 파일 내용 읽기
            string jsonContent = File.ReadAllText(jsonFiles[i]);

            // JSON 데이터를 클래스 객체로 변환
            GeneJsonData jsonData = JsonUtility.FromJson<GeneJsonData>(jsonContent);

            if (jsonData != null)
            {
                if (gene.GeneID == jsonData.GeneID)
                {
                    GeneJsonData targetJsonData = new GeneJsonData(gene);
                    //파일 내부 형식? 설정
                    string targetJsonContent = JsonUtility.ToJson(targetJsonData, true); // true = 보기 좋은 포맷(들여쓰기)
        
                    //파일 생성
                    File.WriteAllText(jsonFiles[i], targetJsonContent);
                    //기존에 이미 존재했던 아이를 리뉴얼하는 것이기 때문에 스태틱아이디를 설정하지 않아도 될듯?
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"JSON 데이터를 변환할 수 없습니다: {jsonFiles[i]}");
            }
        }
    }

    public void DeleteGeneData(Gene gene)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneJson");
        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"폴더 내에 JSON 파일이 없습니다: {folderPath}");
            return;
        }
        
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            // 파일 내용 읽기
            string jsonContent = File.ReadAllText(jsonFiles[i]);

            // JSON 데이터를 클래스 객체로 변환
            GeneJsonData jsonData = JsonUtility.FromJson<GeneJsonData>(jsonContent);

            if (jsonData != null)
            {
                if (gene.GeneID == jsonData.GeneID)
                {
                    File.Delete(jsonFiles[i]);
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"JSON 데이터를 변환할 수 없습니다: {jsonFiles[i]}");
            }
        }
        
    }

    public bool IsExistJsonFile(string path = default)
    {
        return File.Exists(path);
    }
}