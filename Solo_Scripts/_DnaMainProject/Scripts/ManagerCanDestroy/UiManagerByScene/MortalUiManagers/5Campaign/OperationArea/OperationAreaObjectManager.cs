using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationAreaObjectManager : MortalManager<OperationAreaObjectManager>
{
    private GameObject selectedOperationArea;
    private GameObject createdMap;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelectedOperationArea(OperationAreaInfo selectedOperationAreaButton)
    {
        selectedOperationArea = selectedOperationAreaButton.OperationAreaMapPrefab;
        createdMap = Instantiate(selectedOperationArea, selectedOperationAreaButton.gameObject.transform.position, Quaternion.identity);
        OperationAreaUiManager.Instance.SetMapObject(selectedOperationArea);
    }

    public void InstantiateOperationArea()
    {
        createdMap.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

    public void DestroySelectedOperationArea()
    {
        selectedOperationArea.transform.localScale = Vector3.one;
        DestroyImmediate(createdMap);
        createdMap = null;
        selectedOperationArea = null;
    }
}
