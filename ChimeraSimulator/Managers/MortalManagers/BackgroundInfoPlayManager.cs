using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;


//from UiSide ReceiveAnswerManager

public class BackgroundInfoPlayManager : MortalManager<BackgroundInfoPlayManager>
{
//캐릭터 대사위치, 스킵 위치, 지금까지의 대사로그 위치, 자동 재생 위치, 선택지 출력 위치, 선택지에 나타날 버튼 프리팹.
    [SerializeField] private TextMeshProUGUI characterSentence;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button showDialogButton;
    [SerializeField] private Transform dialogParent;
    [SerializeField] private Button autoButton;
    [SerializeField] private GameObject scrollViewObject;
    [SerializeField] private Transform selectButtonParent;
    [SerializeField] private GameObject selectButtonPrefab;
    [SerializeField] private Image characterImage;
    
    
    private (BackGroundInfoDataScriptableObject, int) _selectedScenarioData;

    private Dictionary<string, Scenario> showingScenarios;

    private readonly WaitForSeconds _waitForSeconds = new (0.07f);

    public bool IsAuto { get; private set; } = true;

    private void Start()
    {
        _selectedScenarioData = GameImmortalManager.Instance.ScenarioDataObject;
        //_selectedScenarioData = UISide_ImmortalJsonManager.Instance.ReadAllScenarioData("Aggre").ScenarioBooks[1];//테스트용코드
        ScenarioParsing();
        StartConversation();
        characterImage.gameObject.SetActive(false);
    }
    
    private void ScenarioParsing()
    {
        if (_selectedScenarioData.Item1 == null) return;
        
        if (_selectedScenarioData.Item1.ScenarioBooks == null) return;

        if (showingScenarios == null)
        {
            showingScenarios = new Dictionary<string, Scenario>();
        }
        
        
        for (int i = 0; i < _selectedScenarioData.Item1.ScenarioBooks.Count; i++)
        {
            showingScenarios.Add(_selectedScenarioData.Item1.ScenarioBooks[i].SceneName, _selectedScenarioData.Item1.ScenarioBooks[i]);    
        }
        //쇼잉시나리오는 시나리오의 이름을 키로, 시나리오값을 갖는다.
        //시나리오는 씬네임, 해당씬을 보기위한 신뢰도제한, 해당 씬에서 npc가 말할 대사, 가능한 다음 씬들의이름을 들고있다. 다른 씬들의 이름을 열쇠로 다음대화를 진행한다.
    }

    public void Skip()
    {
        if (_selectedScenarioData.Item2 >= 20)
        {
            ProjectSceneManager.Instance.CallAdditiveScene(11);
            return;
        }
        ProjectSceneManager.Instance.CallAdditiveScene(12);
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
    
    //초기 버튼 생성, 리스너 연결
    private void StartConversation()
    {
        Button firstButton = Instantiate(selectButtonPrefab, selectButtonParent).GetComponent<Button>();
        string firstText = _selectedScenarioData.Item1.ScenarioBooks[0].SceneName;
        firstButton.GetComponentInChildren<TextMeshProUGUI>().text = firstText;
        firstButton.onClick.AddListener(() => PrintOutSentence(showingScenarios[firstText]));
        scrollViewObject.SetActive(true);
    }
    
    //생성된 버튼 클릭시 해당 시나리오의 텍스트들 출력
    private void PrintOutSentence(Scenario scenario)
    {
        StartCoroutine(ShowSentence(scenario));
        //Debug.Log(_selectedScenarioData.Item2);
        if (string.IsNullOrEmpty(scenario.ImagePath) || scenario.ImagePath.Equals(""))//
        {
            characterImage.gameObject.SetActive(false);
        }
        else
        {
            characterImage.gameObject.SetActive(true);
            string folderPath = $"BackgroundInfoStory/ByTimeLine{_selectedScenarioData.Item2}";
            
            Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);
            characterImage.sprite = sprites.FirstOrDefault(sprite => sprite.name.Contains(scenario.ImagePath));
        }
        

        // scenario.ImagePath를 포함하는 스프라이트 찾기
        //Sprite foundSprite = sprites.FirstOrDefault(sprite => sprite.name.Contains(scenario.ImagePath));
        
        for (int i = 0; i < selectButtonParent.childCount; i++)
        {
            GameObject buttonObject = selectButtonParent.GetChild(i).gameObject; 
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            Destroy(buttonObject);
        }
        scrollViewObject.SetActive(false);
    }
    
    
    //내용 출력 중
    private IEnumerator ShowSentence(Scenario scenario)
    {
        List<string> sentences = new List<string>(scenario.Sentences);
        int sentencesCount = sentences.Count;
        
        for (int i = 0; i < sentencesCount; i++)
        {
            if (sentences[i].Contains("PlayerName"))
            {
                sentences[i] = sentences[i].Replace("PlayerName", GameImmortalManager.Instance.PlayerName);
            }
            if (IsAuto)
            {
                yield return PrintStringCoroutine(sentences[i]);
                yield return new WaitForSeconds(1f); // 문장 끝 대기    
            }
            else
            {
                yield return PrintStringCoroutineNeedAnyInput(sentences[i]);
                yield return WaitForMouseClick();
            }
        }
        //yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0.5f);//테스트용
        SetButtonOrContinue(scenario);
        //SetSelectButton(scenario.NextSceneNames);
    }

    private void SetButtonOrContinue(Scenario scenario)
    {
        //브랜치가 트루인 시나리오, false인 시나리오
        SetSelectButton(scenario.NextSceneNames);    
        //이 전에, 신뢰도 스텟 있었을 때 사용했던 코드
        //코드의 무덤
        /*//분기시나리오인가?
        if (scenario.IsBranch)
        {
            //다음씬 이름 받을 준비
            string trustSceneName = "";
            string noneTrustSceneName = "";
            for (int i = 0; i < scenario.NextSceneNames.Count; i++)
            {
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
            SetSelectButton(scenario.NextSceneNames);    
            
        }
        else
        {
            SetSelectButton(scenario.NextSceneNames);
        }*/
    }

    //선택지를 인자로받음
    private void SetSelectButton(List<string> possibleNextScenarioNames)
    {
        int createButtonCount = possibleNextScenarioNames.Count;
        if (createButtonCount == 0)
        {
            if (_selectedScenarioData.Item2 >= 20)//엔딩출력이면
            {
                //이니셜라이즈드로 보내버린다.
                ProjectSceneManager.Instance.CallAdditiveScene(11);
                return;
            }
            ProjectSceneManager.Instance.CallAdditiveScene(12);
            return;
        }
        
        for (int i = 0; i < createButtonCount; i++)
        {
            Button button = Instantiate(selectButtonPrefab, selectButtonParent).GetComponent<Button>();
            string text = possibleNextScenarioNames[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = text;   
            button.onClick.AddListener(() => PrintOutSentence(showingScenarios[text]));
            //Debug.Log(possibleNextScenarioNames[i]);
        }
        
        //Debug.Log(selectButtonParent.childCount);
        /*for (int i = 0; i < selectButtonParent.childCount; i++)
        {
            Button button = selectButtonParent.GetChild(i).GetComponent<Button>();
            if (i < possibleNextScenarioNames.Count)
            {
                string text = possibleNextScenarioNames[i];
                button.GetComponentInChildren<TextMeshProUGUI>().text = text;   
                button.onClick.AddListener(() => PrintOutSentence(showingScenarios[text]));
            }
        }*/
        scrollViewObject.SetActive(true);
    }
    
    //20250210 얘가 계속도네 -> 해결. 신뢰도 분기관련해서 수정이 덜 됐었음
    private IEnumerator PrintStringCoroutine(string sentence)
    {
        string text = sentence;
        string currentText = "";
        characterSentence.text = currentText;

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i]; // 문자열을 점점 늘려가며 추가
            characterSentence.text = currentText;
            //UiSoundManager.Instance.RandomTypingSound();
            //Debug.Log(currentText); // 출력
            //yield return new WaitForSeconds(0.1f); // 0.1초 대기
            yield return _waitForSeconds; // 0.05초대기, 테스트용
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
                UiSoundManager.Instance.RandomTypingSound();
                yield break; // 코루틴 종료
            }
            yield return null;
            currentText += text[i]; // 문자열을 점점 늘려가며 추가
            characterSentence.text = currentText;
            //UiSoundManager.Instance.RandomTypingSound();
            //Debug.Log(currentText); // 출력
            yield return _waitForSeconds; // 0.05초 대기

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
}
