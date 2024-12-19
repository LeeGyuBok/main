using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private float moveSpeed = 5f; // 카메라 이동 속도
    //private float rotationSpeed = 100f; // 카메라 회전 속도
    private float screenEdgeThreshold = 50f; // 화면 가장자리의 임계값 (픽셀 단위)
    //private float screenEdgeLimit = 10f;
    private LayerMask mapLayer;
    private Camera myCamera;

    private Vector2 screenCenter;
    private Vector2 widthPosition;
    private Vector2 heightPosition;

    private GameObject map;
    private float mouseToCenterDistance;

    void Update()
    {
        CameraMovement();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Input.mousePosition);
        mapLayer = LayerMask.GetMask("Map");
        myCamera = gameObject.GetComponent<Camera>();
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        map = GameObject.FindGameObjectWithTag("Map");
    }

    private void CameraMovement()
    {
        if (OperationAreaUiManager.Instance.currentAreaState.GetType() == typeof(OperationAreaDetail) || OperationAreaUiManager.Instance.IsMoveCam)
        {
            return;
        }
        Vector2 mousePosition = Input.mousePosition;
        mouseToCenterDistance = Vector2.Distance(screenCenter, mousePosition);
        Vector2 mainDirection = Vector2.zero;

        widthPosition = new Vector2(mousePosition.x, mainDirection.y);

        Vector3 mainDirectionVector = Vector3.zero;

        // 화면 가장자리에 있어야 그에 따른 이동 방향 설정
        if (mousePosition.x <= screenEdgeThreshold)
        {
            mainDirection = mousePosition - screenCenter;
        }

        if (mousePosition.x >= Screen.width - screenEdgeThreshold)
        {
            mainDirection = mousePosition - screenCenter;
        }

        if (mousePosition.y <= screenEdgeThreshold)
        {
            mainDirection = mousePosition - screenCenter;
        }

        if (mousePosition.y >= Screen.height - screenEdgeThreshold)
        {
            mainDirection = mousePosition - screenCenter;
        }

        mainDirectionVector = new Vector3(mainDirection.x, 0, mainDirection.y).normalized;

        if (mainDirectionVector != Vector3.zero)
        {
            Vector3 nextPosition = transform.position + mainDirectionVector * (moveSpeed * Time.fixedDeltaTime);

            //Debug.DrawRay(nextPosition, Vector3.down * 20.1f, Color.blue);
            
            // 이동 가능 여부 확인
            if (Physics.Raycast(nextPosition, Vector3.down, out RaycastHit hitInfo,transform.position.y + 1f,mapLayer))
            {
                /*Debug.DrawRay(hitInfo.point, new Vector3(mainDirectionVector.x, 0f, 0f), Color.yellow, 3f);
                Debug.DrawRay(hitInfo.point, new Vector3(0f, 0f, mainDirectionVector.z), Color.green, 3f);
                
                Debug.DrawRay(transform.position, new Vector3(mainDirectionVector.x, -21f, 0f), Color.yellow, 3f);
                Debug.DrawRay(transform.position, new Vector3(0f, -21f, mainDirectionVector.z), Color.green, 3f);*/
                
                transform.position = nextPosition;
            }
            else
            {
                widthPosition = new Vector3(mainDirectionVector.x, -(transform.position.y + 1f), 0f);
                if(Physics.Raycast(transform.position, new Vector3(mainDirectionVector.x, -(transform.position.y + 1f), 0f),transform.position.y + 2f,mapLayer))
                {
                    Vector3 widthDirection = new Vector3(mainDirectionVector.x, 0f, 0f).normalized;
                    nextPosition = transform.position + widthDirection * (moveSpeed * Time.fixedDeltaTime);
                    transform.position = nextPosition;
                    return;
                }

                heightPosition = new Vector3(0f, -21f, mainDirectionVector.z);
                if(Physics.Raycast(transform.position, new Vector3(0f, -(transform.position.y + 1f), mainDirectionVector.z),transform.position.y + 2f,mapLayer))
                {
                    Vector3 heightDirection = new Vector3(0f, 0f, mainDirectionVector.z).normalized;
                    nextPosition = transform.position + heightDirection * (moveSpeed * Time.fixedDeltaTime);
                    transform.position = nextPosition;
                }
                
            }
            
            
        }
        /*// 카메라 회전
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, -mouseY * rotationSpeed * Time.deltaTime, Space.Self);*/
    }
}

