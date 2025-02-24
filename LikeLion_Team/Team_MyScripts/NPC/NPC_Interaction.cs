using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NPC_Interaction : Interaction
{
    //아이콘? 필요? 할지도? -> 그냥 AI로 박제하면안되나 ㅋㅋㅋ
    //영어이름, 한국어이름, 엔피씨코드, 한국어설명, 나이, 성별
    [SerializeField] private NPCData_SO dataSo;
    //친밀도!
    public int Friendship { get; private set; }
    
    //유저 인식 범위
    private SphereCollider detectCollider;
    
    //유저를 향해 바라볼 때 속도
    [SerializeField] private int turnSpeed = 200;
    
    //유저를 향해 도는중인지 아닌지
    private bool TurningToPlayer { get; set; } = false;
    
    /// <summary>
    /// 유저 향해 돌기까지 딜레이, float 
    /// </summary>
    [SerializeField]private float turnDelay;
    
    /// <summary>
    /// 대화창사라지는 딜레이, float 
    /// </summary>
    [SerializeField]private float dialogDelay;

    private bool OnCoroutine;

    private void Awake()
    {
        detectCollider = GetComponent<SphereCollider>();


        detectCollider.isTrigger = true;
        detectCollider.center = new Vector3(0f, 1.75f, 0f);
        detectCollider.radius = 2.5f;
    }
    
    private void Start()
    {
        AssignInteractionButton();
    }

    protected override void Update()
    {
        //켜져있는동안 입력막기
        if (!UiManager.Instance.FunctionSelectWindow.activeInHierarchy)
        {
            base.Update();    
        }
    }

    protected override void DoInteraction()
    {
        Friendship += 10;
        //실제 인게임 코드 수정하는 부분
        //UiManager.Instance.dialog.text = dataSo.KoreanDetail;
        if (OnCoroutine)
        {
            return;
        }
        //이름 셋팅
        GameObject npcNameObject = UiManager.Instance.Dialog.transform.GetChild(0).gameObject;

        if (npcNameObject.TryGetComponent(out TextMeshProUGUI npcName))
        {
            npcName.text = $"{dataSo.Name}";
        }
        else
        {
            Debug.Log("set TextMeshProUGUI");
            return;
        }
        
        //메시지 셋팅
        GameObject npcMessageObject = UiManager.Instance.Dialog.transform.GetChild(1).gameObject;
        if (npcMessageObject.TryGetComponent(out TextMeshProUGUI npcMessage))
        {
            if (Friendship > 10)//첫만남이 아닌경우
            {
                npcMessage.text = dataSo.DialogData[1];
            }
            else//첫만남인경우
            {
                npcMessage.text = dataSo.DialogData[0];
            }
        }
        else
        {
            Debug.Log("set TextMeshProUGUI");
            return;
        }
        StartCoroutine(DialogDelay());
        if (Friendship > 20)
        {
            return;
        }
        if (gameObject.TryGetComponent(out NPC_Itemcontributor itemContributor))
        {
            List<ItemData_SO> list = itemContributor.RangeWeaponMaterialItems;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (int.TryParse(list[i].ItemCode, out int itemCode))
                    {
                        Item_SO item = ItemManager_SO.Instance.GetItem(itemCode);
                        Inventory.Instance.PublicItemGotoInventory(item);
                    }
                    else
                    {
                        Debug.Log("itemsetError");    
                    }
                }
            }

            List<ItemData_SO> list2 = itemContributor.MeleeWeaponMaterialItems;
            if (list2.Count > 0)
            {
                for (int i = 0; i < list2.Count; i++)
                {
                    if (int.TryParse(list2[i].ItemCode, out int itemCode))
                    {
                        Item_SO item = ItemManager_SO.Instance.GetItem(itemCode);
                        Inventory.Instance.PublicItemGotoInventory(item);
                    }
                    else
                    {
                        Debug.Log("itemsetError");
                    }
                }
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!UiManager.Instance.FunctionSelectWindow.activeInHierarchy)
        {
            base.OnTriggerEnter(other);
        }
        //플레이어가 시야범위에 들어왔으니 도세요
        TurningToPlayer = true;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        TurningToPlayer = false;
        StopCoroutine(nameof(DialogDelay));
        //Debug.Log("stop cor");
        UiManager.Instance.DialogBox.SetActive(false);
        UiManager.Instance.Player.gameObject.GetComponent<CursorState>().SetCursor(true, false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //넵
            LookPlayer(other.gameObject);
        }
    }

    private void LookPlayer(GameObject character)
    {
        if (TurningToPlayer)
        {
            Vector3 lookDirection = (character.transform.position - gameObject.transform.position).normalized;
            //계산
            Vector3 rotateTowards = Vector3.RotateTowards(gameObject.transform.forward, lookDirection,
                turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1000f);
            //돌았습니다.
            gameObject.transform.forward = rotateTowards == Vector3.zero ? Vector3.forward : rotateTowards;
            //정확하게 바라보고있다면 대기
            if (gameObject.transform.forward.normalized == lookDirection)
            {
                StartCoroutine(LookDelay());    
            }
        }
    }
    //코루틴은, 자신을 포함하고 있는 블록에 아무런 영향을 끼치지 못한다. 오직 코루틴 내부에서, 일드리턴전과 후에 작성한 코드만 작동한다.
    private IEnumerator LookDelay()
    {
        if (TurningToPlayer)
        {
            TurningToPlayer = false;
        }
        yield return new WaitForSeconds(turnDelay);
        TurningToPlayer = true;
    }

    private IEnumerator DialogDelay()
    {
        OnCoroutine = true;
        //기능창이 있으면
        if (dataSo.HaveFunction)
        {
            //다이어로그와 펑션의 부모오브젝트 켜고
            UiManager.Instance.DialogBox.gameObject.SetActive(true);
            //펑션잠깐끄고
            UiManager.Instance.OffFunctionSelectWindow();
            //다이어로그는 켜고
            DialogOnOff(true);
            //다이어로그 읽는동안 기다렸다가
            yield return new WaitForSeconds(dialogDelay);
            //다이어로그 끄고
            DialogOnOff(false);
            //펑션켜고
            UiManager.Instance.OnFunctionSelectWindow();
            UiManager.Instance.Player.gameObject.GetComponent<CursorState>().SetCursor(false, true);
        }
        else//기능창없으면
        {
            //다이어로그와 펑션의 부모오브젝트 켜고
            UiManager.Instance.DialogBox.gameObject.SetActive(true);
            //펑션끄고
            UiManager.Instance.OffFunctionSelectWindow();   
            //다이어로그는 켜고
            DialogOnOff(true);
            //읽는 동안 기다렸ㅆ다가
            yield return new WaitForSeconds(dialogDelay);
            //다이어로그 끄고
            DialogOnOff(false);
            //혹시 펑션 켜져있을지도 모르니끄고
            UiManager.Instance.OffFunctionSelectWindow();    
        }
        OnCoroutine = false;
    }

    private void DialogOnOff(bool turn)
    {
        UiManager.Instance.DialogBox.transform.GetChild(0).gameObject.SetActive(turn);
    }
}
