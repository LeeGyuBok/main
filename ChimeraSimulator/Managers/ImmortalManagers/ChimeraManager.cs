using System.Collections.Generic;
using UnityEngine;

public class ChimeraManager : ImmortalObject<ChimeraManager>, IDataReset
{
    public List<ChimeraData> MyChimeraDatas { get; private set; }
    public List<Chimera> DevelopmentableChimeras { get; private set; }
    //public List<Chimera> MyChimeraDatas { get; private set; }
    
    public ChimeraData TestChimeraData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        DevelopmentableChimeras = new List<Chimera>(GameImmortalManager.MyChimerasCapacity);
        MyChimeraDatas = new List<ChimeraData>(GameImmortalManager.MyChimerasCapacity);

    }

    public bool AddNewMyChimera(ChimeraData chimera)
    {
        if (MyChimeraDatas.Count < GameImmortalManager.MyChimerasCapacity)
        {
            MyChimeraDatas.Add(chimera);
            return true;
        }
        //Debug.Log("MyChimeraDatas Container is Full");
        return false;
    }

    public void RemoveDeadChimera(ChimeraData chimera)
    {
        MyChimeraDatas.Remove(chimera);
    }

    public bool ReallocateChimera(ChimeraData chimera)
    {
        if (MyChimeraDatas.Contains(chimera))
        {
            MyChimeraDatas.Remove(chimera);
            return true;
        }
        //Debug.Log("ChimeraData already removed");
        return false;
    }

    public bool AddDevelopmentableChimera(Chimera chimera)
    {
        if (!DevelopmentableChimeras.Contains(chimera))
        {
            DevelopmentableChimeras.Add(chimera);
            return true;
        }
        //Debug.Log("chimera already assigned");
        return false;
    }

    public void SetTestChimeraData(ChimeraData chimera)
    {
        TestChimeraData = chimera;
    }

    public void DataReset()
    {
        DevelopmentableChimeras.Clear();
        MyChimeraDatas.Clear();
    }
}
