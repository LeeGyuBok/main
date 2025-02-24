using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OfficialTestUiManager: MortalManager<OfficialTestUiManager>, IGoMain
{
    [SerializeField] private ScrollRect testLogScrollRect;
    [SerializeField] private Transform testLog;
    [SerializeField] private TextMeshProUGUI testLogPrefab;
    [SerializeField] private GameObject goMainButton;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI opponentName;
    [SerializeField] private TextMeshProUGUI rewardChimeraInfo;
    [SerializeField] private TextMeshProUGUI combatResultInfo;

    [FormerlySerializedAs("getResult")] [SerializeField] private TextMeshProUGUI gettingResult;
    
    [SerializeField] private TextMeshProUGUI totalResult;

    [SerializeField] private Button nextMatchButton;
    
    private List<string> _testLogText;
    private const int TestLogMaxCount = 100;

    private readonly WaitForSeconds _delay = new(0.33f);
    
    private List<TextMeshProUGUI> _beforeTestLogs;

    protected override void Awake()
    {
        base.Awake();
        _testLogText = new List<string>(TestLogMaxCount);
        _beforeTestLogs = new List<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChimeraTesting();
    }

    private void ChimeraTesting()
    {
        totalResult.gameObject.SetActive(false);
        goMainButton.SetActive(false);
        resultPanel.SetActive(false);
        gettingResult.gameObject.SetActive(true);
        StartCoroutine(GetResultTexting());
    }

    public void TurnOnGoMainButton()
    {
        GameImmortalManager.Instance.AddDailySupplyTokens();
        goMainButton.SetActive(true);
        gettingResult.gameObject.SetActive(false);
    }
    
    public void AddTestLog(string testLogText)
    {
        if (_testLogText.Count >= TestLogMaxCount)
        {
            _testLogText.RemoveAt(0);
            Destroy(testLog.GetChild(0).gameObject);
        }
        
        _testLogText.Add(testLogText);
        
        TextMeshProUGUI textMesh = Instantiate(testLogPrefab, testLog);
        _beforeTestLogs.Add(textMesh);
        textMesh.text = testLogText;
        textMesh.transform.SetAsLastSibling();
        
        testLogScrollRect.verticalNormalizedPosition = 0f;
    }

    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }

    public void SetResultText(string opponent, string reward, string combatResult, int total = 0)
    {
        opponentName.text = opponent;
        rewardChimeraInfo.text = reward;
        combatResultInfo.text = combatResult;
        //알람소리로 바꿔주세요
        UiSoundManager.Instance.InDigitalSound();
        if (total != 0)
        {
            totalResult.gameObject.SetActive(true);
            totalResult.text = $"총 승리 회 수: {total}";
        }
        resultPanel.SetActive(true);
    }
    private IEnumerator GetResultTexting()
    {
        int index = _beforeTestLogs.Count;
        if (index != 0)
        {
            for (int i = 0; i < index; i++)
            {
                Destroy(_beforeTestLogs[i].gameObject);
            }
            _beforeTestLogs.Clear();
            _testLogText.Clear();
        }
        
        testLog.gameObject.SetActive(false);
        SetNextMatchButton(false);
        resultPanel.SetActive(false);
        gettingResult.gameObject.SetActive(true);
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            gettingResult.text += ".";
            //Debug.Log(timer);
            yield return _delay; // 기다린만큼 지난시간을 추가한다.
            timer += 0.33f;
            
        }
        gettingResult.text = "결과 분석 중";
        testLog.gameObject.SetActive(true);
        gettingResult.gameObject.SetActive(false);
        //Debug.Log("coroutine end");
    }
    
    
    //얘는 넥스트 매치 버튼에 연결할 것
    public void GoNextCombat()
    {
        GameImmortalManager.Instance.AddDailySupplyTokens();
        ChimeraTesting();
        OfficialTestManager.Instance.StartTesting();
    }

    public void SetNextMatchButton(bool set)
    {
        nextMatchButton.gameObject.SetActive(set);
    }
}