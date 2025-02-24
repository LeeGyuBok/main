using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    protected GameObject interactionButton;
    protected GameObject ContactObject { get; set; }
    protected bool PlayerContact { get; private set; }

    private bool CanAction { get; set; } = true;

    private float coolTime = 4f;

    private bool onCoolTime;

    protected void AssignInteractionButton()
    {
        interactionButton = UiManager.Instance.UiDictionary[0];
    }

    /// <summary>
    /// On the Button only
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            ContactObject = other.gameObject;
            PlayerContact = true;
            UiManager.Instance.PlayerContact = PlayerContact;
            /*Debug.Log("contact");*/
            if (CanAction)
            {
                interactionButton.SetActive(PlayerContact);    
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        /*if (OnCoolTime)
        {
            return;
        }*/
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerContact = false;
            UiManager.Instance.PlayerContact = PlayerContact;
            interactionButton.SetActive(PlayerContact);
            ContactObject = null;
        }
    }

    protected virtual void Update()
    {
        if (PlayerContact && !onCoolTime)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (CanAction)
                {
                    //어떤 오브젝트와의 상호작용인지에 대한 오버라이드 필요
                    DoInteraction();
                    StartCoroutine(Cooldown());    
                }
            }
        }
    }
    
    private IEnumerator Cooldown()
    {
        //Debug.Log("Cooldown");
        //코루틴은 일드리턴 구문까지 실행하다가 일드리턴 구문에서 지정된 시간동안 멈추고 다시 진행한다.
        //현재는 오브젝트마다 쿨타임이 다르게 흐른다.
        CanAction = false;//쿨타임동안 나를 상호작용할 수 없어요
        onCoolTime = true;//자 쿨타임 돌아요
        interactionButton.SetActive(CanAction);//네, 버튼끌게요
        yield return new WaitForSeconds(coolTime);//쿨타임 시작해요 -> cooltime 동안 아래 구문이 실행되지 않다가
        onCoolTime = false;//아 쿨타임 다 됐어요
        CanAction = true;//쿨타임 끝났으니 나 상호작용할 수 있어요
        
        //혹시 플레이어가 닿아있나요? ->contact
        if (PlayerContact)//네 닿아있네요
        {
            //하지만 기능선택중이에요
            if (UiManager.Instance.FunctionSelectWindow.activeInHierarchy)
            {
                interactionButton.SetActive(!CanAction);//버튼 켜지마세요    
            }
            else//기능선택중이지 않아요
            {
                interactionButton.SetActive(CanAction);//버튼 켜세요
            }

            
            
            
        }
        //안 닿아있어요. 지나갈게요
    }

    protected virtual void DoInteraction()
    {
        
    }
}
