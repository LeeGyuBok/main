using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneNode
{
    public Scenario currentScenario { get; private set; }
    public Button QuestionButton { get; private set; }
}


public class UISide_ReceiveAnswerUiManager : MortalManager<UISide_ReceiveAnswerUiManager>
{
    //캐릭터 대사위치, 스킵 위치, 지금까지의 대사로그 위치, 자동 재생 위치, 선택지 출력 위치, 선택지에 나타날 버튼 프리팹.
    [SerializeField] private TextMeshProUGUI characterSentence;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button showDialogButton;
    [SerializeField] private Transform dialogParent;
    [SerializeField] private Button autoButton;
    [SerializeField] private Transform selectButtonParent;
    [SerializeField] private GameObject selectButtonPrefab;
    
    
    private ScenarioBook scenarioBook;
    
    private Dictionary<string, Scenario> showingScenarios = new Dictionary<string, Scenario>();
    
    public bool IsAuto { get; private set; }
    
    public bool IsSeparatedByTrust { get; private set; }

    private void Start()
    {
        scenarioBook = UISide_ImmortalGameManager.Instance.selectedScenarioBook;
        //scenarioBook = UISide_ImmortalJsonManager.Instance.ReadScenarioData("Aggre").ScenarioBooks[1];//테스트용코드
        ScenarioParsing();
        StartConversation();

    }

    public void Skip()
    {
        
    }

    public void SwitchAuto()
    {
        if (IsAuto)
        {
            //오토표시 애니메이션 비활성화
            IsAuto = false;
            return;
        }
        //오토표시 애니메이션, 여기도 메테리얼로 쉐이더 해야겠다.
        IsAuto = true;
    }
    
    private void ScenarioParsing()
    {
        if (scenarioBook == null) return;
        
        if (scenarioBook.Scenarios == null) return;

        if (showingScenarios == null)
        {
            showingScenarios = new Dictionary<string, Scenario>();
        }
        
        
        for (int i = 0; i < scenarioBook.Scenarios.Count; i++)
        {
            showingScenarios.Add(scenarioBook.Scenarios[i].SceneName, scenarioBook.Scenarios[i]);    
        }
        //쇼잉시나리오는 시나리오의 이름을 키로, 시나리오값을 갖는다.
        //시나리오는 씬네임, 해당씬을 보기위한 신뢰도제한, 해당 씬에서 npc가 말할 대사, 가능한 다음 씬들의이름을 들고있다. 다른 씬들의 이름을 열쇠로 다음대화를 진행한다.
    }

    //버튼 생성, 리스너 연결
    public void StartConversation()
    {
        Button firstButton = Instantiate(selectButtonPrefab, selectButtonParent).GetComponent<Button>();
        string firstText = scenarioBook.Scenarios[0].SceneName;
        firstButton.GetComponentInChildren<TextMeshProUGUI>().text = firstText;
        firstButton.onClick.AddListener(() => PrintOutSentence(showingScenarios[firstText]));
        selectButtonParent.gameObject.SetActive(true);
    }
    
    //생성된 버튼 클릭시 해당 시나리오의 텍스트들 출력
    private void PrintOutSentence(Scenario scenario)
    {
        StartCoroutine(ShowSentence(scenario));
        for (int i = 0; i < selectButtonParent.childCount; i++)
        {
            GameObject buttonObject = selectButtonParent.GetChild(i).gameObject; 
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            Destroy(buttonObject);
        }
        
        selectButtonParent.gameObject.SetActive(false);
    }
    
    private IEnumerator ShowSentence(Scenario scenario)
    {
        List<string> sentences = new List<string>(scenario.Sentences);
            
        int sentencesCount = sentences.Count;
        for (int i = 0; i < sentencesCount; i++)
        {
            if (IsAuto)
            {
                yield return PrintStringCoroutineNeedAnyInput(sentences[i]);
                yield return WaitForMouseClick();
            }
            else
            {
                yield return PrintStringCoroutine(sentences[i]);
                yield return new WaitForSeconds(1f); // 문장 끝 대기    
            }
        }
        //yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0.5f);//테스트용
        SetButtonOrContinue(scenario);
        //SetSelectButton(scenario.NextSceneNames);
    }

    private void SetButtonOrContinue(Scenario scenario)
    {
        //1. 신뢰도 제한이 0인 시나리오, 0이 아닌 시나리오
        //2. 브랜치가 트루인 시나리오, false인 시나리오
        
        //분기시나리오인가?
        if (scenario.IsBranch)
        {
            //신뢰도 시나리오인가?
            if (scenario.TrustLimit != 0)
            {
                //현재 신뢰도가져오기
                int characterTrustLimit = UISide_ImmortalGameManager.Instance.OperatorCurrentTrust;
                //다음씬 이름 받을 준비
                string trustSceneName = "";
                string noneTrustSceneName = "";
                for (int i = 0; i < scenario.NextSceneNames.Count; i++)
                {
                    //다음 시나리오들의 신뢰도를 가져오기
                    int trustLimit = showingScenarios[scenario.NextSceneNames[i]].TrustLimit;
                
                    //신뢰도 수치 충족
                    if (trustLimit != 0 &&trustLimit <= characterTrustLimit)
                    {
                        trustSceneName = scenario.NextSceneNames[i];
                        continue;
                    }
                    noneTrustSceneName = scenario.NextSceneNames[i];
                }

                if (trustSceneName != "")
                {
                    StartCoroutine(ShowSentence(showingScenarios[trustSceneName]));
                }
                else
                {
                    StartCoroutine(ShowSentence(showingScenarios[noneTrustSceneName]));
                }
            }
            else
            {
                SetSelectButton(scenario.NextSceneNames);    
            }
        }
        else
        {
            SetSelectButton(scenario.NextSceneNames);
        }
    }

    //선택지를 인자로받음
    public void SetSelectButton(List<string> possibleNextScenarioNames)
    {
        int createButtonCount = possibleNextScenarioNames.Count;
        if (createButtonCount == 0)
        {
            if (UISide_ImmortalGameManager.Instance.SelectedPlayMode.Equals(PlayMode.CardGame))
            {
                UISide_ImmortalJsonManager.Instance.SaveRenewCharacterScenarioTrustData(UISide_ImmortalGameManager.Instance.selectedScenarioJsonData.CharacterName, 3);    
            }
            UISide_ImmortalSceneManager.Instance.LoadMainScene();
            return;
        }
        
        for (int i = 0; i < createButtonCount; i++)
        {
            Instantiate(selectButtonPrefab, selectButtonParent);
            Debug.Log(possibleNextScenarioNames[i]);
        }
        Debug.Log(selectButtonParent.childCount);
        for (int i = 0; i < selectButtonParent.childCount; i++)
        {
            Button button = selectButtonParent.GetChild(i).GetComponent<Button>();
            if (i < possibleNextScenarioNames.Count)
            {
                string text = possibleNextScenarioNames[i];
                button.GetComponentInChildren<TextMeshProUGUI>().text = text;   
                button.onClick.AddListener(() => PrintOutSentence(showingScenarios[text]));
            }
            
        }
        selectButtonParent.gameObject.SetActive(true);
        
    }

    private IEnumerator ShowDialog()
    {
        foreach (var keyValuePair in showingScenarios)
        {
            List<string> sentences = new List<string>(showingScenarios[keyValuePair.Key].Sentences);
            
            int sentencesCount = sentences.Count;
            for (int i = 0; i < sentencesCount; i++)
            {
                if (IsAuto)
                {
                    yield return PrintStringCoroutineNeedAnyInput(sentences[i]);
                    yield return WaitForMouseClick();
                }
                else
                {
                    yield return PrintStringCoroutine(sentences[i]);
                    yield return new WaitForSeconds(2f); // 문장 끝 대기    
                }
            }
        }
    }
    
    private IEnumerator PrintStringCoroutine(string sentence)
    {
        string text = sentence;
        string currentText = "";
        characterSentence.text = currentText;

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i]; // 문자열을 점점 늘려가며 추가
            characterSentence.text = currentText;
            //Debug.Log(currentText); // 출력
            //yield return new WaitForSeconds(0.1f); // 0.1초 대기
            yield return new WaitForSeconds(0.05f); // 0.05초대기, 테스트용
        }
    }
    
    private IEnumerator ShowDialogNeedAnyInput()
    {
        foreach (var keyValuePair in showingScenarios)
        {
            List<string> sentences = new List<string>(showingScenarios[keyValuePair.Key].Sentences);
            
            int sentencesCount = sentences.Count;
            for (int i = 0; i < sentencesCount; i++)
            {
                yield return PrintStringCoroutineNeedAnyInput(sentences[i]);
                yield return WaitForMouseClick();
            }
        }
    }
    
    private IEnumerator PrintStringCoroutineNeedAnyInput(string sentence)
    {
        string text = sentence;
        string currentText = "";
        characterSentence.text = currentText;
        
        for (int i = 0; i < text.Length; i++)
        {
            if (Input.anyKey)
            {
                characterSentence.text = text; // 전체 텍스트 출력
                yield break; // 코루틴 종료
            }
            yield return null;
            currentText += text[i]; // 문자열을 점점 늘려가며 추가
            characterSentence.text = currentText;
            //Debug.Log(currentText); // 출력
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
        
        }
        characterSentence.text = text;
    }
    
    private IEnumerator WaitForMouseClick()
    {
        // 마우스 왼쪽 버튼 클릭될 때까지 대기
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.4f); // 클릭 후 짧은 대기
    }
}
