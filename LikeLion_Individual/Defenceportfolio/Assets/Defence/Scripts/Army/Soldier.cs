using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Serialization;

public class Soldier : MonoBehaviour
{
    private int bullet;

    private GameObject targetOn;
    private GameObject targetBoss;

    private int dmg = 25;

    private bool IsReloading { get; set; }

    private Animator animator;

    private bool CanAttack { get; set; }
    
    private float cooltime;
    
    [SerializeField] private int _dmg;

    private Queue<GameObject> targetQueue;
    
    private void Awake()
    {
        bullet = 30;
        targetOn = null;
        if (!TryGetComponent(out animator))
        {
            Debug.Log("critical Error: Animation");
        }
        targetQueue = new Queue<GameObject>(30);
        CanAttack = true;
        cooltime = 0.5f;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        dmg += UiManager.Instance.UpgradeCount * 9;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss") && other.gameObject.TryGetComponent(out BossStatus bossStatus))
        {
            if (!bossStatus.Dead)
            {
                Debug.Log("Boss Enter");
                targetBoss = other.gameObject;    
            }
        }
        if (other.CompareTag("Zombie"))
        {
            //콜라이더에 닿는 애들이 좀비이면 큐에 넣는다.
            targetQueue.Enqueue(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //디펜스 게임에서는 공부할게 좀 있다. 어그로 관련 - 맨앞에있는 놈을 우선타격. 나중에는 위험도에 따른 어그로도 관리해보자
        //쿨타임을 어떻게 적용할까
        if (targetBoss != null)
        {
            transform.LookAt(targetBoss.transform.position);
            Shot(targetBoss);
            Debug.Log("boss shot");
            if (other.gameObject.TryGetComponent(out BossStatus bossStatus))
            {
                if (bossStatus.Dead)
                {
                    animator.Play("demo_combat_idle");
                    AudioManager.Instance.Stop("Shot");
                    targetBoss = null;
                }
            }
            return;
        }

        if (other.CompareTag("Zombie"))
        { 
            if (targetQueue.Count > 0)
            {
                targetOn = targetQueue.Peek();
                if (targetOn)
                {
                    transform.LookAt(targetOn.transform.position);
                    Shot(targetOn);    
                }
            }
            else
            {
                return;
            }
            //Debug.Log("zombie shot");
        }

        if (other.gameObject.TryGetComponent(out ZombieStatus zombieStatus))
        {
            if (zombieStatus.Dead)
            {
                animator.Play("demo_combat_idle");
                AudioManager.Instance.Stop("Shot");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            animator.Play("demo_combat_idle");
            AudioManager.Instance.Stop("Shot");
        }
        if (other.gameObject == targetQueue.Peek())
        {
            targetQueue.Dequeue();
            if (targetQueue.Count > 0)
            {
                targetOn = targetQueue.Peek();    
            }
            else
            {
                targetOn = null;
                targetQueue.Clear();
            }
            animator.Play("demo_combat_idle");
            AudioManager.Instance.Stop("Shot");
        }
        /*if (!targetOn)
        {
            targetQueue.Clear();
            targetOn = null;
        }*/
    }

    void Shot(GameObject target)
    {
        //사격조건 - 타겟이 헬스파워 인터페이스를 갖고있고, 리로딩중이 아니고, 어택을 할 수 있다면
        if (target.TryGetComponent(out IHealthPower zombieHealthPower) && !IsReloading && CanAttack)
        {
            //쏜다
            bullet--;
            animator.Play("demo_combat_shoot");
            AudioManager.Instance.Play("Shot");
            zombieHealthPower.TakeDmg(dmg);
            //쐈다. 쿨타임을 적용하자. 언제? 리로딩이 아닐 때
            if (bullet<=0)//리로딩해야해요
            {
                Debug.Log("Reloading");
                AudioManager.Instance.Stop("Shot");
                AudioManager.Instance.Play("Reload");
                animator.Play("demo_combat_idle");
                IsReloading = true;
                StartCoroutine(Reload());
            }
            else//리로딩안해도되요
            {
                StartCoroutine(Cooldown());
            }
        }
        
        if (target.TryGetComponent(out ZombieStatus zombieStatus))
        {
            if (zombieStatus.Dead)
            {
                targetQueue.Dequeue();
                targetOn = null;
            }
        }
        if (target.TryGetComponent(out BossStatus bossStatus))
        {
            if (bossStatus.Dead)
            {
                targetBoss = null;
            }
        }
        /*else
        {
            return;
        }*/
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(3f);
        bullet = 30;
        IsReloading = false;
    }

    private IEnumerator Cooldown()
    {
        //Debug.Log("Cooldown");
        CanAttack = false;
        yield return new WaitForSeconds(cooltime);
        CanAttack = true;
    }

    public void Upgrade()
    {
        dmg += 9;
        Debug.Log(dmg);
    }

    private void LateUpdate()
    {
        if (targetOn.IsDestroyed())
        {
            Debug.Log($"Detect Missing, repair{gameObject.name}, {gameObject.transform.position}");
            Awake();
            animator.Play("demo_combat_idle");
        }
    }
}
