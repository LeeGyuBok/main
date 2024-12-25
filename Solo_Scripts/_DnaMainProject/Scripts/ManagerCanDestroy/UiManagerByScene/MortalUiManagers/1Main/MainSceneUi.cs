using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUi : MortalManager<MainSceneUi>
{
    private Vector2 screenCenter;
    //using UnityEngine.UI;
    /*[SerializeField] private Button testButton;*/
    // Start is called before the first frame update
    void Start()
    {
        ImmortalCamera.Instance.gameObject.transform.position = new Vector3(0f, 30f, 0f);
        ImmortalCamera.Instance.gameObject.transform.rotation = Quaternion.Euler(60, 0, 0);
        
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region mainSceneUi
    public void ButtonToCommanderOfficeScene()
    {
        SceneImmortalManager.Instance.LoadCommanderOfficeScene();
    }
    public void ButtonToOperatorScene()
    {
        SceneImmortalManager.Instance.LoadOperatorScene();
    }
    public void ButtonToLaboratoryScene()
    {
        SceneImmortalManager.Instance.LoadLaboratoryScene();
    }
    public void ButtonToOperationAreaScene()
    {
        //작전 지역씬으로 변경예정
        SceneImmortalManager.Instance.LoadOperationAreaScene();
    }
    public void ButtonToDnaCaptureScene()
    {
        SceneImmortalManager.Instance.LoadDnaCaptureScene();
    }
    public void ButtonToTrainingRoomScene()
    {
        SceneImmortalManager.Instance.LoadTrainingRoomScene();
    }

    public void ExitGame()
    {
        GameImmortalManager.Instance.ExitGame();
    }
    
    #endregion
}
