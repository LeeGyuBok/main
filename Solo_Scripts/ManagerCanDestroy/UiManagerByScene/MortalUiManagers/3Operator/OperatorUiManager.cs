using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorUiManager : MortalManager<OperatorUiManager>
{
    [SerializeField] private Button operatorButtonPrefab;
    [SerializeField] private Transform operatorButtonParent;

    /// <summary>
    /// Copy of GameImmortalManagerInstance's List, characterPrefab.
    /// </summary>
    private List<OperatorBattleStatus> operatorList;
    private Dictionary<Button, OperatorBattleStatus> operatorButtonDictionary;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        operatorButtonDictionary = new Dictionary<Button, OperatorBattleStatus>();
    }

    void Start()
    {
        operatorList = GameImmortalManager.Instance.GetOperatorDataList();

        for (int i = 0; i < operatorList.Count; i++)
        {
            //FriendlyOperator friendlyOperator = operatorList[i] as FriendlyOperator; 
            
            if (operatorList[i] is FriendlyOperator friendlyOperator)
            {
                Button button = Instantiate(operatorButtonPrefab, operatorButtonParent);
                operatorButtonDictionary[button] = operatorList[i];
                button.GetComponentInChildren<TextMeshProUGUI>().text = friendlyOperator.TrustData.CharacterName;
                int index = i;
                button.onClick.AddListener(() => ButtonToOperatorDetail(operatorList[index]));
            }
            else
            {
                Debug.Log($"{operatorList[i]} is not friendly operator.");
            }
        }
    }

    private void ButtonToOperatorDetail(OperatorBattleStatus operatorBattleStatus)
    {
        GameImmortalManager.Instance.SetOperatorData(operatorBattleStatus);
        for (int i = 0; i < operatorButtonParent.childCount; i++)
        {
            Button button = operatorButtonParent.GetChild(i).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            DestroyImmediate(button.gameObject);
        }
        SceneImmortalManager.Instance.LoadOperatorDetailScene();
    }

    public override void ButtonToMainScene()
    {
        for (int i = 0; i < operatorButtonParent.childCount; i++)
        {
            Button button = operatorButtonParent.GetChild(i).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            DestroyImmediate(button.gameObject);
        }
        operatorList.Clear();
        operatorList = null;
        
        base.ButtonToMainScene();
    }
}
