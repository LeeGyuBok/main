using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(BoxCollider))]
public class Ladder_interaction : Interaction
{
    //친밀도!
    public bool IsUsing { get; private set; }

    private BoxCollider contactCollider;

    private GameObject interactionPoint;

    private void Awake()
    {
        contactCollider = GetComponent<BoxCollider>();
        contactCollider.isTrigger = true;
        contactCollider.center += new Vector3(0, 0, -0.25f);
        contactCollider.size = gameObject.GetComponent<MeshFilter>().mesh.bounds.size + new Vector3(0.25f, 0.25f, 0.25f);
        interactionPoint = transform.GetChild(0).gameObject;
    }
    
    private void Start()
    {
        AssignInteractionButton();
    }

    //f버튼을 눌러서 상호작용중이다.
    protected override void DoInteraction()
    {
        IsUsing = true;
        //자식오브젝트에서 부모오브젝트로 접근하기
        GameObject player = ContactObject.transform.parent.gameObject;
        player.TryGetComponent(out Rigidbody limitMove);
        
        if (CompareTag("Ladder"))
        {
            if (IsUsing)
            {
                player.transform.position = interactionPoint.transform.position;
                limitMove.constraints = (RigidbodyConstraints)114;
                if (player.transform.position.z < -1)
                {
                    IsUsing = false;
                }
            }
            limitMove.constraints = RigidbodyConstraints.FreezeRotation;
        }

        Debug.Log(IsUsing);
    }
}
