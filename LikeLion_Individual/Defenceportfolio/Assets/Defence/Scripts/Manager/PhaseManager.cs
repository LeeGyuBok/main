using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    private PhaseStateMachine pSM;
    public PhaseStateMachine PSM
    {
        get { return pSM; }
    }

    public PhaseManager Instance { get; private set; }
    
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        pSM = new PhaseStateMachine(Instance);
    }

    // Start is called before the first frame update
    void Start()
    {
        pSM.Initialize(pSM.state1);
    }

    // Transition is called once per frame
    void Update()
    {
        pSM.Update();
    }
}
