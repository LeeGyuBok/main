using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlace : MonoBehaviour
{
    private Material originMaterial;

    private Renderer renderer;

    private bool IsEmpty;
    
    public bool Empty => IsEmpty;
    
    // Start is called before the first frame update
    void Awake()
    {
        renderer = GetComponent<Renderer>();
        originMaterial = renderer.material;
        IsEmpty = true;
    }

    private void OnMouseDown()
    {
        if (IsEmpty)
        {
            if (UiManager.Instance.Money >= 60)
            {
                UiManager.Instance.SpawnSoldier();
                AudioManager.Instance.Play("Spawn");
                MyPlayerController.Instance.GetNewCharacter(transform.position + new Vector3(0,1.5f,0),  
                    transform.rotation * Quaternion.Euler(0, 180, 0));
                Debug.Log(Quaternion.identity);
                IsEmpty = false;    
            }
            else
            {
                //부정적인 효과음 출력
            }
        }
    }

    private void OnMouseEnter()
    {
        if (IsEmpty)
        {
            renderer.material = MaterialManager.Instance.outlineMaterial;    
        }
        
    }
    
    private void OnMouseExit()
    {
        renderer.material = originMaterial;
    }
}
