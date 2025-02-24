using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChimeraData", menuName = "Scriptable Objects/ChimeraData")]
public class ChimeraData : ScriptableObject
{ 
    public Chimera Chimera { get; private set; }
    public FeatureList FeatureInfo { get; private set; }
    public BaseStatus BaseStatus { get; private set; }
    
    private bool _initialized = false;
    private bool _setCoefficient = false;

    private Vector3 _cagedSpawnPoint = new (4f, -0.5f, -2f);
    private Vector3 _cagedSpawnRotate = new (0f, -140f, 0f);
    
    public MainDna MainDna { get; private set; }
    public DnaMainSkill MainSkill { get; private set; }
    public List<DnaSubSkill> SubSkills { get; private set; }
    
    public float MaxHealthPoint { get; private set; }
    public float CurrentHealthPoint { get; private set; }
    public float AttackPoint { get; private set; }
    public float DefencePoint { get; private set; }
    public float AgilityPoint { get; private set; }

    /// <summary>
    /// Initialize MainDna, MainSkill, SubSkills
    /// </summary>
    /// <param name="chimera"></param>
    /// <param name="featureInfo"></param>
    /// <param name="baseStatus"></param>
    public void SetChimeraData(Chimera chimera, FeatureList featureInfo, BaseStatus baseStatus)
    {
        if (!_initialized)
        {
            if (chimera == null) return;
            if (featureInfo == null) return;
            if (baseStatus == null) return;
            Chimera = chimera;
            FeatureInfo = featureInfo;
            BaseStatus = baseStatus;

            MainDna = new MainDna(chimera.GeneType);
            MainSkill = MainDna.DnaMainSkill;
            SubSkills = MainDna.DnaSubSkills;

            MaxHealthPoint = BaseStatus.MaxHealthPoint;
            CurrentHealthPoint = MaxHealthPoint;
            AttackPoint = BaseStatus.AttackPoint;
            DefencePoint = BaseStatus.DefencePoint;
            AgilityPoint = BaseStatus.AgilityPoint;

            _initialized = true;
        }
    }

    public void SetCoefficientToStatus()
    {
        if (!_setCoefficient)
        {
            MaxHealthPoint += MaxHealthPoint * MainDna.TotalHealthCoefficient/100;
            CurrentHealthPoint = MaxHealthPoint;
            AttackPoint += AttackPoint * MainDna.TotalAttackCoefficient/100;
            DefencePoint += DefencePoint * MainDna.TotalDefenceCoefficient/100;
            AgilityPoint += AgilityPoint * MainDna.TotalAgilityCoefficient/100;
            _setCoefficient = true;
        }
    
    }

    public Chimera CagedInstantiateChimera()
    {
        Chimera chimera = Instantiate(Chimera, _cagedSpawnPoint, Quaternion.Euler(_cagedSpawnRotate));
        chimera.Initialize(this);
        return chimera;
    }
    
    public Chimera InstantiateChimera(Vector3 spawnPoint = default)
    {
        Chimera chimera;
        if (spawnPoint == default)
        {
            chimera = Instantiate(Chimera, Vector3.zero, Quaternion.identity);    
        }
        else
        {
            chimera = Instantiate(Chimera, spawnPoint, Quaternion.identity);    
        }
        
        chimera.Initialize(this);
        return chimera;
    }
}
