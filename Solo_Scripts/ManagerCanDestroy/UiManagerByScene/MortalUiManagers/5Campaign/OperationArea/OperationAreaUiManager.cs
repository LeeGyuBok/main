using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OperationAreaUiManager : MortalManager<OperationAreaUiManager>
{
    #region MapStateVar

    private WorldMap worldMap;
    private ContinentMap continentMap;
    private OperationAreaMap operationAreaMap;
    private OperationAreaDetail operationAreaDetail;

    private Dictionary<WorldUiObject, IAreaState> worldUiObjectsByButton = new();
    private Vector2 screenCenter;
    public IAreaState currentAreaState { get; private set; }
    public bool IsMoveCam { get; private set; }

    //맵 상태 변경 버튼리스트
    [SerializeField] private List<WorldUiObject> worldContinentButtons;
    public List<WorldUiObject> WorldContinentButtons => worldContinentButtons;
    [SerializeField] private List<WorldUiObject> continentAreaButtons;
    public List<WorldUiObject> ContinentAreaButtons => continentAreaButtons;
    [SerializeField] private List<OperationAreaInfo> operationAreaButtons;
    public List<OperationAreaInfo> OperationAreaButtons => operationAreaButtons;
    
    //버튼리스트를 한번에 제어
    [SerializeField] private GameObject worldContinentButtonsGameObject;
    public GameObject WorldContinentButtonsGameObject => worldContinentButtonsGameObject;
    [SerializeField] private GameObject continentAreaButtonsGameObject;
    public GameObject ContinentAreaButtonsGameObject => continentAreaButtonsGameObject;
    [SerializeField] private GameObject operationAreaButtonsGameObject;
    public GameObject OperationAreaButtonsGameObject => operationAreaButtonsGameObject;

    #endregion
   
    
    
    [SerializeField] private GameObject operationAreaInfoPanel;
    [SerializeField] private GameObject possibleOperatorPanel;
    [SerializeField] private GameObject possibleOperatorButtonListParent;
    [SerializeField] private Button possibleOperatorButtonPrefab;
    [SerializeField] private List<Button> selectedOperatorButtons;
    
    private List<FriendlyOperator> friendlyOperators;
    private Dictionary<Button, FriendlyOperator> friendlyOperatorButtons;
    private List<FriendlyOperator> selectedFriendlyOperators;
    private Dictionary<Button, FriendlyOperator> selectedFriendlyOperatorButtons;
    

    
    protected override void Awake()
    {
        base.Awake();
        worldMap = new WorldMap();
        continentMap = new ContinentMap();
        operationAreaMap = new OperationAreaMap();
        operationAreaDetail = new OperationAreaDetail();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        friendlyOperators = new List<FriendlyOperator>(GameImmortalManager.Instance.FriendlyOperators);
        selectedFriendlyOperators = new List<FriendlyOperator>();
        friendlyOperatorButtons = new Dictionary<Button, FriendlyOperator>();
        selectedFriendlyOperatorButtons = new Dictionary<Button, FriendlyOperator>();

        for (int i = 0; i < friendlyOperators.Count; i++)
        {
            Button button = Instantiate(possibleOperatorButtonPrefab, possibleOperatorButtonListParent.transform);
            button.onClick.AddListener(() => OperatorSelect(button));
            friendlyOperatorButtons.Add(button, friendlyOperators[i]);
        }
        
        //월드맵상태에서 버튼클릭시, 대륙맵상태로 전환
        for (int i = 0; i < worldContinentButtons.Count; i++)
        {
            worldUiObjectsByButton.Add(worldContinentButtons[i], continentMap);
        }
        //대룍맵상태에서 버튼클릭시, 작전지역 상태로 전환
        for (int i = 0; i < continentAreaButtons.Count; i++)
        {
            worldUiObjectsByButton.Add(continentAreaButtons[i], operationAreaMap);
        }

        for (int i = 0; i < operationAreaButtons.Count; i++)
        {
            worldUiObjectsByButton.Add(operationAreaButtons[i], operationAreaDetail);
        }

        currentAreaState = worldMap;
        currentAreaState.EnterState();
        
        ImmortalCamera.Instance.gameObject.transform.position = new Vector3(0f, currentAreaState.CameraYPos, 0f);
        ImmortalCamera.Instance.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    #region MapState
    
    public void SetWorldContinentStater()
    {
        currentAreaState.ExitState();
        currentAreaState = worldMap;
        StartCoroutine(CameraMove(gameObject));
    }

    public void SetContinentState()
    {
        currentAreaState.ExitState();
        currentAreaState = continentMap;
        StartCoroutine(CameraMove(gameObject));
        
    }
    
     public void ChangeMapState(WorldUiObject worldUiObject)
    {
        if (currentAreaState == null)
        {
            currentAreaState = worldMap;
            currentAreaState.EnterState();
            return;
        }
        
        currentAreaState.ExitState();
        
        currentAreaState = worldUiObjectsByButton[worldUiObject];
        if (currentAreaState.Equals(operationAreaDetail))
        {
            OperationAreaObjectManager.Instance.SetSelectedOperationArea(worldUiObject as OperationAreaInfo);
            operationAreaInfoPanel.SetActive(true);
            possibleOperatorPanel.SetActive(true);
        }
        else
        {
            if (operationAreaInfoPanel.activeInHierarchy || possibleOperatorPanel.activeInHierarchy)
            {
                operationAreaInfoPanel.SetActive(false);
                possibleOperatorPanel.SetActive(false);
            }
        }

        IsMoveCam = true;
        StartCoroutine(CameraMove(worldUiObject.gameObject));
    }

    private IEnumerator CameraMove(GameObject worldUiObject)
    {
        if (ImmortalCamera.Instance.gameObject.transform.position.y > currentAreaState.CameraYPos)
        {
            //카메라가 현재 지역상태의 정해진 카메라위치값보다 크면 == 작아져야하면
            while (ImmortalCamera.Instance.gameObject.transform.position.y > currentAreaState.CameraYPos)
            {
                //Debug.Log(ImmortalCamera.Instance.gameObject.transform.position.y > currentAreaState.CameraYPos);
                /*버튼에 가까이 갈때 -> y값을 내린다. 이 때, 버튼의 '방향'으로도 움직여야한다. 둘을 따로 움직여도될거같은데 y값 먼저 맞추고 x,z값 맞추기.*/
                Vector3 direction = (worldUiObject.transform.position - ImmortalCamera.Instance.gameObject.transform.position).normalized;
                Vector3 delta = direction * (currentAreaState.CameraMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position += delta;
                yield return null;    
                
            }
            //언제까지 가까이가나? 버튼이 화면의 중앙에 올 때 까지.화면의 중앙? 오브젝트의 위치를 화면상의 위치로 변환하여 그 위치가 스크린센터인지확인한다.
            //거리로하지말고.. 그냥 x,y좌표로 해야하나?
            //다시 월드좌표상으로해야..하나?
            
            Vector3 directionToWorldUiObject = (worldUiObject.transform.position - ImmortalCamera.Instance.gameObject.transform.position).normalized;
            Vector2 worldUiScreenPosition = new Vector2(worldUiObject.transform.position.x, worldUiObject.transform.position.z);
            Vector2 cameraScreenPosition = new Vector2(ImmortalCamera.Instance.gameObject.transform.position.x, ImmortalCamera.Instance.gameObject.transform.position.z);
            float distance = Vector2.Distance(worldUiScreenPosition, cameraScreenPosition);
            //Debug.Log("before while distance: " + distance);
            while (distance > 1f)
            {
                //Debug.Log(distance > 1f);
                //아 제발.. 아이... 뉴 벡터3(벡터x, 0, 벡터y) 로하니까 당연히 안되지..
                //수정완료
                Vector3 newDirection = new Vector3(directionToWorldUiObject.x, 0f, directionToWorldUiObject.z);
                Vector3 newDelta = newDirection * (currentAreaState.CameraMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position += newDelta;
                yield return null;
                worldUiScreenPosition = new Vector2(worldUiObject.transform.position.x, worldUiObject.transform.position.z);
                cameraScreenPosition = new Vector2(ImmortalCamera.Instance.gameObject.transform.position.x, ImmortalCamera.Instance.gameObject.transform.position.z);
                distance = Vector2.Distance(worldUiScreenPosition, cameraScreenPosition);
                //Debug.Log("enter while distance: " + distance);
            }
            
            /*float centerDistance =  Vector2.Distance(screenCenter, ImmortalCamera.Instance.gameObject.transform.position);
            while (centerDistance > 1f)
            {
                Vector3 centerDirection = (Vector3.zero - ImmortalCamera.Instance.gameObject.transform.position).normalized;
                Vector3 centerDelta = centerDirection * (cameraHeightMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position += centerDelta;
                yield return null;
                centerDistance =  Vector2.Distance(screenCenter, cameraScreenPosition);
            }*/

            IsMoveCam = false;
            currentAreaState.EnterState();
            Debug.Log(currentAreaState.GetType().Name);
            yield break;
        }
        if (ImmortalCamera.Instance.gameObject.transform.position.y < currentAreaState.CameraYPos)
        {
            while (ImmortalCamera.Instance.gameObject.transform.position.y < currentAreaState.CameraYPos)
            {
                Vector3 direction = (worldUiObject.transform.position - ImmortalCamera.Instance.gameObject.transform.position).normalized;
                Vector3 delta = direction * (currentAreaState.CameraMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position -= delta;
                yield return null;    
            }
            
            Vector3 directionToWorldUiObject = (worldUiObject.transform.position - ImmortalCamera.Instance.gameObject.transform.position).normalized;
            Vector2 worldUiScreenPosition = new Vector2(worldUiObject.transform.position.x, worldUiObject.transform.position.z);
            Vector2 cameraScreenPosition = new Vector2(ImmortalCamera.Instance.gameObject.transform.position.x, ImmortalCamera.Instance.gameObject.transform.position.z);
            float distance = Vector2.Distance(worldUiScreenPosition, cameraScreenPosition);
            //Debug.Log("before while distance: " + distance);
            while (distance > 1f)
            {
                //아 제발.. 아이... 뉴 벡터3(벡터x, 0, 벡터y) 로하니까 당연히 안되지..
                //수정완료
                Vector3 newDirection = new Vector3(directionToWorldUiObject.x, 0f, directionToWorldUiObject.z);
                Vector3 newDelta = newDirection * (currentAreaState.CameraMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position += newDelta;
                yield return null;
                worldUiScreenPosition = new Vector2(worldUiObject.transform.position.x, worldUiObject.transform.position.z);
                cameraScreenPosition = new Vector2(ImmortalCamera.Instance.gameObject.transform.position.x, ImmortalCamera.Instance.gameObject.transform.position.z);
                distance = Vector2.Distance(worldUiScreenPosition, cameraScreenPosition);
                //Debug.Log("enter while distance: " + distance);
            }
            
            /*
            float centerDistance =  Vector2.Distance(screenCenter, cameraScreenPosition);
            while (centerDistance > 1f)
            {
                Vector3 centerDirection = (Vector3.zero - new Vector3(ImmortalCamera.Instance.gameObject.transform.position.x, 0f, ImmortalCamera.Instance.gameObject.transform.position.z)).normalized;
                Vector3 centerDelta = centerDirection * (cameraHeightMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position += centerDelta;
                yield return null;
                centerDistance =  Vector2.Distance(screenCenter, cameraScreenPosition);
            }*/
            IsMoveCam = false;
            currentAreaState.EnterState();
        }
    }

    #endregion

    #region OperatorSelect

    
    private void OperatorSelect(Button button)
    {
        //그 버튼에 해당하는 오퍼레이터를 가져와서
        if (friendlyOperatorButtons.TryGetValue(button, out FriendlyOperator friendlyOperator))
        {
            //이미 등록된 오퍼레이터인지 리스트를 확인하고
            if (selectedFriendlyOperators.Contains(friendlyOperator))//등록되어있으면 딕셔너리에서 먼저 지운다. 
            {
                //선택된 오퍼레이터 버튼들에 대해서
                for (int i = 0; i < selectedOperatorButtons.Count; i++)
                {
                    //그 버튼으로 밸류값을 찾아서
                    if (selectedFriendlyOperatorButtons.TryGetValue(selectedOperatorButtons[i], out FriendlyOperator removingOperator))
                    {
                        //그 밸류값들이 서로 일치하면
                        if (friendlyOperator.Equals(removingOperator))
                        {
                            //딕셔너리에서 그 쌍을 지우고
                            selectedFriendlyOperatorButtons.Remove(selectedOperatorButtons[i]);
                            break;
                        }
                    }
                }

                //지운다.
                selectedFriendlyOperators.Remove(friendlyOperator);
                
                //최신화하기
                for (int i = 0; i < selectedOperatorButtons.Count; i++)
                {
                    if (selectedFriendlyOperatorButtons.TryGetValue(selectedOperatorButtons[i], out FriendlyOperator selectedOperator))
                    {
                        selectedOperatorButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = selectedOperator.TrustData.CharacterName;
                        continue;
                    }
                    selectedOperatorButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = "-";

                }
            }
            else
            {
                //선택된 오퍼레이터의 수가 선택된오퍼레이션의 버튼 수보다 적으면
                if (selectedOperatorButtons.Count > selectedFriendlyOperators.Count)
                {
                    selectedFriendlyOperators.Add(friendlyOperator);  
                    for (int i = 0; i < selectedOperatorButtons.Count; i++)
                    {
                        if (i >= selectedFriendlyOperators.Count)
                        {
                            break;
                        }
                        else
                        {
                            selectedFriendlyOperatorButtons[selectedOperatorButtons[i]] = selectedFriendlyOperators[i];
                            //텍스트프로메쉬는 테스트용
                            selectedOperatorButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                                selectedFriendlyOperatorButtons[selectedOperatorButtons[i]].TrustData.CharacterName;    
                        }
                    }    
                }
                else
                {
                    Debug.Log(selectedFriendlyOperatorButtons.Count);
                    return;
                }
            }
        }
        Debug.Log(selectedFriendlyOperatorButtons.Count);
    }

    public void OpenPossibleOperatorPanel()
    {
        if (possibleOperatorPanel.activeInHierarchy)
        {
            return;
        }
        possibleOperatorPanel.SetActive(true);
    }

    public void ClosePossibleOperatorPanel()
    {
        possibleOperatorPanel.SetActive(false);
    }

    #endregion

   
    
    //현재 테스트용
    public void ButtonToCampaignScene()
    {
        SceneImmortalManager.Instance.LoadCampaignScene();
    }

   
}
