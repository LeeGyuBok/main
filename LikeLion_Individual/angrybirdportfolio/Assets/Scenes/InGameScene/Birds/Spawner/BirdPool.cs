using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BirdPool : MonoBehaviour
{
    //이 풀은 스스로 모든 걸 다 할 수 있습니다.
    
    //풀에 넣을 프리팹
    [SerializeField] private Bird m_BirdPrefab;
    
    //풀에서 꺼낸 프리팹
    private Bird m_Curbird;
    
    //실제 풀
    private Stack<Bird> m_BirdPool;
    
    //private BirdSpawn m_BirdSpawn;

    //실제 풀 크기
    [SerializeField] private int m_PoolSize;


    void Awake()
    {
        //Debug.Log("여기는 b 풀 어웨이크");
        SetUpPool();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("여기는 b 풀 스타트");
        SpawnBird();
    }

    /*public void SetBirdSpawn(BirdSpawn _v)
    {
        //m_BirdSpawn = _v;
    }*/

    //새를 소환해요
    public void SpawnBird()
    {
        //m_BirdSpawn.SpawnBird();
        m_Curbird = GetPooledBird();
    }
    

    //풀을 준비해요
    void SetUpPool()
    {
        //스택을 만들고
        m_BirdPool = new Stack<Bird>();
        //풀의 크기만큼
        for (int i = 0; i < m_PoolSize; i++)
        {
            //새를 만들어서 
            Bird bird = Instantiate(m_BirdPrefab, transform.position, quaternion.identity);
            //프로퍼티할당을 통해 생성된 새에게 돌아갈 집을 알려줘요.
            bird.m_Pool = this;
            //비활성화해서
            bird.gameObject.SetActive(false);
            //넣어요
            m_BirdPool.Push(bird);
        }
    }

    //새를 데리고와요
    private Bird GetPooledBird()
    {
        //남은 새가 없다면
        if (m_BirdPool.Count == 0)
        {
            //새로만들어요
            //Debug.Log("스택에 없어서 새로 만들어요");
            m_Curbird = Instantiate(m_BirdPrefab, transform.position, quaternion.identity);
            m_Curbird.m_Pool = this;
            return m_Curbird;
        }
        //남은새가 있다면
        //Debug.Log("스택에서 꺼내요");
        m_Curbird = m_BirdPool.Pop();
        m_Curbird.gameObject.SetActive(true);
        return m_Curbird;
    }

    //풀로 돌아가요
    public void ReturnToPool(Bird _bird)
    {
        m_BirdPool.Push(_bird);
        //Debug.Log("새를 풀에 넣어요");
        _bird.gameObject.SetActive(false);
        //Debug.Log("풀에 넣은 새를 재워요");
    }
}
