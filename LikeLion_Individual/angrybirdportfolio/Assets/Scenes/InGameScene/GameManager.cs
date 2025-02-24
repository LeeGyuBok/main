using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int score;
    
    public TextMeshProUGUI textScore;
    
    [SerializeField] private Pig pigPrefab;

    private Stack<Pig> enemyList;

    [SerializeField] private GameObject endImage;

    private void Awake()
    {
        enemyList = new Stack<Pig>();
    }
    

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        Debug.Log("GM Initialize");
        textScore.text = "Score: " + $"{score.ToString()}";
        
        for (int i = 0; i < 6; i++)
        {
            //스폰포인트를 찾아서 스폰합니다. 근대 이거 에너미 스폰에 있을 필요가있나?
            //게임매니저로 옮겨도 괜찮지 않을까?
            Debug.Log(GameObject.Find($"Spawnpoint{i+1}").transform.position);//위치를 확인했다.
            Pig pigInstance = Instantiate(pigPrefab, GameObject.Find($"Spawnpoint{i+1}").transform.position, quaternion.identity);
            pigInstance.DestroyEvent += HandleADestroyed;
            enemyList.Push(pigInstance);
            Debug.Log(pigInstance + $"{i+1}");    
        }
    }

    void HandleADestroyed(object sender, EventArgs e)
    {
        Pig pig = sender as Pig;
        if (pig != null) {
            pig.DestroyEvent -= HandleADestroyed; // 이벤트 핸들러 해제
        }

        if (textScore != null)
        {
            Debug.Log($"이벤트 수신완료");
            score += 100;
            textScore.text = "Score: " + $"{score.ToString()}";
            textScore.transform.localScale *= 1.1f;
        }

        if (score >= 600)
        {
            StartCoroutine(GotoEndScene());
        }
    }

    IEnumerator GotoEndScene()
    {
        Instantiate(endImage, new Vector3(23, 13, 0), quaternion.identity);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("EndScene");
    }
}
