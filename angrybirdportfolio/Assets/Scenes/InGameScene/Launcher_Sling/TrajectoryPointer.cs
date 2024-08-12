using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPointer : MonoBehaviour
{
    [SerializeField] private GameObject pointerPrefab;

    private int pointerCount = 12;

    public float pointSpacing = 0.1f;

    private static GameObject[] pointers;

    private void Awake()
    {
      
    }

    // Start is called before the first frame update
    void Start()
    {
        // 점 배열 초기화
        pointers = new GameObject[pointerCount];
        Debug.Log(pointers.Length);
        for (int i = 0; i < pointerCount; i++)
        {
            GameObject pointer = Instantiate(pointerPrefab);
            pointer.SetActive(false);
            pointers[i] = pointer;
        }

        Debug.Log("예상 궤적 표시 준비 완료");
        Debug.Log(pointers.Length);     
    }

    public void PublicRenderTrajectory(Vector2 startPoint, Vector2 velocity, float gravityScale)
    {
        Debug.Log(pointers);     
        Debug.Log("프라이빗 렌더러로 갈게");
        RenderTrajectory(startPoint, velocity, gravityScale);
    }
    
    private void RenderTrajectory(Vector2 startPoint, Vector2 velocity, float gravityScale)
    {
        Debug.Log("프라이빗렌더러야");
        Debug.Log(pointers.Length);// 이게 왜 0이야?
        for (int i = 0; i < pointers.Length; i++)
        {
            float t = i * pointSpacing;
            Vector2 pos = CalculatePoint(startPoint, velocity, t, gravityScale);
            pointers[i].transform.position = pos;
            pointers[i].SetActive(true);
        }
    }
    
    private Vector2 CalculatePoint(Vector2 startPoint, Vector2 velocity, float t, float gravityScale)
    {
        float g = Mathf.Abs(gravityScale * Physics2D.gravity.y); // 중력 가속도
        Debug.Log(gravityScale);
        
        float x = startPoint.x + velocity.x * t;
        float y = startPoint.y + velocity.y * t - 0.5f * g * t * t;
        return new Vector2(x, y);
    }

    public void HideTrajectory()
    {
        foreach (var point in pointers)
        {
            point.SetActive(false); // 점을 비활성화
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
