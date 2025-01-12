using System;
using UnityEditor.Rendering;
using UnityEngine;

public enum HitBoxType
{
    None,//아무것도아님
    Stiff,// idleState로 강제전환
    Stun,// stunState 적용
    Airborne//AirborneState 적용
    //스턴과 에어본에 대한 스테이트를 만들어야하고, 적 정의 및 적의 상태패턴 정의 필요<-= 이거먼저
}

public static class HitType
{
    /// <summary>
    /// Airborne type already add Vector2.up * 3f
    /// </summary>
    /// <param name="target"></param>
    /// <param name="pushDirection">need normalize.</param>
    /// <param name="hitBoxType"></param>
    public static void ApplyEffect(Rigidbody2D target, Vector2 pushDirection, HitBoxType hitBoxType)
    {
        if (pushDirection.x < 0)
        {
            target.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            target.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        switch (hitBoxType)
        {
            case HitBoxType.Stiff:
                pushDirection = Vector2.zero;
                StiffEffect(target, pushDirection);
                break;
            case HitBoxType.Stun:
                pushDirection *= 0.5f;
                StunEffect(target, pushDirection);
                break;
            case HitBoxType.Airborne:
                pushDirection += (Vector2.up * 3f);
                AirborneEffect(target, pushDirection);
                break;
        }
    }
    private static void StiffEffect(Rigidbody2D target, Vector2 direction)
    {
        switch (target.bodyType)
        {
            case RigidbodyType2D.Dynamic://맞은 대상이 플레이어 // 디렉션으 ㄴ들어오는데 ?
                target.AddForce(direction, ForceMode2D.Impulse);
                break;
            case RigidbodyType2D.Kinematic://맞은 대상이 적
                target.MovePosition(target.position + direction);
                break;
            case RigidbodyType2D.Static:
                //벽부수기? 가능?
                break;
        }
        //Debug.Log(target.gameObject.name + " Stiff");
    }
    private static void StunEffect(Rigidbody2D target, Vector2 direction)
    {
        switch (target.bodyType)
        {
            case RigidbodyType2D.Dynamic://맞은 대상이 플레이어
                target.AddForce(direction, ForceMode2D.Impulse);
                break;
            case RigidbodyType2D.Kinematic://맞은 대상이 적
                target.MovePosition(target.position + direction);
                break;
            case RigidbodyType2D.Static:
                //벽부수기? 가능?
                break;
        }
        //Debug.Log(target.gameObject.name + " Stun");
    }
    private static void AirborneEffect(Rigidbody2D target, Vector2 direction)
    {
        switch (target.bodyType)
        {
            case RigidbodyType2D.Dynamic://맞은 대상이 플레이어
                target.AddForce(direction, ForceMode2D.Impulse);
                break;
            case RigidbodyType2D.Kinematic://맞은 대상이 적
                target.MovePosition(target.position + direction);
                break;
            case RigidbodyType2D.Static:
                //벽부수기? 가능?
                break;
        }
        //Debug.Log(target.gameObject.name + " Airborne");
    }
}

public class PlayerHitBoxController : MonoBehaviour
{
    [SerializeField] private Vector3 startAttackHitBoxSize;
    [SerializeField] private Vector3 startMiddleAttackHitBoxSize;
    [SerializeField] private Vector3 middleEndAttackHitBoxSize;
    [SerializeField] private Vector3 endAttackHitBoxSize;
    [SerializeField] private Vector3 groundSlamHitBoxSize;
    
    [SerializeField] private Transform startAttackHitBox;
    [SerializeField] private Transform startMiddleAttackHitBox;
    [SerializeField] private Transform middleEndAttackHitBox;
    [SerializeField] private Transform endAttackHitBox;
    [SerializeField] private Transform groundSlamHitBox;

    private Player _player;

    private Collider2D[] hitColliders;
    
    public static int EnemyLayerMask { get; private set; }
    
    private void Awake()
    {
        EnemyLayerMask = LayerMask.GetMask("Enemy");
        _player = gameObject.transform.parent.GetComponent<Player>();
        if (!_player)
        {
            _player = FindFirstObjectByType<Player>();
        }
    }

    public void StartAttack()
    {
        hitColliders = new []{Physics2D.OverlapBox(startAttackHitBox.position, startAttackHitBoxSize, 0f, EnemyLayerMask)};
        //Debug.Log(startAttackHitBox.transform.position);
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();
                enemy.HitBoxType = HitBoxType.Stiff;
                enemy.StateChangePublic(enemy.EnemyHitState);
                //적의 현재스테이트 받아오기
                HitType.ApplyEffect(enemy.EnemyRb, _player.LookDirection, HitBoxType.Stiff);
            }
        }
    }

    public void StartMiddleAttack()
    {
        hitColliders = new []{Physics2D.OverlapBox(startMiddleAttackHitBox.position, startMiddleAttackHitBoxSize, 0f, EnemyLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();
                enemy.HitBoxType = HitBoxType.Stun;
                enemy.StateChangePublic(enemy.EnemyHitState);
                //적의 현재스테이트 받아오기
                HitType.ApplyEffect(enemy.EnemyRb, _player.LookDirection, HitBoxType.Stun);
            }
        }
    }

    public void MiddleEndAttack()
    {
        hitColliders = new []{Physics2D.OverlapBox(middleEndAttackHitBox.position, middleEndAttackHitBoxSize, 0f, EnemyLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();
                enemy.HitBoxType = HitBoxType.Stiff;
                enemy.StateChangePublic(enemy.EnemyHitState);
                //적의 현재스테이트 받아오기
                HitType.ApplyEffect(enemy.EnemyRb, _player.LookDirection, HitBoxType.Stiff);
            }
        }
    }

    public void EndAttack()
    {
        hitColliders = new []{Physics2D.OverlapBox(endAttackHitBox.position, endAttackHitBoxSize, 0f, EnemyLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();
                enemy.HitBoxType = HitBoxType.Airborne;
                enemy.StateChangePublic(enemy.EnemyHitState);
                //적의 현재스테이트 받아오기
                HitType.ApplyEffect(enemy.EnemyRb, _player.LookDirection, HitBoxType.Airborne);
            }
        }
    }

    public void GroundSlam()
    {
        hitColliders = new []{Physics2D.OverlapBox(groundSlamHitBox.position, groundSlamHitBoxSize, 0f, EnemyLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();
                enemy.StateChangePublic(enemy.EnemyHitState);
                //적의 현재스테이트 받아오기
                HitType.ApplyEffect(enemy.EnemyRb, _player.LookDirection, HitBoxType.Airborne);
            }
        }
    }

    /*public void GroundSlamFlying()
    {
        
    }*/

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(startAttackHitBox.position, startAttackHitBoxSize);
        Gizmos.DrawWireCube(startMiddleAttackHitBox.position, startMiddleAttackHitBoxSize);
        Gizmos.DrawWireCube(middleEndAttackHitBox.position, middleEndAttackHitBoxSize);
        Gizmos.DrawWireCube(endAttackHitBox.position, endAttackHitBoxSize);
        Gizmos.DrawWireCube(groundSlamHitBox.position, groundSlamHitBoxSize);
    }
}
