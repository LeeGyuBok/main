using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignUiManager : MortalManager<CampaignUiManager>
{
    [SerializeField] private Button startButton;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SimulationStart()
    {
        Debug.Log("SetActiveObjectScene");
        //SceneImmortalManager.Instance.SetActiveObjectScene();//진짜로 없어도 되네.
        MySecondAStar.Instance.Spawn();
        ImmortalCamera.Instance.gameObject.transform.position = new Vector3(0f, 30f, 0f);
        ImmortalCamera.Instance.gameObject.transform.rotation = Quaternion.Euler(90, 0, -90);
        startButton.gameObject.SetActive(false);
    }
}
