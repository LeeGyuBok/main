using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderOfficeUiManager : MortalManager<CommanderOfficeUiManager>
{
    public CommanderCharacter commander { get; private set; }
    private int queueData;
    [SerializeField] private GameObject noDataPanel;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        noDataPanel.SetActive(false);
    }

    private void Start()
    {
        (CommanderCharacter, Queue<string>) data = GameImmortalManager.Instance.GetInterviewData();
        commander = data.Item1;
        queueData = data.Item2.Count;
    }

    public void ButtonToInterviewScene()
    {
        if (queueData < 1)
        {
            noDataPanel.SetActive(true);
            return;
        }
        //'인물'이 정해져서 면담이 가능하다면, 
        SceneImmortalManager.Instance.LoadInterviewScene();
        //불가능의 경우, 팝업창을 띄우고(확인버튼만 있음.) 홈으로 보낸다.
    }
}
