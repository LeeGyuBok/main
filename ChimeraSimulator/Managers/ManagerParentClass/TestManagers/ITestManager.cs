using UnityEngine;

public interface ITestManager
{
    public ChimeraData PlayerChimeraData { get; }
    public ChimeraData OpponentChimeraData { get; }
    public ChimeraData WinChimeraData { get; }
    public Chimera PlayerChimera { get; }
    public Chimera OpponentChimera { get; }
    public CombatMachine CombatMachine { get; }
    public void WinnerIs(Chimera chimera);
}
