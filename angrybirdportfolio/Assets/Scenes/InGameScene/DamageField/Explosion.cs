using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float m_ExplosionForce; // 폭발력
    [SerializeField] private float m_ExplosionRadius; // 폭발 반경
    private AudioSource m_BoomSound;
    private Collider2D m_BoomCollider;

    public void Awake()
    {
        m_BoomSound = GetComponent<AudioSource>();
        m_BoomCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        m_BoomSound.Play();
        Destroy(gameObject, 1.6f);
    }

    //부딪쳤을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        //힘을가하고
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = rb.position - (Vector2)transform.position;
            float distance = direction.magnitude;
            float force = m_ExplosionForce * (1 - distance / m_ExplosionRadius);
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            //없앤다.
        }
        //콜라이더끄기
        m_BoomCollider.enabled = false;
    }
}
