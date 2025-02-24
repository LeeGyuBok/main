using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseRightClickHandler: MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(eventData.button);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ManagingChimeraSceneUiManager.Instance.ShowReallocationPanel(eventData.pointerClick.GetComponent<Button>());
        }
    }

    public void Hi()
    {
        
    }
}
