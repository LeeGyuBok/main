using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISide_MainSceneUiManager : MortalManager<UISide_MainSceneUiManager>
{
    [SerializeField] private RectTransform scrollViewParent;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject confirmPanel;
    private float threshold = 100f;
    private float buttonHeight = 100f;

    private List<FriendlyOperator> friendlyOperators;

    private int contentsIndex;

    //private int ShowingfirstIndex;
    //private int ShowinglastIndex;
    
    private Dictionary<Button , FriendlyOperator> dataByButton = new Dictionary<Button , FriendlyOperator>();
    

    // Start is called before the first frame update
    void Start()
    {
        friendlyOperators = new List<FriendlyOperator>(UISide_ImmortalGameManager.Instance.CharacterPrefab);
        for (int i = 0; i < friendlyOperators.Count; i++)
        {
            Button button = Instantiate(buttonPrefab, scrollViewParent);
            dataByButton[button] = friendlyOperators[i];
            button.onClick.AddListener(() => SetOperatorData(button));
            button.GetComponentInChildren<TextMeshProUGUI>().text = friendlyOperators[i].TrustData.CharacterName + $"{i}";
            
        }

        //ShowingfirstIndex = 0;
        //ShowinglastIndex = scrollViewParent.transform.childCount;
    }
    // 종료 버튼으로 재사용하자.
    /// <summary>
    /// 게임종료 로직으로 오버라이드됨
    /// </summary>
    public override void ButtonToMainScene()
    {
        UISide_ImmortalGameManager.Instance.ExitGame();
    }

    public void EnterGameWithNpc()
    {
        scrollView.SetActive(true);
        UISide_ImmortalGameManager.Instance.SetPlayMode(PlayMode.CardGame);
    }
    
    public void EnterArchiveAboutNpc()
    {
        scrollView.SetActive(true);
        UISide_ImmortalGameManager.Instance.SetPlayMode(PlayMode.Archive);
    }

    public void ExitScrollView()
    {
        scrollView.SetActive(false);
    }

    public void CancelConfirm()
    {
        confirmPanel.SetActive(false);
    }

    public void LoadScene()
    {
        switch (UISide_ImmortalGameManager.Instance.SelectedPlayMode)
        {
            case PlayMode.Archive:
                UISide_ImmortalSceneManager.Instance.LoadSelectQuestionScene();    
                break;
            case PlayMode.CardGame:
                UISide_ImmortalSceneManager.Instance.LoadGameScene();
                break;
        }
    }

    

    private void SetOperatorData(Button button)
    {
        UISide_ImmortalGameManager.Instance.SelectOperator(dataByButton[button]);
        UISide_ImmortalJsonManager.Instance.SetSelectedCharacterName(dataByButton[button]);
        confirmPanel.SetActive(true);
    }
    //버튼 동적생성시 최적화하기.
    public void RectDelta(Vector2 delta)
    {
        /*int ypos = (int)scrollViewParent.transform.localPosition.y;
        if (scrollViewParent.anchoredPosition.y > threshold)
        {
            // 첫 번째 버튼을 가져와 마지막으로 이동
            Transform firstButton = scrollViewParent.GetChild(ShowingfirstIndex);
            firstButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = indexDictionary[ShowinglastIndex + 1].TrustData.ScenarioName;
            firstButton.SetAsLastSibling();

            // Content의 위치를 조정해 "순환" 효과
            Vector2 newPos = scrollViewParent.anchoredPosition;
            newPos.y -= buttonHeight; // Content의 Y 위치를 버튼 높이만큼 줄임.
            scrollViewParent.anchoredPosition = newPos;
        }
        // 스크롤이 위로 이동하여 Content가 -threshold를 초과했을 때
        // 지금 이부분이 되지않아.
        else if (scrollViewParent.anchoredPosition.y < -threshold)
        {
            Debug.Log("SetAsFirstSibling");
            // 마지막 버튼을 가져와 첫 번째로 이동
            Transform lastButton = scrollViewParent.GetChild(scrollViewParent.childCount - 1);
            lastButton.SetAsFirstSibling();

            // Content의 위치를 조정해 "순환" 효과
            Vector2 newPos = scrollViewParent.anchoredPosition;
            newPos.y += buttonHeight; // Content의 Y 위치를 버튼 높이만큼 높임
            scrollViewParent.anchoredPosition = newPos;
        }*/
    }
}
