using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationAreaObjectManager : MortalManager<OperationAreaObjectManager>
{
    private GameObject selectedOperationArea;
    
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
        selectedOperationArea = Instantiate(selectedOperationAreaButton.OperationAreaMapPrefab, selectedOperationAreaButton.gameObject.transform.position, Quaternion.identity);
        
    }

    public void InstantiateOperationArea()
    {
        selectedOperationArea.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

    public void DestroySelectedOperationArea()
    {
        selectedOperationArea.transform.localScale = Vector3.one;
        DestroyImmediate(selectedOperationArea);
        selectedOperationArea = null;
    }
}
