using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SameToggleClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Toggle _toggle;
    public void OnPointerClick(PointerEventData eventData)
    {
        AuthorityUiManager.Instance.EnterSameToggle(_toggle.isOn);
    }

    public void SetToggle(Toggle toggle)
    {
        if (_toggle == null)
        {
            _toggle = toggle;    
        }
    }
}
