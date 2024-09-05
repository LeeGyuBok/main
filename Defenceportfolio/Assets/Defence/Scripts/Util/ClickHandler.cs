using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //메인카메라태그가붙은 카메라로부터 ray를 쏜다. 스크린의 특정 포인트까지, 그리고 그포인트는(인풋.마우스포지션)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // "IgnoreRaycast" 레이어를 무시하는 레이어 마스크 생성
            int layerMask = 1 << LayerMask.NameToLayer("IgnoreRaycast");
            layerMask = ~layerMask; // 반전하여 "IgnoreRaycast" 레이어만 무시하도록 설정

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // 클릭된 오브젝트 처리
                Debug.Log("Clicked on " + hit.collider.name);
                // 여기서 오브젝트 B의 특정 동작을 수행하도록 추가 코드 작성 가능
            }
        }
    }
}
