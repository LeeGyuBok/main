using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GetChimeraAndGeneUiManager : MortalManager<GetChimeraAndGeneUiManager>, IGoMain
{
    [FormerlySerializedAs("supplyToken")] [SerializeField] private TextMeshProUGUI supplyGeneTokens;
    [FormerlySerializedAs("chimeraToken")] [SerializeField] private TextMeshProUGUI supplyChimeraTokens;
    
    private void Start()
    {
        SetGeneTokensCount();
        SetChimeraTokensCount();
    }

    public void SetGeneTokensCount()
    {
        supplyGeneTokens.text = GameImmortalManager.Instance.GeneSupplyTokenCount.ToString();
    }

    public void SetChimeraTokensCount()
    {
        supplyChimeraTokens.text = GameImmortalManager.Instance.ChimeraSupplyTokenCount.ToString();
    }
    
    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }
}
