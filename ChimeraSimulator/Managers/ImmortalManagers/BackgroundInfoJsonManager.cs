using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public enum BackGroundInfoType
{
    ByResearcherRank,
    ByTimeLine
}

[Serializable]public class ScenarioJson
{
    public List<Scenario> ScenarioBooks;
}

[Serializable]public class TimeLineScenario : ScenarioJson
{
    public int NeedTimeLineDate;
    public TimeLineScenario(int needTimeLineDate)
    {
        NeedTimeLineDate = needTimeLineDate;
        ScenarioBooks = new List<Scenario>();
    }
}

[Serializable]public class ResearcherRankScenario : ScenarioJson
{
    //질문코드에 따른 시나리오북 -> 딕셔너리로 생성이 안되네
    //이 데이터가 시간대 스토리인지? 직급 스토리인지? 0 -> 직급, 1 -> 시간대
    public int NeedResearcherRankInt;
    public int SortOrder;
    private static int Index;
    
    public ResearcherRankScenario(int needResearcherRankInt)
    {
        NeedResearcherRankInt = needResearcherRankInt;
        Index++;
        SortOrder = Index;
        ScenarioBooks = new List<Scenario>();
    }
}

[Serializable]public class Scenario
{
    //현재 시나리오 이름
    public string SceneName;
    //보여줄 이미지
    public string ImagePath;
    //이게 분기인지 아닌지? 선택지가 나뉠 때 사용함.
    public bool IsBranch;
    //이 시나리오의 대사들
    public List<string> Sentences;
    //현재 시나리오에서 갈 수 있는 다음 시나리오들. 시나리오들의 개 수 만큼 버튼 생성
    public List<string> NextSceneNames;
}


public class BackgroundInfoJsonManager : ImmortalObject<BackgroundInfoJsonManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    public List<ScenarioJson> ReadAllScenarioData()
    {
        // "ScenarioJson" 폴더에서 모든 JSON 파일 불러오기
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Scenario");

        if (jsonFiles == null || jsonFiles.Length == 0)
        {
            //Debug.Log("No file");
            return null;
        }

        List<ScenarioJson> scenarios = new List<ScenarioJson>();

        foreach (TextAsset jsonFile in jsonFiles)
        {
            string jsonContent = jsonFile.text;

            if (jsonFile.name.Contains(BackGroundInfoType.ByResearcherRank.ToString()))
            {
                ScenarioJson scenarioData = JsonUtility.FromJson<ResearcherRankScenario>(jsonContent);
                scenarios.Add(scenarioData);
            }
            else if (jsonFile.name.Contains(BackGroundInfoType.ByTimeLine.ToString()))
            {
                ScenarioJson scenarioData = JsonUtility.FromJson<TimeLineScenario>(jsonContent);
                scenarios.Add(scenarioData);
            }
        }

        return scenarios;
    }
    
    public bool IsExistJsonFile(string path = default)
    {
        return File.Exists(path);
    }
    
    /*//샘플 파일이 필요한 경우 실행
    private void Start()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");

        for (int i = 0; i < (int)300/7; i++)
        {
            string filePath = Path.Combine(folderPath, $"{BackGroundInfoType.ByTimeLine.ToString()}_{i}_ScenarioData.json");
            if (IsExistJsonFile(filePath))
            {
                Debug.Log(ReadTimelineScenarioData(i).ScenarioBooks.Count);
                //GameImmortalManager.Instance.SetBackgroundInfoData(ReadTimelineScenarioData(i));
                continue;
            }

            TestTimeLineScenario(i);
            //GameImmortalManager.Instance.SetBackgroundInfoData(ReadTimelineScenarioData(i));
        }


        for (int i = 0; i < 4; i++)
        {
            string filePath = Path.Combine(folderPath, $"{BackGroundInfoType.ByResearcherRank.ToString()}_{i+1}_{i}_ScenarioData.json");
            if (IsExistJsonFile(filePath))
            {
                Debug.Log(ReadResearcherScenarioData(i).ScenarioBooks.Count);
                //GameImmortalManager.Instance.SetBackgroundInfoData(ReadResearcherScenarioData(i));
                continue;
            }

            TestResearcherRankScenario(i);
            //GameImmortalManager.Instance.SetBackgroundInfoData(ReadResearcherScenarioData(i));
        }
    }
  private void TestTimeLineScenario(int needTimeLineDate)
    {
        Scenario sample = new Scenario
        {
            SceneName = "sampleScene1",
            Sentences = new List<string>()
            {
                "sample","sample2","sample3","sample4","sample5","sample6",
            },
            NextSceneNames = new List<string>()
            {
                "sampleScene2"
            },
            IsBranch = false
        };
        
        Scenario sample2 = new Scenario
        {
            SceneName = "sampleScene2",
            Sentences = new List<string>()
            {
                "sample2","sample22","sample23","sample24","sample25","sample26",
            },
            NextSceneNames = new List<string>(){"sampleScene1", "sampleScene3" },
            IsBranch = true
        };
        
        Scenario sample3 = new Scenario
        {
            SceneName = "sampleScene3",
            Sentences = new List<string>()
            {
                "sample32","sample232","sample323","sample324","sample325","sample326",
            },
            NextSceneNames = new List<string>(){},
            IsBranch = false
        };
        
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
            IsBranch = false
        };
        
        
        
        //완성된 시나리오북을 제이슨파일로 변환할 준비하기
        ScenarioJson sampleScenarioJson = new TimeLineScenario(needTimeLineDate);;
        sampleScenarioJson.ScenarioBooks.Add(sample);
        sampleScenarioJson.ScenarioBooks.Add(sample2);
        sampleScenarioJson.ScenarioBooks.Add(sample3);
        
        //폴더 경로 설정
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"경로 생성 완료: {folderPath}");
        }
        
        string filePath = Path.Combine(folderPath, $"{BackGroundInfoType.ByTimeLine.ToString()}_{needTimeLineDate}_ScenarioData.json");
        
        //파일 내부 형식? 설정
        string jsonContent = JsonUtility.ToJson(sampleScenarioJson, true); // true = 보기 좋은 포맷(들여쓰기)

        //파일 생성
        File.WriteAllText(filePath, jsonContent); // 파일 저장. 파일 패스와 폴더패스가 따로있네
    }
    public ScenarioJson ReadTimelineScenarioData(int needTimeLineDate)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        string characterJsonFilePath = Path.Combine(folderPath, $"{BackGroundInfoType.ByTimeLine.ToString()}_{needTimeLineDate}_ScenarioData.json");
        if (File.Exists(characterJsonFilePath))
        {
            string jsonContent = File.ReadAllText(characterJsonFilePath);    
            ScenarioJson characterScenarioData = JsonUtility.FromJson<TimeLineScenario>(jsonContent);
            return characterScenarioData;
        }

        return null;
    }
    private void TestResearcherRankScenario(int needResearcherRankInt)
    {
        Scenario sample = new Scenario
        {
            SceneName = "sampleScene1",
            Sentences = new List<string>()
            {
                "sample","sample2","sample3","sample4","sample5","sample6",
            },
            NextSceneNames = new List<string>()
            {
                "sampleScene2"
            },
            IsBranch = false
        };
        
        Scenario sample2 = new Scenario
        {
            SceneName = "sampleScene2",
            Sentences = new List<string>()
            {
                "sample2","sample22","sample23","sample24","sample25","sample26",
            },
            NextSceneNames = new List<string>(){"sampleScene1", "sampleScene3" },
            IsBranch = true
        };
        
        Scenario sample3 = new Scenario
        {
            SceneName = "sampleScene3",
            Sentences = new List<string>()
            {
                "sample32","sample232","sample323","sample324","sample325","sample326",
            },
            NextSceneNames = new List<string>(){},
            IsBranch = false
        };
        
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
            IsBranch = false
        };
        
        //완성된 시나리오북을 제이슨파일로 변환할 준비하기
        ResearcherRankScenario sampleScenarioJson = new ResearcherRankScenario(needResearcherRankInt); 
        sampleScenarioJson.ScenarioBooks.Add(sample);
        sampleScenarioJson.ScenarioBooks.Add(sample2);
        sampleScenarioJson.ScenarioBooks.Add(sample3);
        

        //폴더 경로 설정
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"경로 생성 완료: {folderPath}");
        }
        string filePath = Path.Combine(folderPath, $"{BackGroundInfoType.ByResearcherRank.ToString()}_{needResearcherRankInt}_{sampleScenarioJson.SortOrder}_ScenarioData.json");
        
        //파일 내부 형식? 설정
        string jsonContent = JsonUtility.ToJson(sampleScenarioJson, true); // true = 보기 좋은 포맷(들여쓰기)

        //파일 생성
        File.WriteAllText(filePath, jsonContent); // 파일 저장. 파일 패스와 폴더패스가 따로있네
    }
    public ScenarioJson ReadResearcherScenarioData(int needResearcherRankInt)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        string dataPath = Path.Combine(folderPath, $"{BackGroundInfoType.ByResearcherRank.ToString()}_{needResearcherRankInt}_{needResearcherRankInt+1}_ScenarioData.json");
        if (File.Exists(dataPath))
        {
            string jsonContent = File.ReadAllText(dataPath);    
            ScenarioJson scenarioData = JsonUtility.FromJson<ResearcherRankScenario>(jsonContent);
            return scenarioData;
        }

        return null;
    }
    
    public void SaveSampleScenarioData(int backgroundInfoTypeInt, int dailyCount = 0)
    {
        Scenario sample = new Scenario
        {
            SceneName = "sampleScene1",
            Sentences = new List<string>()
            {
                "sample","sample2","sample3","sample4","sample5","sample6",
            },
            NextSceneNames = new List<string>()
            {
                "sampleScene2"
            },
            IsBranch = false
        };
        
        Scenario sample2 = new Scenario
        {
            SceneName = "sampleScene2",
            Sentences = new List<string>()
            {
                "sample2","sample22","sample23","sample24","sample25","sample26",
            },
            NextSceneNames = new List<string>(){"sampleScene1", "sampleScene3" },
            IsBranch = true
        };
        
        Scenario sample3 = new Scenario
        {
            SceneName = "sampleScene3",
            Sentences = new List<string>()
            {
                "sample32","sample232","sample323","sample324","sample325","sample326",
            },
            NextSceneNames = new List<string>(){},
            IsBranch = false
        };
        
        Scenario sample4 = new Scenario
        {
            SceneName = "sample",
            Sentences = new List<string>()
            {
                "sample","sample2","sample3","sample4","sample5","sample6",
            },
            NextSceneNames = new List<string>(){"sample", "sample2"},
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
            IsBranch = false
        };
        
        //완성된 시나리오북을 제이슨파일로 변환할 준비하기
        ScenarioJson sampleScenarioJson;

        //폴더 경로 설정
        string folderPath = Path.Combine(Application.persistentDataPath, "ScenarioJson");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"경로 생성 완료: {folderPath}");
        }

        string filePath = "";
        //파일 경로 설정
        switch (dailyCount)
        {
            case 0:
                filePath = Path.Combine(folderPath, $"{(BackGroundInfoType)backgroundInfoTypeInt}_ScenarioData.json");
                sampleScenarioJson = new ScenarioJson
                {
                    ScenarioBooks = new List<Scenario>
                    {
                        sample,
                        sample2,
                        sample3,
                    }
                };
                break;
            default:
                filePath = Path.Combine(folderPath, $"{(BackGroundInfoType)backgroundInfoTypeInt}_{dailyCount}_ScenarioData.json");
                sampleScenarioJson = new ScenarioJson
                {
                    ScenarioBooks = new List<Scenario>
                    {
                        sample,
                        sample2,
                        sample3,
                    }
                };
                break;
        }
        
        
        //파일 내부 형식? 설정
        string jsonContent = JsonUtility.ToJson(sampleScenarioJson, true); // true = 보기 좋은 포맷(들여쓰기)

        //파일 생성
        File.WriteAllText(filePath, jsonContent); // 파일 저장. 파일 패스와 폴더패스가 따로있네
        //Debug.Log($"JSON 저장 완료: {filepath}/{characterName}.json");
    }*/
    
    //코드의 무덤
    /*public void SaveRenewCharacterScenarioTrustData(string characterName, int trust)
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
            string jsonString = JsonUtility.ToJson(characterScenarioData, true);
            File.WriteAllText(characterJsonFilePath, jsonString);
            return;
        }
    }
    Debug.Log("error in jsonFile renew");

}*/
}
