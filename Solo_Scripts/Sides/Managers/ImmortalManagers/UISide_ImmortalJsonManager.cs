using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization; // 파일입출력기능


[Serializable]public class ScenarioJson
{
    //질문코드에 따른 시나리오북 -> 딕셔너리로 생성이 안되네
    //리스트내의 인덱스에 따라서 노드 생성
    public int CurrentTrust = 0;
    public string CharacterName;
    public List<ScenarioBook> ScenarioBooks = new  List<ScenarioBook>();
}

[Serializable]public class ScenarioBook
{
    //노드번호, 해당 노드의 시나리오 -> 딕셔너리로 생성이 안되네
    public string ScenarioName;
    public List<Scenario> Scenarios = new List<Scenario>();
}

[Serializable]public class Scenario
{
    //현재 시나리오 이름
    public string SceneName;
    //현재 시나리오의 센텐스를 결정할 수 있는 신뢰도 수치
    public int TrustLimit;
    //이게 분기인지 아닌지? 선택지가 나뉠 때 사용함.
    public bool IsBranch;
    //이 시나리오의 대사들
    public List<string> Sentences;
    //현재 시나리오에서 갈 수 있는 다음 시나리오들. 시나리오들의 개 수 만큼 버튼 생성
    public List<string> NextSceneNames;
}

public class UISide_ImmortalJsonManager : ImmortalObject<UISide_ImmortalJsonManager>
{
    public string SelectedCharacterName { get; private set; }
    public ScenarioJson SelectedScenarioJson { get; private set; }
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        for (int i = 0; i < UISide_ImmortalGameManager.Instance.CharacterPrefab.Count; i++)
        {
            string characterName = UISide_ImmortalGameManager.Instance.CharacterPrefab[i].TrustData.CharacterName;
            string characterJsonFilePath = Path.Combine(folderPath, $"{characterName}_ScenarioData.json");
            if (IsExistJsonFile(characterJsonFilePath))
            {
                Debug.Log(ReadScenarioData(characterName).ScenarioBooks.Count);
                /*if (characterName.Equals("Aggre"))
                {
                    SelectedScenarioJson = ReadScenarioData(characterName);
                    SaveRenewCharacterScenarioTrustData(characterName, 5);
                }*/
                continue;
            }
            SaveSampleScenarioData(characterName);
        }

     
        
    }

    public void SetSelectedCharacterName(FriendlyOperator character)
    {
        SelectedCharacterName = character.TrustData.CharacterName; 
        SelectedScenarioJson = ReadScenarioData(SelectedCharacterName);
    }

    public void SaveSampleScenarioData(string characterName)
    {
        Scenario sample = new Scenario
        {
            SceneName = "sample",
            Sentences = new List<string>()
            {
                "sample","sample2","sample3","sample4","sample5","sample6",
            },
            NextSceneNames = new List<string>()
            {
                "sample2"
            },
            TrustLimit = 0,
            IsBranch = false
        };
        
        Scenario sample2 = new Scenario
        {
            SceneName = "sample2",
            Sentences = new List<string>()
            {
                "sample2","sample22","sample23","sample24","sample25","sample26",
            },
            NextSceneNames = new List<string>(){"sample", },
            TrustLimit = 10,
            IsBranch = true
        };
        
        Scenario sample3 = new Scenario
        {
            SceneName = "sample32",
            Sentences = new List<string>()
            {
                "sample32","sample232","sample323","sample324","sample325","sample326",
            },
            NextSceneNames = new List<string>(){"sample", "sample2"},
            TrustLimit = 0,
            IsBranch = false
        };
            
        ScenarioBook sampleScenarioBook = new ScenarioBook();
        
        sampleScenarioBook.Scenarios.Add(sample);
        sampleScenarioBook.Scenarios.Add(sample2);
        sampleScenarioBook.Scenarios.Add(sample3);
        sampleScenarioBook.ScenarioName = "SampleScenario";
        
        Scenario sample4 = new Scenario
        {
            SceneName = "sample",
            Sentences = new List<string>()
            {
                "sample","sample2","sample3","sample4","sample5","sample6",
            },
            NextSceneNames = new List<string>()
            {
                "sample2"
            },
            TrustLimit = 0,
            IsBranch = false
        };
        
        Scenario sample41 = new Scenario
        {
            SceneName = "sample2",
            Sentences = new List<string>()
            {
                "sample2","sample22","sample23","sample24","sample25","sample26",
            },
            NextSceneNames = new List<string>(){"sample", },
            TrustLimit = 10,
            IsBranch = true
        };
        
        Scenario sample42 = new Scenario
        {
            SceneName = "sample32",
            Sentences = new List<string>()
            {
                "sample32","sample232","sample323","sample324","sample325","sample326",
            },
            NextSceneNames = new List<string>(){"sample", "sample2"},
            TrustLimit = 0,
            IsBranch = false
        };
            
        ScenarioBook sampleScenarioBook2 = new ScenarioBook();
        
        sampleScenarioBook2.Scenarios.Add(sample4);
        sampleScenarioBook2.Scenarios.Add(sample41);
        sampleScenarioBook2.Scenarios.Add(sample42);
        sampleScenarioBook2.ScenarioName = "SampleScenario";
        
        
        
        //완성된 시나리오북을 제이슨파일로 변환할 준비하기
        ScenarioJson sampleScenarioJson = new ScenarioJson
        {
            CharacterName = characterName,
            ScenarioBooks = new List<ScenarioBook>
            {
                sampleScenarioBook,
                sampleScenarioBook2
            }
        };


        //폴더 경로 설정
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"경로 생성 완료: {folderPath}");
        }
        
        //파일 경로 설정
        string filePath = Path.Combine(folderPath, $"{characterName}_ScenarioData.json");
        
        //파일 내부 형식? 설정
        string jsonContent = JsonUtility.ToJson(sampleScenarioJson, true); // true = 보기 좋은 포맷(들여쓰기)

        //파일 생성
        File.WriteAllText(filePath, jsonContent); // 파일 저장. 파일 패스와 폴더패스가 따로있네
        //Debug.Log($"JSON 저장 완료: {filepath}/{characterName}.json");
    }

    public ScenarioJson ReadScenarioData(string characterName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        string characterJsonFilePath = Path.Combine(folderPath, $"{characterName}_ScenarioData.json");
        if (File.Exists(characterJsonFilePath))
        {
            string jsonContent = File.ReadAllText(characterJsonFilePath);    
            ScenarioJson characterScenarioData = JsonUtility.FromJson<ScenarioJson>(jsonContent);
            return characterScenarioData;
        }

        return null;
    }
    
    public bool IsExistJsonFile(string path = default)
    {
        return File.Exists(path);
    }

    public void SaveRenewCharacterScenarioTrustData(string characterName, int trust)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"폴더 내에 JSON 파일이 없습니다: {folderPath}");
            return;
        }
        
        
        string characterJsonFilePath = Path.Combine(folderPath, $"{characterName}_ScenarioData.json");

        for (int i = 0; i < jsonFiles.Length; i++)
        {
            string jsonContent = File.ReadAllText(jsonFiles[i]);
            ScenarioJson characterScenarioData = JsonUtility.FromJson<ScenarioJson>(jsonContent);
            if (SelectedScenarioJson.CharacterName != null && SelectedScenarioJson.CharacterName.Equals(characterName))
            {
                characterScenarioData.CurrentTrust += trust;
                SelectedScenarioJson.CurrentTrust = characterScenarioData.CurrentTrust;
                
                string jsonString = JsonUtility.ToJson(characterScenarioData, true);
                Debug.Log(characterScenarioData.CurrentTrust);
                File.WriteAllText(characterJsonFilePath, jsonString);
                return;
            }
        }
        Debug.Log("error in jsonFile renew");

    }
    
    
}
