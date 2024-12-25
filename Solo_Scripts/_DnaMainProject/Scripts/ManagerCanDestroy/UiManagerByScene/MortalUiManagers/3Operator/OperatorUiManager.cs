using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorUiManager : MortalManager<OperatorUiManager>
{
    [SerializeField] private Button operatorButtonPrefab;
    [SerializeField] private Transform operatorButtonParent;
    [SerializeField] private ScrollRect scrollView;

    private Vector2 beforeAmount = Vector2.zero;
    private float movedAmount = 0f;

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

        for (int i = 0; i < operatorButtonParent.childCount; i++)
        {
            if (operatorList[i] is FriendlyOperator friendlyOperator)
            {
                Button button = operatorButtonParent.GetChild(i).gameObject.GetComponent<Button>();
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
        
        /*for (int i = 0; i < operatorList.Count; i++)
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
        }*/
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

    public void ScrollAmount(Vector2 amount)
    {
        int ypos = -(int)scrollView.content.transform.localPosition.y;
        Debug.Log(ypos);//-20씩 늘고 줄어듦
        /*if (beforeAmount == Vector2.zero)
        {
            beforeAmount = amount;
        }
        else
        {
            movedAmount = (beforeAmount - amount).y;
            if (movedAmount >= 0.2f)
            {
                Debug.Log(movedAmount);
                movedAmount = 0f;
                return;
            }

            if (movedAmount < 0.2f)
            {
                Debug.Log(movedAmount);
                movedAmount = 0f;
            }
            
            beforeAmount = amount;
        }*/
        
        
    }
}
