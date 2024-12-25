using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DnaCaptureUiManager : MortalManager<DnaCaptureUiManager>
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestGeneCreator()
    {
        Gene gene = new Gene();
    }
}
