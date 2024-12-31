using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UISide_ImmortalSceneManager : ImmortalObject<UISide_ImmortalSceneManager>
{
    [SerializeField] private GameObject sceneEventSystem;
    
    [SerializeField] private List<string> uiSceneNameList; // < <string>
    [SerializeField] private List<string> objectSceneNameList; // < <string>
    
    private Dictionary<string, string> sceneNameDictionary;
    
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

    public void LoadMainScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[0], scene);
    }
    
    public void LoadGameScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[1], scene);
    }
    
    public void LoadArchiveScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[2], scene);
    }
    
    public void LoadSelectQuestionScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[3], scene);
    }
    
    public void LoadReceiveAnswerScene()
    {
        //현재 씬 가져와서
        Scene scene = SceneManager.GetActiveScene();
        SetActiveScene(uiSceneNameList[4], scene);
    }
    
    
    private void SetActiveScene(string loadingSceneName, Scene unloadingScene)
    {
        StartCoroutine(SetActiveSceneAsync(loadingSceneName, unloadingScene));
    }
    
    
    public void SetActiveObjectScene()
    {
        //내가 짠 코드
        Scene scene = SceneManager.GetActiveScene();
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
        }
        
        //gpt가 짠코드
        /*Scene scene = SceneManager.GetActiveScene();
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
        }*/
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
