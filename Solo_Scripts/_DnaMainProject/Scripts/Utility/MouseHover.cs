using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//이 스크립트가 붙은 오브젝트에 마우스를 올리면 아래에 정의된 함수가 실행됨
public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
{
    //public bool IsHovered { get; private set; } = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LaboratoryUiManager.Instance)
        {
            LaboratoryUiManager.Instance.ShowSimpleInfo(eventData.pointerEnter.gameObject.GetComponent<Button>());
            return;
        }
        OperatorDetailUiManager.Instance.ShowSimpleInfo(eventData.pointerEnter.gameObject.GetComponent<Button>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (LaboratoryUiManager.Instance)
        {
            LaboratoryUiManager.Instance.HideInfo();
            return;
        }
        OperatorDetailUiManager.Instance.HideInfo();
    }

    //호버된 오브젝트 위에 마우스가 움직이면 얘가 실행
    public void OnPointerMove(PointerEventData eventData)
    {
        if (LaboratoryUiManager.Instance)
        {
            LaboratoryUiManager.Instance.MoveInfo();
            return;
        }
        OperatorDetailUiManager.Instance.MoveInfo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 오른쪽 마우스 버튼 클릭 시
        // eventData.button <- 이벤트를 발생시킨 버튼 == 포인터이벤트데이터 클래스의 인풋버튼이넘의 right
        /*if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 버튼의 onClick 이벤트를 호출
            //Debug.Log(eventData.pointerPress);
            if (eventData.pointerPress.TryGetComponent(out Button button))
            {
                LaboratoryUiManager.Instance.ShowInfoSwitch();
            }
        }*/
    }
}
