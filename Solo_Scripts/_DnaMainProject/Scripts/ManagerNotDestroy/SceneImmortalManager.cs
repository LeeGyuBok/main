using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneImmortalManager : ImmortalObject<SceneImmortalManager>, ISubject
{
    [SerializeField] private GameObject sceneEventSystem;
    
    [SerializeField] private List<string> uiSceneNameList; // < <string>
    [SerializeField] private List<string> objectSceneNameList; // < <string>

    /// <summary>
    /// objectScene by UiScene
    /// </summary>
    private Dictionary<string, string> sceneNameDictionary;
    
    public List<IObserver> observers = new List<IObserver>();
    
    public void RegisterObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void UnregisterObserver(IObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);    
        }
    }

    public void NotifyObservers(string sceneName)
    {
        //순차적으로 검사한다. 만약 첫번째 조건에서 들어맞으면 if문 내부로 들어간다.
        //생각해보니까 모두 쟤네들이 아니어야된다... 난 멍청했다..
        //UiScene을 넘겨준다
        //이 부분. 약간 문제가 있음. 그러니까.. 이것을 사용해서 기능을 추가할 때 약간 문제가있음.
        if (!sceneName.Equals(uiSceneNameList[0]) && !sceneName.Equals(uiSceneNameList[1]) && !sceneName.Equals(uiSceneNameList[7]))
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].DataRenew(sceneName);
            }
        }
        //카메라에 붙은 카메라무브 컴포넌트 지우기위한 용도..
        if (sceneName.Equals(uiSceneNameList[0]) || sceneName.Equals(uiSceneNameList[1]) || sceneName.Equals(uiSceneNameList[7]))
        {
            ImmortalCamera.Instance.DataRenew(sceneName);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        if (uiSceneNameList.Count == objectSceneNameList.Count)
        {
            sceneNameDictionary = new Dictionary<string, string>();
            for (int i = 0; i < uiSceneNameList.Count; i++)
            {
                sceneNameDictionary[uiSceneNameList[i]] = objectSceneNameList[i];
            }
        }
        if (sceneEventSystem == null)
        {
            sceneEventSystem = FindFirstObjectByType<EventSystem>().gameObject;
        }
        
        sceneEventSystem.SetActive(true);
        NotifyObservers(SceneManager.GetActiveScene().name);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals(uiSceneNameList[0]) && SceneManager.sceneCount < 2)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNameDictionary[uiSceneNameList[0]], LoadSceneMode.Additive);
            if (operation == null)
            {
                Debug.Log("No Scene");
                return;
            }
            //와일문은 함부로넣는게 아니다.
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(uiSceneNameList[0]));
        }
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    //어디티브를 활용한, 내가 짠 코드.
    #region Not Use
    /*
    public void LoadCommanderOfficeScene()
    {
        //현재 활성화된 씬 확인
        Scene scene = SceneManager.GetActiveScene();
        
        //씬 전환하기전에, 현재 메인 씬인지 확인
        if (scene.name.Equals(mainUiSceneName))
        {
            //메인씬이면 누른 버튼이 켜줄 씬을 로드
            SetActiveScene(commanderOffice);
            //로드가 끝나면 활성화되었던(이전) 씬을 언로드
            SetSceneUnload(scene);
        }
    }

    //홉버튼에 넣을것
    public void LoadMainScene()
    {
        //현져 활성화된 씬 확인
        Scene scene = SceneManager.GetActiveScene();
        //Async가 붙어 비동기처리 - 조금 늦게 완료 == 충돌가능성 증가
        //메인이면 리턴
        if (scene.name.Equals(mainUiSceneName))
        {
            return;
        }
        //메인씬이 아니면 메인씬을 먼저 켜고
        SetActiveScene(mainUiSceneName);   
        //씬 끄기
        SetSceneUnload(scene);
    }
    
    private void LoadAdditiveScene(string sceneName)
    {
        //내가 하고싶은건 메인씬에서 특정 씬들을 로드했다가 끄고싶어.
        //현재 활성화된 씬이름이 로드하려는 씬의 이름과 같으면 함수종료
        if (SceneManager.GetActiveScene().name.Equals(sceneName))
        {
            return;
        }
        //현재 활성화된 씬이름이 로드하려는 씬의 이름과 다르면 로드
        //이거 좀 신박한데? 단, 이때 추가되는 씬은 그 뭐냐, 카메라를 꺼줘야한다.
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    
    private void SetActiveScene(string sceneName) 
    {
        //LoadAdditiveScene(sceneName);//위의 씬과 동일. 게다가 이 경우, 씬이 즉시 로드되는게 아니라 시간이 걸림 -> 로딩화면이 필요한 이유
        StartCoroutine(SetActiveSceneAsync(sceneName));
    }

    private void SetSceneUnload(Scene scene)
    {
        StartCoroutine(UnloadSceneAsync(scene));
    }
    
    private IEnumerator SetActiveSceneAsync(string sceneName)
    {
        //씬을 어디티브로 로드한다. 이때, 씬은 즉시 로드되지 않으므로 기다려야한다.
        LoadAdditiveScene(sceneName);
        sceneEventSystem.SetActive(false);
        sceneEventSystem = null;
        // 씬 로드 완료를 기다림
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield return null; // 다음 프레임으로 대기
        }

        // 로드 완료 후 활성화
        completeSceneload = true;
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        sceneEventSystem = FindFirstObjectByType<EventSystem>().gameObject;
        sceneEventSystem.SetActive(true);
        //Debug.Log($"Scene {sceneName} is now active.");
    }

    private IEnumerator UnloadSceneAsync(Scene scene)
    {
        while (!completeSceneload)
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync(scene);
        completeSceneload = false;
    }
    */

    #endregion
    
    
    //짜다보니 이거 어디티브 안해도될거같아서 gpt가 짠 코드. 근데 이녀석도 어디티브쓰네?
    public void LoadCommanderOfficeScene()
    {
        //현재 씬 가져와서
        //오전 3:11 2024-11-25 - 이거 씬이 뭔지 알 필요 있음? 없는거같은데?
        Scene scene = SceneManager.GetActiveScene();
        //커맨더오피스 씬을 로드한다.
        SetActiveScene(uiSceneNameList[1], scene);
    }
    
    public void LoadOperatorScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[2], scene);
    }
    
    public void LoadLaboratoryScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[3], scene);
    }
    
    public void LoadCampaignScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[4], scene);
    }
    
    public void LoadDnaCaptureScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[5], scene);
    }
    
    public void LoadTrainingRoomScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[6], scene);
    }

    public void LoadMainScene()
    {
        //현재 씬 가져와서 - 이건 필요한듯 ㅋㅋ
        Scene scene = SceneManager.GetActiveScene();
        //메인씬이면 리턴하고
        if (scene.name.Equals(uiSceneNameList[0]))
        {
            return;
        }
        //메인씬이아니면 메인씬을 로드하고 현재 씬을 끈다.
        //액티베이트씬(활성화된 씬)은 마지막에 활성화된 씬이다.
        //여기서 활성화된 씬은 UI씬이다.
        SetActiveScene(uiSceneNameList[0], scene);
    }
    
    public void LoadInterviewScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[7], scene);
    }
    
    public void LoadOperatorDetailScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[8], scene);
    }
    
    public void LoadOperationAreaScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[9], scene);
    }
    
    private void SetActiveScene(string loadingSceneName, Scene unloadingScene)
    {
        StartCoroutine(SetActiveSceneAsync(loadingSceneName, unloadingScene));
    }

    public void SetActiveObjectScene()
    {
        //내가 짠 코드
        /*Scene scene = SceneManager.GetActiveScene();
        if (uiSceneNameList.Contains(scene.name))//지금 활성화된 씬이 ui씬이다.
        {
            Scene objectScene = SceneManager.GetSceneByName(sceneNameDictionary[scene.name]);
            SceneManager.SetActiveScene(objectScene);
            return;
        }
        if (objectSceneNameList.Contains(scene.name))//지금 활성화된 씬이 오브젝트씬이다.
        {
            int index = uiSceneNameList.IndexOf(scene.name);
            Scene uiScene = SceneManager.GetSceneByName(uiSceneNameList[index]);
            SceneManager.SetActiveScene(uiScene);
        }*/
        
        //gpt가 짠코드
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log(scene.name);
        // 현재 활성화된 씬이 UI 씬인지 확인
        if (uiSceneNameList.Contains(scene.name))
        {
            if (sceneNameDictionary.TryGetValue(scene.name, out string objectSceneName))
            {
                Scene objectScene = SceneManager.GetSceneByName(objectSceneName);
                SceneManager.SetActiveScene(objectScene);
            }
            return;
        }

        // 현재 활성화된 씬이 오브젝트 씬인지 확인
        if (objectSceneNameList.Contains(scene.name))
        {
            int index = uiSceneNameList.IndexOf(scene.name);
            if (index != -1) // 유효한 인덱스인지 확인
            {
                Scene uiScene = SceneManager.GetSceneByName(uiSceneNameList[index]);
                SceneManager.SetActiveScene(uiScene);
            }
        }
    }

    private IEnumerator SetActiveSceneAsync(string loadingSceneName, Scene unloadingScene)
    {
        // 비동기 로드 시작- 어싱크로오레이션은 현재 씬이 얼마나 로드되었는지 알려주는 객체다
        AsyncOperation uiSceneOperation = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        
        //사용자의 입력 막기
        sceneEventSystem.SetActive(false);
        sceneEventSystem = null;

        //널 체크
        if (uiSceneOperation == null)
        {
            Debug.LogError($"Failed to load scene: {loadingSceneName}. LoadOperation returned null.");
            yield break; // 더 이상 진행하지 않음
        }
        
        // 로드 완료를 기다림
        while (!uiSceneOperation.isDone)
        {
            yield return null;
        }
        
        //Debug.Log(unloadingScene.name);
        AsyncOperation objectSceneOperation = SceneManager.LoadSceneAsync(sceneNameDictionary[loadingSceneName], LoadSceneMode.Additive);
        //널 체크
        if (objectSceneOperation == null)
        {
            Debug.LogError($"Failed to load scene: {sceneNameDictionary[loadingSceneName]}. LoadOperation returned null.");
            yield break; // 더 이상 진행하지 않음
        }
        //Debug.Log(unloadingScene.name);
        // 로드 완료를 기다림
        while (!objectSceneOperation.isDone)
        {
            yield return null;
        }
        
        //Debug.Log(unloadingScene.name);
        // 새 씬 활성화
        Scene uiScene = SceneManager.GetSceneByName(loadingSceneName);
        SceneManager.SetActiveScene(uiScene);

        // EventSystem 재설정
        sceneEventSystem = FindFirstObjectByType<EventSystem>().gameObject;
        sceneEventSystem.SetActive(true);
        //Debug.Log(SceneManager.GetActiveScene().name);

        NotifyObservers(loadingSceneName);
        Debug.Log(unloadingScene.name);
        // 이전 씬 언로드
        StartCoroutine(UnloadSceneAsync(unloadingScene));
    }

    private IEnumerator UnloadSceneAsync(Scene scene)
    {
        string objectSceneName = sceneNameDictionary[scene.name];
        
        AsyncOperation unloadObjectSceneOperation = SceneManager.UnloadSceneAsync(objectSceneName);
        
        if (unloadObjectSceneOperation == null)
        {
            Debug.LogError($"Failed to load scene: {objectSceneName}. LoadOperation returned null.");
            yield break; // 더 이상 진행하지 않음
        }
        
        while (!unloadObjectSceneOperation.isDone)
        {
            yield return null;
        }
        
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(scene);
        
        if (unloadOperation == null)
        {
            Debug.LogError($"Failed to load scene: {scene.name}. LoadOperation returned null.");
            yield break; // 더 이상 진행하지 않음
        }
        
        // 언로드 완료를 기다림
        while (!unloadOperation.isDone)
        {
            yield return null;
        }
    }


}
    

