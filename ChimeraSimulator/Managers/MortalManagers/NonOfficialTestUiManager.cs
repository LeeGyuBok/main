using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class NonOfficialTestUiManager : MortalManager<NonOfficialTestUiManager>, IGoMain
{
    [SerializeField] private ScrollRect testLogScrollRect;
    [SerializeField] private Transform testLog;
    [SerializeField] private TextMeshProUGUI testLogPrefab;
    [SerializeField] private GameObject goMainButton;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI opponentName;
    [SerializeField] private TextMeshProUGUI rewardChimeraInfo;
    [SerializeField] private TextMeshProUGUI combatResultInfo;

    [SerializeField] private TextMeshProUGUI getResult;
    
    private List<string> _testLogText;
    private const int TestLogMaxCount = 100;
    
    private readonly WaitForSeconds _delay = new(0.4f);

    protected override void Awake()
    {
        base.Awake();
        _testLogText = new List<string>(TestLogMaxCount);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goMainButton.SetActive(false);
        resultPanel.SetActive(false);
        getResult.gameObject.SetActive(true);
        StartCoroutine(GetResultTexting());
    }

    public void TurnOnGoMainButton()
    {
        GameImmortalManager.Instance.AddDailySupplyTokens();
        goMainButton.SetActive(true);
        getResult.gameObject.SetActive(false);
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
        textMesh.text = testLogText;
        textMesh.transform.SetAsLastSibling();
        
        testLogScrollRect.verticalNormalizedPosition = 0f;
    }

    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }

    public void SetResultText(string opponent, string reward, string combatResult)
    {
        opponentName.text = opponent;
        rewardChimeraInfo.text = reward;
        combatResultInfo.text = combatResult;
        UiSoundManager.Instance.InDigitalSound();
        resultPanel.SetActive(true);
    }

    private IEnumerator GetResultTexting()
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            getResult.text += ".";
            //Debug.Log(timer);
            yield return _delay;
            timer += 0.4f;
        }
    }
}
