using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UISide_ImmortalGameManager : ImmortalObject<UISide_ImmortalGameManager>
{
    [SerializeField] private List<FriendlyOperator> characterPrefab;
    public List<FriendlyOperator> CharacterPrefab => characterPrefab;

    private FriendlyOperator selectedOperator;


    public void SelectOperator(FriendlyOperator selectOperator)
    {
        selectedOperator = selectOperator;
    }

    public void DeselectOperator()
    {
        selectedOperator = null;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // PlayMode 종료
#else
                Application.Quit(); // 빌드된 환경에서 게임 종료
#endif    
    }
}
