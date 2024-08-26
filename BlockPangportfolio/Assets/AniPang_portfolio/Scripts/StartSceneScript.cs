using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;//<- 씬을 사용하기 위해 이게 필요해요

public class StartSceneScript : MonoBehaviour
{
    public void OnClickStartButton()
    {
        Debug.Log("Start Clicked");
        SceneManager.LoadScene("AnipangScene");
    }
    
    public void OnClickExitButton()
    {
        Debug.Log("Exit Clicked");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Play 모드를 종료합니다.
#else
        Application.Quit(); // 빌드된 게임에서는 게임을 종료합니다.
#endif
    }
}
