using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrustable
{
    public TrustableData TrustData { get; }
    public int CurrentTrust { get; }
    public void AddTrust(int amount);
    public void RemoveTrust(int amount);
    
    public string MyName()
    {
        return TrustData.CharacterName;
    }

    public Sex MySex()
    {
        return TrustData.Sex;
    }
    
    public int MyAge()
    {
        return TrustData.Age;
    }

    public void SetJsonTrustData(int jsonTrust);
    public void SetInitialTrustData();
}
