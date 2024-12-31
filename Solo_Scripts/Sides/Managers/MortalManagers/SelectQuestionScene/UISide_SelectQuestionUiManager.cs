using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//애는 아카이브씬에서도 사용하면 되겠다.
public class UISide_SelectQuestionUiManager : MortalManager<UISide_SelectQuestionUiManager>
{
    [SerializeField] private GameObject questionSelectPanel;
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Button heightFixedButtonPrefab;

    private void Start()
    {
        SetQuestion();
    }

    private void SetQuestion()
    {
        List<ScenarioBook> books = new List<ScenarioBook>(UISide_ImmortalJsonManager.Instance.SelectedScenarioJson.ScenarioBooks);
        for (int i = 0; i < books.Count; i++)
        {
            Button button = Instantiate(heightFixedButtonPrefab, buttonsParent);
            int selectedIndex = i;
            button.GetComponentInChildren<TextMeshProUGUI>().text = books[selectedIndex].ScenarioName;
            button.onClick.AddListener(() => ReceiveAnswer(books[selectedIndex]));
        }
    }

    private void ReceiveAnswer(ScenarioBook selectedScenarioBook)
    {
        UISide_ImmortalGameManager.Instance.SetScenarioBook(selectedScenarioBook);
        UISide_ImmortalSceneManager.Instance.LoadReceiveAnswerScene();
    }
}
