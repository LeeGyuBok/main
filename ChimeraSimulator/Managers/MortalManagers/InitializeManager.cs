using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InitializeManager : MortalManager<InitializeManager>
{
    [SerializeField] private GameObject rulePanel;
    [SerializeField] private GameObject inputFieldPanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private TextMeshProUGUI inputtedName;

    private float _enterDelay;

    public void Start()
    {
        inputField.onEndEdit.AddListener(SetPlayerName);
        inputFieldPanel.SetActive(false);
        alertPanel.SetActive(false);
    }

    private void Update()
    {
        if (alertPanel.activeInHierarchy)
        {
            _enterDelay += Time.deltaTime;
            if (_enterDelay <= 0.1f) return;
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EnterGame();
            }
        }
    }

    public void EnterLab()
    {
        UiSoundManager.Instance.InfoSound();
        inputFieldPanel.SetActive(true);
    }

    public void OpenRule()
    {
        UiSoundManager.Instance.InfoSound();
        rulePanel.SetActive(true);
    }

    public void CloseRule()
    {
        UiSoundManager.Instance.InfoSound();
        rulePanel.SetActive(false);
    }
    private void SetPlayerName(string playerName)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameImmortalManager.Instance.SetPlayerName(playerName);
            inputtedName.text = playerName;
            EnterGameAlert();
        }
    }

    public void CloseInputFieldPanel()
    {
        UiSoundManager.Instance.InfoSound();
        inputField.text = string.Empty;
        _enterDelay = 0f;
        inputFieldPanel.SetActive(false);
    }

    private void EnterGameAlert()
    {
        UiSoundManager.Instance.InfoSound();
        alertPanel.SetActive(true);
    }

    public void CloseEnterGameAlert()
    {
        UiSoundManager.Instance.InfoSound();
        GameImmortalManager.Instance.SetPlayerName(string.Empty);
        _enterDelay = 0f;
        alertPanel.SetActive(false);
    }

    public void EnterGame()
    {
        if (GameImmortalManager.Instance.PlayerName.Equals(string.Empty))
        {
            _enterDelay = 0f;
            return;
        }
        UiSoundManager.Instance.InfoSound();
        GameImmortalManager.Instance.SetInitialGameResources();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // PlayMode 종료
#else
                Application.Quit(); // 빌드된 환경에서 게임 종료
#endif
    }
}
