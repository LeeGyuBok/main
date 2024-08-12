using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BombirdPool : MonoBehaviour
{
    //이 풀은 스스로 모든 걸 다 할 수 있습니다.
    
    //풀에 넣을 프리팹
    [SerializeField] private Bombird m_BombirdPrefab;
    //풀에서 꺼낼 프리팹
    private Bombird m_CurBombird;
    //실제 풀
    private Stack<Bombird> m_BombirdPool;
    //private BirdSpawn m_BirdSpawn;

    //실제 풀 크기
    [SerializeField] private int m_PoolSize;


    void Awake()
    {
        //Debug.Log("여기는 bb 풀 어웨이크");
        SetUpPool();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("여기는 bb 풀 스타트");
        SpawnBombird();
    }

    /*public void SetBirdSpawn(BirdSpawn _v)
    {
        //m_BirdSpawn = _v;
    }*/

    //새를 소환해요


    public void SpawnBombird()
    {
        m_CurBombird = GetPooledBombird();
    }

    //풀을 준비해요
    void SetUpPool()
    {
        //스택을 만들고
        m_BombirdPool = new Stack<Bombird>();
        //Debug.Log("스택!");
        //풀의 크기만큼
        for (int i = 0; i < m_PoolSize; i++)
        {
            //새를 만들어서 
            m_CurBombird = Instantiate(m_BombirdPrefab, transform.position, quaternion.identity);
            //Debug.Log("인스턴스!");
            //프로퍼티할당을 통해 생성된 새에게 돌아갈 집을 알려줘요.
            m_CurBombird.m_BombirdPool = this;
            //Debug.Log("할당!");
            //비활성화해서
            m_CurBombird.gameObject.SetActive(false);
            //Debug.Log("비활성화!");
            //넣어요
            m_BombirdPool.Push(m_CurBombird);
            //Debug.Log("스택에 넣기!");
        }
    }

    //새를 데리고와요
    private Bombird GetPooledBombird()
    {
        //남은 새가 없다면
        if (m_BombirdPool.Count == 0)
        {
            //새로만들어요
            //Debug.Log("스택에 없어서 새로 만들어요");
            m_CurBombird = Instantiate(m_BombirdPrefab, transform.position, quaternion.identity);
            m_CurBombird.m_BombirdPool = this;
            return m_CurBombird;
        }
        //남은새가 있다면
        //Debug.Log("스택에서 꺼내요");
        m_CurBombird = m_BombirdPool.Pop();
        m_CurBombird.gameObject.SetActive(true);
        return m_CurBombird;
    }

    //풀로 돌아가요
    
    public void ReturnToBomPool(Bombird _bombird)
    {
        m_BombirdPool.Push(_bombird);
        //Debug.Log("새를 풀에 넣어요");
        _bombird.gameObject.SetActive(false);
        //Debug.Log("풀에 넣은 새를 재워요");
    }
}
