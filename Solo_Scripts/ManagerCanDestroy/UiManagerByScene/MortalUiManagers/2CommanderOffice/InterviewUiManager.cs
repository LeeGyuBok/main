using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InterviewUiManager : MortalManager<InterviewUiManager>
{
    [SerializeField] private Button exitInterviewButton;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform selectButtonParent;
    [SerializeField] private Button selectButtonPrefab;
    
    
    private CommanderCharacter selectedCommander;
    private Queue<string> playerActivationLog;
    private DialogueNode dialogueNode;
    private int selectedResponseIndex = -1;

    private bool needResoponse { get; set; }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
      
    }

    private void Start()
    {
        (CommanderCharacter, Queue<string>) data = GameImmortalManager.Instance.GetInterviewData();
        selectedCommander = data.Item1;
        playerActivationLog = new Queue<string>(data.Item2);

        if (playerActivationLog.Count < 1)
        {
            SceneImmortalManager.Instance.LoadCommanderOfficeScene();
            Debug.Log("no player Activation log");
            return;
        }

        selectedCommander.SetDialogueNode(playerActivationLog);
        
        dialogueNode = selectedCommander.startDialogueNode;
        //Debug.Log(selectedCommander.Data.CharacterName);
        StartCoroutine(ShowAndWaitInput(dialogueNode));
    }


    public override void ButtonToMainScene()
    {
        GameImmortalManager.Instance.ClearInterviewData();
        selectedCommander.InitializeDialogueNode();
        base.ButtonToMainScene();
    }

    private void SettingDialogue(DialogueNode selectedDialogue)
    {
        if (exitInterviewButton.gameObject.activeInHierarchy)
        {
            exitInterviewButton.gameObject.SetActive(false);    
        }
        if (selectButtonParent.gameObject.activeInHierarchy)
        {
            int childCount = selectButtonParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject button = selectButtonParent.GetChild(0).gameObject;
                button.GetComponent<Button>().onClick.RemoveAllListeners();
                DestroyImmediate(button);
            }
            selectButtonParent.gameObject.SetActive(false);
        }
        
        characterName.text = selectedCommander.TrustData.CharacterName;
        dialogueText.text = selectedDialogue.sentence;
        
        if (!characterName.gameObject.activeInHierarchy)
        {
            characterName.gameObject.SetActive(true);
        }
        if (!dialogueText.gameObject.activeInHierarchy)
        {
            dialogueText.gameObject.SetActive(true);    
        }

        if (!selectedDialogue.isEndNode)
        {
            needResoponse = true;    
        }
        else
        {
            StartCoroutine(TestExitButtonOn());
        }
        
    }

    private void SettingResponseDialogue(List<DialogueNode> response) 
    {
        if (exitInterviewButton.gameObject.activeInHierarchy)
        {
            exitInterviewButton.gameObject.SetActive(false);    
        }
        
        characterName.text = selectedCommander.TrustData.CharacterName;//플레이어네임으로 바꿀 예정
        dialogueText.text = "";
        
        if (!selectButtonParent.gameObject.activeInHierarchy)
        {
            selectButtonParent.gameObject.SetActive(true);
        }

        //Queue<string> logQueue = playerActivationLog;
        //Queue<string> logQueue = new Queue<string>(playerActivationLog); // <- 올바른 복사본 생성법이라는데?
        
        //이 아래가 플레이어에게 줄 선택지 생성장소
        
        for (int i = 0; i < response.Count; i++)
        {
            Button button = Instantiate(selectButtonPrefab, selectButtonParent);
            button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = response[i].sentence;

            int index = i;
            button.onClick.AddListener(() => SetSelectedIndex(index));
        }
    }

    private void SetSelectedIndex(int index)
    {
        selectedResponseIndex = index;
        Debug.Log(index);
    }

    private void SettingNonResponseDialogue(DialogueNode middleDialogueNode)
    {
        if (!exitInterviewButton.gameObject.activeInHierarchy)
        {
            exitInterviewButton.gameObject.SetActive(true);    
        }
        
        characterName.text = selectedCommander.TrustData.CharacterName;
        dialogueText.text = middleDialogueNode.sentence;
        
        if (selectButtonParent.gameObject.activeInHierarchy)
        { 
            int childCount = selectButtonParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(selectButtonParent.GetChild(0).gameObject);
            }
            selectButtonParent.gameObject.SetActive(false);
        }
    }

    private IEnumerator TestExitButtonOn()
    {
        yield return new WaitForSeconds(1f);
        exitInterviewButton.gameObject.SetActive(true);
    }

    private IEnumerator ShowAndWaitInput(DialogueNode selectedDialogue)
    {
        //받은 노드가 끝노드?
        while (!selectedDialogue.isEndNode)//ㄴㄴ
        {
            //텍스트 셋팅. 해당 노드의 대사 셋팅 및 출력
            SettingDialogue(selectedDialogue);
        
            //키입력 대기
            while (needResoponse)
            {
                if (Input.anyKeyDown)
                {
                    needResoponse = false;
                }
                yield return null;
            }

            if (selectedDialogue.responses.Count > 1)
            {
                SettingResponseDialogue(selectedDialogue.responses);
                //셀릭티드리스펀스인데스가 0인동안 대기 == 버튼을 누르면 바뀔 예정
                while (selectedResponseIndex == -1)
                {
                    yield return null;
                }
                selectedDialogue = selectedDialogue.responses[selectedResponseIndex];
                selectedResponseIndex = -1;
            }
            else//만약 반응이 하나이하의 노드였다면
            {
                //일단 노드에 반응이 있는지 확인. 없으면 코루틴 종료
                if (selectedDialogue.responses.Count == 0)
                {
                    Debug.LogError("No response");
                    yield break;   
                }
                //한 개 있으면, 와일문에 들어가는 다이어로그를 그 한 개의 노드로 변경.
                selectedDialogue = selectedDialogue.responses[0];
            }

            
        }//와일이 끝 == 엔드노드다.
        SettingDialogue(selectedDialogue);
    }
}
