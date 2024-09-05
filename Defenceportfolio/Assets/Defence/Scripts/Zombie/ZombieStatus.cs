using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStatus : MonoBehaviour, IHealthPower
{
    public int DMG { get; set; }

    //이베엔트
    
    //좀비 - 나죽어욧
    //좀비풀 - 그럼 이거 실행시켜
    public event Action<GameObject> DeadEvent;
    //유아이 - 이것도 이것도!
    public event Action ScoretoText;
    public event Action<GameObject> MoneytoText;

    //인터페이스
    public virtual int Hp { get; private set; }
    public int MaxHp { get; set; } = 100;

    public int HpIncrement { get; set; } = 10;
    public float Speed { get; set; } = 2.0f;

    public bool Dead { get; private set; }
    
    //물리용 콜라이더
    private CapsuleCollider capsuleCollider;
    
    //애니메이터
    private Animator animator;
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    private Rigidbody rigidbody;
    
    

    private void Awake()
    {
        if (!TryGetComponent(out animator))
        {
            Debug.Log("error");
        }
        if (!TryGetComponent(out capsuleCollider))
        {
            Debug.Log("error");
        }
        if (!TryGetComponent(out rigidbody))
        {
            Debug.Log("error");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ZombieSpawner.Instance.spawnCount % 10 == 0 & ZombieSpawner.Instance.spawnCount != 0)
        {
            Debug.Log(nameof(Elite));
            Elite();
        }
        Hp = MaxHp;
    }

    // Transition is called once per frame
    /*void Update()
    {
        
    }*/

    public void TakeDmg(int dmg)
    {
        Hp -= dmg;
        if (Hp <= 0 && !Dead)
        {
            Die();
        }
    }

    void Die()
    {
        //죽었어요
        Dead = true;
        DeadEvent?.Invoke(gameObject);
        ScoretoText?.Invoke();
        MoneytoText?.Invoke(gameObject);
        animator.SetBool(IsDead, Dead);
        capsuleCollider.enabled = false;
        StartCoroutine(FallDown());
        AnimatorStateInfo dead = animator.GetCurrentAnimatorStateInfo(0);
        Destroy(gameObject, dead.length + 2.8f);
    }
    
    //여기는 10번째 게임오브젝트만이다.
    private void Elite()
    {
        gameObject.transform.localScale += Vector3.one * 0.7f;
        MaxHp += 200;
        Hp = MaxHp;
        Speed += 0.3f;
    }
    
    private void OnApplicationQuit()
    {
        gameObject.transform.localScale = Vector3.one;
        MaxHp = 100;
        Speed = 2.0f;
        Dead = false;
    }

    private IEnumerator FallDown()
    {
        yield return new WaitForSeconds(0.4f);
        rigidbody.isKinematic = true;
    }
}
