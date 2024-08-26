using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBlockPool : MonoBehaviour
{
    [SerializeField]private PhysicsBlock[] blocksPhysics;

    private Queue<PhysicsBlock> blockPool;
    private int QueueSize = 300;
    private float destroyBlockXposition;
    private float destroyBlockYposition;


    private void Awake()
    {
        blockPool = new Queue<PhysicsBlock>(QueueSize);
        destroyBlockXposition = 0.0f;
        destroyBlockYposition = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnDelay(0.3f));
    }
    
    public void ReturnBlock(PhysicsBlock block)
    {
        //block.transform.position = Vector3.zero;
        block.gameObject.SetActive(false);
        destroyBlockXposition = block.gameObject.transform.position.x;
        destroyBlockYposition = block.gameObject.transform.position.y;
        blockPool.Enqueue(block);
        /*GameManagerPhysicsScript.Instance.GameBlocks.Remove(block.GetInstanceID());//코드리뷰받은 코드*/
        if (blockPool.Count == 0)
        {
            int randomInt = UnityEngine.Random.Range(0, 7);
            PhysicsBlock newBlock = Instantiate(blocksPhysics[randomInt], new Vector3(destroyBlockXposition, 16f, 0), Quaternion.identity);
            //마우스 클릭 이벤트 등록
            newBlock.OnMouseClickBlockPhysics += GameManagerPhysicsScript.Instance.HandleBlockMouseClickPhysics;
            block.DestroyEvent += GameManagerPhysicsScript.Instance.Scoring;    
        }
        else
        { 
           SetBlock();
        }
    }

    private void SetBlock()
    {
        //여기요 최초 생성시에는 풀에 안들어가고 돌아갈때 풀로들어가요
        //인스턴스아이디가 고유한 걸 이용하라는 말씀이시죠? 알겠습니다
        
        PhysicsBlock newBlock = blockPool.Dequeue();
        /*GameManagerPhysicsScript.Instance.GameBlocks.Add(newBlock.GetInstanceID(), newBlock);//코드리뷰받은 코드*/
        newBlock.transform.position = new Vector3(destroyBlockXposition, transform.position.y+destroyBlockYposition*0.5f, 0);
        newBlock.Nullifying();
        newBlock.gameObject.SetActive(true);
    }
    
    private IEnumerator SpawnDelay(float delay)
    {
        // 주어진 시간 동안 대기
        yield return new WaitForSeconds(delay);
        //블럭들이 딱맞게 붙으면 간혹 떨어지지않는 현상이 발생하기때문에 방지하기위한 최소간격
        //매 생성마다 0.02씩이요 일단 그래서 테트리스는 배열로 해보려고요 오오 Array .. 
        //블럭들 일괄동작이요? 
        //어떤 콜리전이요? 저거는 y축이라서 아마 중력때문에 더 내려간거같고 사실 y축으로만 블럭이 
        //자유낙하하기때문에 y는 따로 안줬어요. 떨어질 때 주변에 잇는애들이랑 마찰로 안떨어지는거라서
        float padding = 0.02f;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                float xOffset = i * padding;
                Vector3 blockLocation = new Vector3(i+xOffset+1f, j+1f, 0);
                int randomInt = UnityEngine.Random.Range(0, 7);
                //아래 코드는 풀링을 고려해볼것 <- 기존에는 단순 인스턴시에이트였음
                PhysicsBlock block = Instantiate(blocksPhysics[randomInt], blockLocation, Quaternion.identity);
                //마우스 클릭 이벤트 등록
                block.OnMouseClickBlockPhysics += GameManagerPhysicsScript.Instance.HandleBlockMouseClickPhysics;
                //파괴시 이벤트 등록
                block.DestroyEvent += GameManagerPhysicsScript.Instance.Scoring;
                if (j>9)
                {
                    block.gameObject.SetActive(false);
                    blockPool.Enqueue(block);
                }
                //블럭이 처음에 나타날 때는 풀밖에 나온상태라고 생각을 해서
                //초기에 나타나는 애들은 풀에 안넣었었는데 그거 때문이었나보네요
                //네네 오 알겠습니다.
                /*if (block.gameObject.activeInHierarchy)
                {//코드리뷰받은 코드
                    GameManagerPhysicsScript.Instance.GameBlocks.Add(block.GetInstanceID(), block);
                }*/
                
                
                //blockPool.Enqueue(block);

                //block.isMatch = false;
                //Debug.Log(blockBoard[i,j]);
                //Debug.Log(blockBoard[i,j].tag);
            }
        }
        //Debug.Log(blockPool.Count);
    }
}
