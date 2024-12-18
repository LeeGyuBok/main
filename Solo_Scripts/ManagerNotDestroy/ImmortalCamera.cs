using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImmortalCamera : ImmortalObject<ImmortalCamera>, IObserver
{
    public void DataRenew(string data)
    {
        if (data.Equals("10OperationAreaUi")) 
        {
            //Debug.Log("NeedCameraMove");
            gameObject.AddComponent<CameraMover>();
            gameObject.AddComponent<PhysicsRaycaster>();
        }
        else
        {
            if (gameObject.TryGetComponent(out CameraMover cameraMover))
            {
                //Debug.Log("DestroyImmediate CameraMove");
                DestroyImmediate(cameraMover); 
            }
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneImmortalManager.Instance.RegisterObserver(Instance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
