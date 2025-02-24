using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Pig : MonoBehaviour
{
    private Rigidbody2D m_PigRigidbody;
    
    private float m_ImpactToDestroy = 2f;

    public delegate void EventHandler(object sender, EventArgs e);

    public event EventHandler DestroyEvent;

    [SerializeField] private GameObject destroyEffect;
    
    private void OnDestroy()
    {
        if (gameObject != null)
        {
            Instantiate(destroyEffect, transform.position, quaternion.identity);
            Debug.Log("이벤트핸들러 발동!");
            DestroyEvent?.Invoke(this, EventArgs.Empty);    
        }
    }

    private void Awake()
    {
        m_PigRigidbody = GetComponent<Rigidbody2D>();
        Debug.Log("리지드바디 장착!");
        m_PigRigidbody.velocity =  new Vector2(0, 0);
        Debug.Log("속박!");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_PigRigidbody.velocity =  new Vector2(0, 0);
        Debug.Log("구속!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Rigidbody2D rb = other.otherRigidbody;
        if (rb != null)
        {
            float impactForce = other.relativeVelocity.magnitude * rb.mass;
            Debug.Log(impactForce);
            // 충격력이 임계값을 초과하면 오브젝트를 파괴합니다.
            if (impactForce > m_ImpactToDestroy)
            {
                Debug.Log("돼지가 파괴된다");
                Destroy(gameObject);
            }
        }
    }
}
