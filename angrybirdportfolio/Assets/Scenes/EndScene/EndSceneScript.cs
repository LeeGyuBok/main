using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour
{
    public void OnClickGoMain()
    {
        Debug.Log("GTM Clicked");
        SceneManager.LoadScene("StartScene");
    }
}
