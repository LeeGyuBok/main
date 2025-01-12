using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyHitBoxController : MonoBehaviour
{
    [SerializeField] private Vector3 highHitBoxSize;
    [SerializeField] private Vector3 middleHitBoxSize;
    [SerializeField] private Vector3 spinHitBoxSize;
    [SerializeField] private Vector3 upperHitBoxSize;
    [SerializeField] private Vector3 straightHitBoxSize;
    [SerializeField] private Vector3 jabHitBoxSize;
    
    [FormerlySerializedAs("highHitBoxPosition")] [SerializeField] private Transform highHitBox;
    [FormerlySerializedAs("middleHitBoxPosition")] [SerializeField] private Transform middleHitBox;
    [FormerlySerializedAs("spinHitBoxPosition")] [SerializeField] private Transform spinHitBox;
    [FormerlySerializedAs("upperHitBoxPosition")] [SerializeField] private Transform upperHitBox;
    [FormerlySerializedAs("straightHitBoxPosition")] [SerializeField] private Transform straightHitBox;
    [FormerlySerializedAs("jabHitBoxPosition")] [SerializeField] private Transform jabHitBox;

    //private int testint;

    
    private Collider2D[] hitColliders;

    private Enemy _enemy;
    
    public static int PlayerLayerMask { get; private set; }

    private void Awake()
    {
        PlayerLayerMask = LayerMask.GetMask("Player");
        _enemy = gameObject.transform.parent.GetComponent<Enemy>();
    }

    public void High()
    {
        hitColliders = new []{Physics2D.OverlapBox(highHitBox.position, highHitBoxSize, 0f, PlayerLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Player player = hit.gameObject.GetComponent<Player>();
                if (player.HitBoxType != HitBoxType.Airborne)
                {
                    player.HitBoxType = HitBoxType.Stun;
                    //플레이어가 데미지를 입을 수 있는 상태가 아니면 리턴
                    player.StateChangePublic(player.PlayerHitState);
                    HitType.ApplyEffect(player.CharacterRb, _enemy.ChaseState.SightDirection, HitBoxType.Stun);
                }
            }
        }
    }
    
    public void Middle()
    {
        hitColliders = new []{Physics2D.OverlapBox(middleHitBox.position, middleHitBoxSize, 0f, PlayerLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Player player = hit.gameObject.GetComponent<Player>();
                if (player.HitBoxType != HitBoxType.Airborne)
                {
                    player.HitBoxType = HitBoxType.Stiff;
                    //플레이어가 데미지를 입을 수 있는 상태가 아니면 리턴
                    player.StateChangePublic(player.PlayerHitState);
                    HitType.ApplyEffect(player.CharacterRb, _enemy.ChaseState.SightDirection, HitBoxType.Stiff);
                }

        
            }
        }
    }
    
    public void Spin()
    {
        hitColliders = new []{Physics2D.OverlapBox(spinHitBox.position, spinHitBoxSize, 0f, PlayerLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Player player = hit.gameObject.GetComponent<Player>();
                if (player.HitBoxType != HitBoxType.Airborne)
                {
                    player.HitBoxType = HitBoxType.Airborne;
                    //Debug.Log(testint++); 뭐야 ? 여기는 왜 한번불려?
                    //플레이어가 데미지를 입을 수 있는 상태가 아니면 리턴
                    player.StateChangePublic(player.PlayerHitState);
                    HitType.ApplyEffect(player.CharacterRb, _enemy.ChaseState.SightDirection, HitBoxType.Airborne);
                }
              
            }
        }
    }
    
    public void Upper()
    {
        hitColliders = new []{Physics2D.OverlapBox(upperHitBox.position, upperHitBoxSize, 0f, PlayerLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Player player = hit.gameObject.GetComponent<Player>();
                if (player.HitBoxType != HitBoxType.Airborne)
                {
                    player.HitBoxType = HitBoxType.Stun;
                    //플레이어가 데미지를 입을 수 있는 상태가 아니면 리턴
                    player.StateChangePublic(player.PlayerHitState);
                    HitType.ApplyEffect(player.CharacterRb, _enemy.ChaseState.SightDirection, HitBoxType.Stun);
                }
         
            }
        }
    }
    
    public void Straight()
    {
        hitColliders = new []{Physics2D.OverlapBox(straightHitBox.position, straightHitBoxSize, 0f, PlayerLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Player player = hit.gameObject.GetComponent<Player>();
                if (player.HitBoxType != HitBoxType.Airborne)
                {
                    player.HitBoxType = HitBoxType.Stiff;
                    //플레이어가 데미지를 입을 수 있는 상태가 아니면 리턴
                    player.StateChangePublic(player.PlayerHitState);
                    HitType.ApplyEffect(player.CharacterRb, _enemy.ChaseState.SightDirection, HitBoxType.Stiff);
                }
    
            }
        }
    }
    
    public void Jab()
    {
        hitColliders = new []{Physics2D.OverlapBox(jabHitBox.position, jabHitBoxSize, 0f, PlayerLayerMask)};
        if (hitColliders.Length > 0 && hitColliders[0])
        {
            foreach (Collider2D hit in hitColliders)
            {
                Player player = hit.gameObject.GetComponent<Player>();
                if (player.HitBoxType != HitBoxType.Airborne)
                {
                    player.HitBoxType = HitBoxType.Stiff;
                    //플레이어가 데미지를 입을 수 있는 상태가 아니면 리턴
                    player.StateChangePublic(player.PlayerHitState);
                    HitType.ApplyEffect(player.CharacterRb, _enemy.ChaseState.SightDirection, HitBoxType.Stiff);
                }

            }
        }
        
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(highHitBox.position, highHitBoxSize);
        Gizmos.DrawWireCube(middleHitBox.position, middleHitBoxSize);
        Gizmos.DrawWireCube(spinHitBox.position, spinHitBoxSize);
        Gizmos.DrawWireCube(jabHitBox.position, jabHitBoxSize);
        Gizmos.DrawWireCube(straightHitBox.position, straightHitBoxSize);
        Gizmos.DrawWireCube(upperHitBox.position, upperHitBoxSize);
    }
}
