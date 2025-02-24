using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

//immortal들 씬따로<- 항상 켜져있음.
//이벤트 시스템은 새롭게 로드되는 additive씬의 것을 계속 재 할당하면서 사용
public class ProjectSceneManager : ImmortalObject<ProjectSceneManager>
{
    [SerializeField] private EventSystem sceneEventSystem;
    
    [SerializeField] private List<string> additiveSceneNameList;

    [SerializeField] private GameObject loadingSceneObject;
    [SerializeField] private TextMeshProUGUI loadingSceneAnnouncement;
    [SerializeField] private List<string> loadingSceneAnnouncementList;
    
    private Scene currentScene;
    public string CurrentSceneName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        sceneEventSystem = FindFirstObjectByType<EventSystem>();
        if (!sceneEventSystem)
        {
            sceneEventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
        }
        currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(additiveSceneNameList[11], LoadSceneMode.Additive);
        CurrentSceneName = additiveSceneNameList[11];
    
    }

    private void Start()
    {
        CurrentSceneName = currentScene.name;
        loadingSceneObject.SetActive(false);
        if (!currentScene.name.Equals(additiveSceneNameList[11]))
        {
            currentScene = SceneManager.GetSceneByName(additiveSceneNameList[11]);
            SceneManager.SetActiveScene(currentScene);
        }
    }

    public void CallAdditiveScene(int index)
    {
        //Debug.Log(additiveSceneNameList[index]);
        SetAdditiveScene(additiveSceneNameList[index]);
        int randomIndex = UnityEngine.Random.Range(0, loadingSceneAnnouncementList.Count);
        loadingSceneAnnouncement.text = loadingSceneAnnouncementList[randomIndex];
        CameraManager.Instance.SetPosition(additiveSceneNameList[index]);
        BackgroundMusicManager.Instance.PlayMusic(index);
    }

    private void SetAdditiveScene(string sceneName)
    {
        currentScene = SceneManager.GetActiveScene();
        CurrentSceneName = currentScene.name;
        if (additiveSceneNameList.Contains(sceneName))
        {
            StartCoroutine(SetActiveSceneAsync(sceneName, currentScene));
            //SceneManager.SetActiveScene(additiveScene);
        }
    }

    private IEnumerator SetActiveSceneAsync(string additiveSceneName, Scene unloadingScene)
    {
        
        loadingSceneObject.SetActive(true);//<- 로딩용 캔버스
        
        // 이전 씬 언로드
        yield return StartCoroutine(UnloadSceneAsync(unloadingScene));//언로딩을 먼저해야해요!
        
        // 비동기 로드 시작- 어싱크로오레이션은 현재 씬이 얼마나 로드되었는지 알려주는 객체다
        AsyncOperation uiSceneOperation = SceneManager.LoadSceneAsync(additiveSceneName, LoadSceneMode.Additive);
        
        //사용자의 입력 막기
        sceneEventSystem.gameObject.SetActive(false);
        //sceneEventSystem = null;

        //널 체크
        if (uiSceneOperation == null)
        {
            //Debug.LogError($"Failed to load scene: {additiveSceneName}. LoadOperation returned null.");
            //아마 메인 씬으로 돌아가는 로직이 필요할 듯
            yield break; // 더 이상 진행하지 않음
        }
        
        
        //20250211 로딩씬 구현 부분. 90퍼센트에서 멈춰요.
        //새롭게 켤 씬의 로딩을 잠깐 멈춰요
        uiSceneOperation.allowSceneActivation = false;
        
        float timer = 0f;
        // 로드 완료를 기다림
        while (!uiSceneOperation.isDone)
        {
            yield return null;//<- 20250211 유니티에게 제어권 넘기기?
            if (uiSceneOperation.progress < 0.9f)//0.9까지로드해요
            {
                //막대기 채우기. 0.9까지
            }
            else//이후에 1.9초동안 나머지 로드해요
            {
                timer += Time.unscaledDeltaTime;
                //막대기 남은 부분 채우기. 나머지 0.1을 n초에 걸쳐서
                if (timer >= 2.4f)
                {
                    uiSceneOperation.allowSceneActivation = true;
                }
            }
        }
        
        //Debug.Log(unloadingScene.name);
        // 새 씬 활성화
        Scene additiveScene = SceneManager.GetSceneByName(additiveSceneName);
        SceneManager.SetActiveScene(additiveScene);

        currentScene = SceneManager.GetActiveScene();
        CurrentSceneName = currentScene.name;
        
        // EventSystem 재설정
        //sceneEventSystem = FindFirstObjectByType<EventSystem>();
        sceneEventSystem.gameObject.SetActive(true);
        //Debug.Log(SceneManager.GetActiveScene().name);

        //NotifyObservers(additiveSceneName);
        loadingSceneObject.SetActive(false);
    }

    private IEnumerator UnloadSceneAsync(Scene unloadScene)
    {
        string unloadSceneName = unloadScene.name;
        //Debug.Log(unloadSceneName);
        AsyncOperation unloadObjectSceneOperation = SceneManager.UnloadSceneAsync(unloadSceneName);
        
        if (unloadObjectSceneOperation == null)
        {
            //Debug.LogError($"Failed to load unloadScene: {unloadSceneName}. LoadOperation returned null.");
            //메인씬으로 돌아가는 로직이 필요할수도
            yield break; // 더 이상 진행하지 않음
        }
        
        while (!unloadObjectSceneOperation.isDone)
        {
            yield return null;
        }
        //Debug.Log($"Unloading {unloadSceneName} completed.");
    }

    
}
