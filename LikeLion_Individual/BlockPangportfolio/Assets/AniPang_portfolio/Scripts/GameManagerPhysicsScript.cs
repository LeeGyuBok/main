using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class GameManagerPhysicsScript : MonoBehaviour
{
    [SerializeField]private PhysicsBlock[] blocksPhysics;
    
    //블럭 두개 담을 정적 스택
    private Stack<GameObject> _exchangeStack;
    
    //두 블럭이 동일한지 아닌지 확인할 때 사용할 변수
    private GameObject _block1;
    private GameObject _block2;
    
    public AudioClip confirm;
    public AudioClip denied;

    public static GameManagerPhysicsScript Instance { get; private set; }
    
    public TextMeshProUGUI textScore;
    public Button restart;
    public Button main;

    private int score;

    internal void Scoring(PhysicsBlock block)
    {
        score += 100;
        /*Debug.Log(score.ToString());*/
        textScore.text = "Score: " + $"{score.ToString()}";
    }

    /*public Dictionary<int, PhysicsBlock> GameBlocks = new Dictionary<int, PhysicsBlock>();//코드리뷰받은 코드

    private float currentMovementDelta = 0.0f;
    private float gravityStrength = 9.81f;
    
    private void FixedUpdate()
    {
        bool bRefresh = true;
        
        foreach (var keyValuePair in GameBlocks)
        {
            if (keyValuePair.Value.GetSpeed() >= 0.01f)
            {
                bRefresh = false;
                break;
            }
        }

        if (bRefresh)
        {
            foreach (var keyValuePair in GameBlocks)
            {
                //네네 이해했습니다.
                keyValuePair.Value.CompareTagIfStay_J();
            }
        }
    }*/

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            //DontDestroyOnLoad(gameObject); // 씬이 변경되어도 오브젝트가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하는 경우 현재 오브젝트를 파괴
        }
        
        //스택 초기화
        _exchangeStack = new Stack<GameObject>(2);
        
        
        //블럭들이 딱맞게 붙으면 간혹 떨어지지않는 현상이 발생하기때문에 방지하기위한 최소간격
        /*float padding = 0.02f;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                float xOffset = i * padding;
                Vector3 blockLocation = new Vector3(i+xOffset+1f, j+1f, 0);
                int randomInt = UnityEngine.Random.Range(0, 7);
                //아래 코드는 풀링을 고려해볼것
                PhysicsBlock block = Instantiate(blocksPhysics[randomInt], blockLocation, Quaternion.identity);
                //마우스 클릭 이벤트 등록
                block.OnMouseClickBlockPhysics += HandleBlockMouseClickPhysics;
                //block.isMatch = false;
                //Debug.Log(blockBoard[i,j]);
                //Debug.Log(blockBoard[i,j].tag);
            }
               
        }*/
        score = 0;
        Debug.Log("GM Initialize");

        textScore.gameObject.SetActive(false);
        restart.gameObject.SetActive(false);
        main.gameObject.SetActive(false);
        TimerScript.Instance.TimerEnd += TurnOnScore;
    }

    //블럭을 클릭하면 이 함수가 호출됨
    internal void HandleBlockMouseClickPhysics(PhysicsBlock block)
    {
        //스택의 현재 용량에 따른 스위치케이스문 
        switch (_exchangeStack.Count)
        {
            /*Debug.Log($"나는 {block.name}");*/
            case 0:
                PushFirstBlockToStack(block);
                AudioManagerPhysicsScript.Instance.PlaySound(confirm);
                return;
            case 1:
            {
                ValidateBlocks(block);

                NullifyBlocks();

                break;
            }
            default:
                Debug.Log("Critical Error");
                //강제 스택 초기화
                _exchangeStack.Clear();
                break;
        }
    }
    //첫번째 클릭한 블럭 할당하기
    private void PushFirstBlockToStack(PhysicsBlock block)
    {
        //Debug.Log(_exchangeStack.Count);
        _exchangeStack.Push(block.gameObject);
        _block1 = block.gameObject;
    }
    
    
    //두번째 클릭한 블럭 구분하기
    private void ValidateBlocks(PhysicsBlock block)
    {
        //Debug.Log(_exchangeStack.Count);
        _block2 = block.gameObject;
        
        //할당한 두 블럭이 같으면
        if (_block1.Equals(_block2))
        {
            //Debug.Log("같은 맛이야. 안 먹어.");
            AudioManagerPhysicsScript.Instance.PlaySound(denied);
            _exchangeStack.Clear();
            //Debug.Log(_exchangeStack.Count);
        }
        else//할당한 두 블럭이 서로 다르면
        {
            _exchangeStack.Push(block.gameObject);
            //Debug.Log(_exchangeStack.Count);
            /*
             두 블럭의 위치를 이동시키는 로직
            Debug.Log(_block1.transform.position);
            Debug.Log(_block2.transform.position);
            */
            MoveBlocks();
            //Debug.Log(_exchangeStack.Count);
        }
    }
    
    //할당한 블럭 지우기
    private void NullifyBlocks()
    {
        if (!_block1.IsUnityNull())
        {
            _block1 = null;
            //Debug.Log(_block1.IsUnityNull());
        }

        if (!_block2.IsUnityNull())
        {
            _block2 = null;
            //Debug.Log(_block2.IsUnityNull());
        }
    }
    
    //실질적으로 블럭을 움직이기
    private void MoveBlocks()
    {
        Vector3 isClose = _block2.transform.position - _block1.transform.position;
        if (isClose.magnitude <= 1.1)
        {
            AudioManagerPhysicsScript.Instance.PlaySound(confirm);
            //Debug.Log(isClose);
            //선택된 두 블럭의 위치를 바꾼다.
            _block1.transform.Translate(isClose);
            _block2.transform.Translate(-isClose);
            //Debug.Log(_block1.transform.position);
            //Debug.Log(_block2.transform.position);
            NullifyBlocks();
            //스택 비우기
            _exchangeStack.Clear();
        }
        else
        {
            //Debug.Log(isClose);
            //Debug.Log("너무 멀어~");
            AudioManagerPhysicsScript.Instance.PlaySound(denied);
            NullifyBlocks();
            //스택 비우기
            _exchangeStack.Clear();
        }
    }

    private void TurnOnScore(TimerScript timerScript)
    {
        if (timerScript.Timer <= 0)
        {
            StartCoroutine(UIDelay(0.6f));
        }
    }

    public void ReloadScene()
    {
        // 현재 활성화된 씬의 이름을 가져와서 다시 로드합니다.
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void MainScene()
    {
        Debug.Log("Start Clicked");
        SceneManager.LoadScene("MainScene");
    }

    private IEnumerator UIDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (score == 0)
        {
            textScore.text = "Score: " + $"{score.ToString()}";
        }
        textScore.gameObject.SetActive(true);
        restart.gameObject.SetActive(true);
        main.gameObject.SetActive(true);
    }
}
