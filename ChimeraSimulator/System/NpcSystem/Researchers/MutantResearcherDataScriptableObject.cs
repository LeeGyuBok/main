using UnityEngine;

[CreateAssetMenu(fileName = "MutantResearcherDataScriptableObject", menuName = "Scriptable Objects/MutantResearcherDataScriptableObject")]
public class MutantResearcherDataScriptableObject : ResearcherDataScriptableObject
{
    [SerializeField] private Chimera rewardMutantChimera;
    public Chimera RewardMutantChimera => rewardMutantChimera;
}
