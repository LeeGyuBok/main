using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    //프리팹
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject bossZombiePrefab;

    private ZombieStatus status;

    private BossStatus boss;
    //적 소환 간격
    private float interval;
    
    //좀비가 얼마나 소환이 됐는지. 만약 이 숫자를 모든 싱글톤 객체들이 알아야한다면 그때 스태틱
    public int spawnCount { get; set; }
    
    private int totalSpawnCount { get; set; }
    
    //좀비를 얼마나 소환해야하는지
    public int targetSpawnCount;

    //적 생성 큐
    private Queue<GameObject> zombies;
    
    public static ZombieSpawner Instance { get; private set; }
    
    private bool IsSpawn { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Instance.zombies = new Queue<GameObject>();
        if (!zombiePrefab.TryGetComponent(out status))
        {
            Debug.Log("critical error");
        }
        
        if (!bossZombiePrefab.TryGetComponent(out boss))
        {
            Debug.Log("critical error");
        }
        
        interval = 2f;
        spawnCount = 0;
        totalSpawnCount = spawnCount;
    }

    // Start is called before the first frame update
    /*void Start()
    {
        //호출 반복 / 이 이름의 / 이 시간이 지나고 / 이 시간이 지날때마다
        InvokeRepeating(nameof(SpawnZombie), interval, interval);
    }*/

    // Transition is called once per frame
    void Update()
    {
        
    }

    private void AddZombie(GameObject zombie)
    {
        Instance.zombies.Enqueue(zombie);
    }

    private void RemoveZombie(GameObject zombie)
    {
        //큐에서 첫번째 요소를 제거하지 않고 반환하는게 Peek
        //큐에서 첫번째 요소를 제거하고 (반환하는게) Dequeue
        //큐의 맨 끝에 요소를 추가하는게 Enqueue
        //큐는 피포(FIFO)다
        if (zombies.Count > 0 && zombies.Peek() == zombie)
        {
            Instance.zombies.Dequeue();
        }
    }

    public GameObject GetNextZombie()
    {
        if (zombies.Count > 0)
        {
            return Instance.zombies.Peek();
        }
        return null;
    }

    public void PublicSpawnZombie()
    {
        SpawnZombie();
    }
    private void SpawnZombie()
    {
        if (!IsSpawn)
        {
            GameObject newZombie;

            //풀 고민하기? 풀은 동일한 게임오브젝트를 반복적으로 사용해야할 때 사용한다.
            if (spawnCount % 2 == 0)
            {
                newZombie = Instantiate(zombiePrefab, transform.position + Vector3.left, Quaternion.identity);
                newZombie.SetActive(false);
            }
            else
            {
                newZombie = Instantiate(zombiePrefab, transform.position + Vector3.right, Quaternion.identity);
                newZombie.SetActive(false);
            }
                        
            if (newZombie.TryGetComponent(out ZombieStatus zombie))
            {
                zombie.DeadEvent += Instance.RemoveZombie;
                zombie.ScoretoText += UiManager.Instance.Scoring;
                zombie.MoneytoText += UiManager.Instance.CalculateMoney;
            }

            int mult = totalSpawnCount  / 10;
            zombie.MaxHp += zombie.HpIncrement * mult;
            if (totalSpawnCount % 20 == 0 && interval > 1f)
            {
                interval -= 0.05f;
            }


            newZombie.SetActive(true);
            AddZombie(newZombie);
            spawnCount++;
            totalSpawnCount++;
            StartCoroutine(SpawnDelay());
            //Debug.Log($"spawn: {spawnCount} / queue: {zombies.Count}");
        }
    }

    public GameObject PublicSpawnBoss()
    {
        GameObject boss = SpawnBoss();
        return boss;
    }

    private GameObject SpawnBoss()
    {
        GameObject boss = Instantiate(bossZombiePrefab, transform.position, Quaternion.identity);
        return boss;
    }
    
    private IEnumerator SpawnDelay()
    {
        //Debug.Log("Cooldown");
        IsSpawn = true;
        yield return new WaitForSeconds(interval);
        IsSpawn = false;
    }
}
