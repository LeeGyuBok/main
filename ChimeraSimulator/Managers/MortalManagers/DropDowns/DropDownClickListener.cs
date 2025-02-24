using UnityEngine;
using UnityEngine.EventSystems;

public class DropDownClickListener : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AuthorityUiManager.Instance.TurnOffMyGeneListContent();
    }
}
