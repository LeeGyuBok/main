using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OperationAreaUiManager : MortalManager<OperationAreaUiManager>
{
    [SerializeField] private List<WorldUiObject> worldContinentButtons;
    public List<WorldUiObject> WorldContinentButtons => worldContinentButtons;
    [SerializeField] private List<WorldUiObject> continentAreaButtons;
    public List<WorldUiObject> ContinentAreaButtons => continentAreaButtons;
    [SerializeField] private List<WorldUiObject> operationAreaButtons;
    public List<WorldUiObject> OperationAreaButtons => operationAreaButtons;
    
    [SerializeField] private GameObject worldContinentButtonsGameObject;
    public GameObject WorldContinentButtonsGameObject => worldContinentButtonsGameObject;
    [SerializeField] private GameObject continentAreaButtonsGameObject;
    public GameObject ContinentAreaButtonsGameObject => continentAreaButtonsGameObject;
    [SerializeField] private GameObject operationAreaButtonsGameObject;
    public GameObject OperationAreaButtonsGameObject => operationAreaButtonsGameObject;

    [SerializeField] private GameObject operationAreaInfoPanel;

    
    [SerializeField] private float cameraHeightMoveSpeed;
    
    private WorldMap worldMap;
    private ContinentMap continentMap;
    private OperationAreaMap operationAreaMap;

    private Dictionary<WorldUiObject, IAreaState> worldUiObjectsByButton = new();
    private Vector2 screenCenter;
    public IAreaState currentAreaState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        worldMap = new WorldMap();
        continentMap = new ContinentMap();
        operationAreaMap = new OperationAreaMap();
    }

    // Start is called before the first frame update
    void Start()
    {
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

        currentAreaState = worldMap;
        currentAreaState.EnterState();
        
        ImmortalCamera.Instance.gameObject.transform.position = new Vector3(0f, currentAreaState.CameraYPos, 0f);
        ImmortalCamera.Instance.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

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
    
    public void ButtonToCampaignScene()
    {
        SceneImmortalManager.Instance.LoadCampaignScene();
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
        StartCoroutine(CameraMove(worldUiObject.gameObject));
    }

    private IEnumerator CameraMove(GameObject worldUiObject)
    {
        Debug.Log(ImmortalCamera.Instance.gameObject.transform.position.y > currentAreaState.CameraYPos);
        if (ImmortalCamera.Instance.gameObject.transform.position.y > currentAreaState.CameraYPos)
        {
            //카메라가 현재 지역상태의 정해진 카메라위치값보다 크면 == 작아져야하면
            while (ImmortalCamera.Instance.gameObject.transform.position.y > currentAreaState.CameraYPos)
            {
                /*버튼에 가까이 갈때 -> y값을 내린다. 이 때, 버튼의 '방향'으로도 움직여야한다. 둘을 따로 움직여도될거같은데 y값 먼저 맞추고 x,z값 맞추기.*/
                Vector3 direction = (worldUiObject.transform.position - ImmortalCamera.Instance.gameObject.transform.position).normalized;
                Vector3 delta = direction * (cameraHeightMoveSpeed * Time.fixedDeltaTime);
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
                //아 제발.. 아이... 뉴 벡터3(벡터x, 0, 벡터y) 로하니까 당연히 안되지..
                //수정완료
                Vector3 newDirection = new Vector3(directionToWorldUiObject.x, 0f, directionToWorldUiObject.z);
                Vector3 newDelta = newDirection * (cameraHeightMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position += newDelta;
                yield return null;
                worldUiScreenPosition = new Vector2(worldUiObject.transform.position.x, worldUiObject.transform.position.z);
                cameraScreenPosition = new Vector2(ImmortalCamera.Instance.gameObject.transform.position.x, ImmortalCamera.Instance.gameObject.transform.position.z);
                distance = Vector2.Distance(worldUiScreenPosition, cameraScreenPosition);
                //Debug.Log("enter while distance: " + distance);
            }
            currentAreaState.EnterState();
            yield break;
        }
        if (ImmortalCamera.Instance.gameObject.transform.position.y < currentAreaState.CameraYPos)
        {
            while (ImmortalCamera.Instance.gameObject.transform.position.y < currentAreaState.CameraYPos)
            {
                Vector3 direction = (worldUiObject.transform.position - ImmortalCamera.Instance.gameObject.transform.position).normalized;
                Vector3 delta = direction * (cameraHeightMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position -= delta;
                yield return null;    
            }
            
            Vector3 directionToWorldUiObject = (worldUiObject.transform.position - ImmortalCamera.Instance.gameObject.transform.position).normalized;
            Vector2 worldUiScreenPosition = new Vector2(worldUiObject.transform.position.x, worldUiObject.transform.position.z);
            Vector2 cameraScreenPosition = new Vector2(ImmortalCamera.Instance.gameObject.transform.position.x, ImmortalCamera.Instance.gameObject.transform.position.z);
            float distance = Vector2.Distance(worldUiScreenPosition, cameraScreenPosition);
            Debug.Log("before while distance: " + distance);
            while (distance > 1f)
            {
                //아 제발.. 아이... 뉴 벡터3(벡터x, 0, 벡터y) 로하니까 당연히 안되지..
                //수정완료
                Vector3 newDirection = new Vector3(directionToWorldUiObject.x, 0f, directionToWorldUiObject.z);
                Vector3 newDelta = newDirection * (cameraHeightMoveSpeed * Time.fixedDeltaTime);
                ImmortalCamera.Instance.gameObject.transform.position += newDelta;
                yield return null;
                worldUiScreenPosition = new Vector2(worldUiObject.transform.position.x, worldUiObject.transform.position.z);
                cameraScreenPosition = new Vector2(ImmortalCamera.Instance.gameObject.transform.position.x, ImmortalCamera.Instance.gameObject.transform.position.z);
                distance = Vector2.Distance(worldUiScreenPosition, cameraScreenPosition);
                Debug.Log("enter while distance: " + distance);
            }
            currentAreaState.EnterState();
        }
    }
}
