using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISide_CardGameUiManager : MortalManager<UISide_CardGameUiManager>
{
    [SerializeField] private Button PickAndSortButton;
    [SerializeField] private Button ShowSecondaryButton;
    [SerializeField] private Button AddNewCardFromOpponentDeckButton;
    [SerializeField] private Button ShowResultButton;
    
    [SerializeField] private GameObject ResultPanel;
    [SerializeField] private Button AcceptButton;
    [SerializeField] private Button GotoMainButton;

    public void Start()
    {
        ShowSecondaryButton.gameObject.SetActive(false);
        AddNewCardFromOpponentDeckButton.gameObject.SetActive(false);
        ShowResultButton.gameObject.SetActive(false);
    }

    public void PickAndSortCards()
    {
        UISide_CardGameObjectManager.Instance.PickAndSortCards();
        StartCoroutine(ShowDelayButton(ShowSecondaryButton));
    }

    public void ShowSecondLargeCard()
    {
        UISide_CardGameObjectManager.Instance.ShowSecondLargeCard();

        StartCoroutine(ShowDelayButton(AddNewCardFromOpponentDeckButton));
    }

    public void AddNewCardFromOpponentCardList()
    {
        if (UISide_CardGameObjectManager.Instance.SetAdditionalCard) return;
        
        UISide_CardGameObjectManager.Instance.AddNewCardFromOpponentCardList();
        ShowResultButton.gameObject.SetActive(false);
        StartCoroutine(ShowDelayButton(ShowResultButton));
    }

    public void ShowResult()
    {
        UISide_CardGameObjectManager.Instance.ShowResult();
    }
    
    public void ReStartCardGame()
    {
        UISide_CardGameObjectManager.Instance.ReGame();
        ResultPanel.SetActive(false);
        
        ShowSecondaryButton.gameObject.SetActive(false);
        AddNewCardFromOpponentDeckButton.gameObject.SetActive(false);
        ShowResultButton.gameObject.SetActive(false);
    }

    public void AskQuestion()
    {
        //UISide_ImmortalSceneManager.Instance.
    }

    public void GoToMain()
    {
        UISide_ImmortalSceneManager.Instance.LoadMainScene();
    }

    public void SetButtonByResult(GameResult result)
    {
        switch (result)
        {
            case GameResult.Draw:
            case GameResult.Lose:
                AcceptButton.onClick.RemoveAllListeners();
                AcceptButton.onClick.AddListener(ReStartCardGame);
                AcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "ReStart";
                ResultPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{result.ToString()}";
                break;
            case GameResult.Win:
                AcceptButton.onClick.RemoveAllListeners();
                AcceptButton.onClick.AddListener(AskQuestion);
                AcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ask Question";
                ResultPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{result.ToString()}";
                break;
        }
        ResultPanel.SetActive(true);
    }

    private IEnumerator ShowDelayButton(Button button)
    {
        yield return new WaitForSeconds(1.8f);
        button.gameObject.SetActive(true);
    }
}
