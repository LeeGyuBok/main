using UnityEngine;

[CreateAssetMenu(fileName = "BaseStatus", menuName = "Scriptable Objects/BaseStatus")]
public class BaseStatus : ScriptableObject
{
    [SerializeField] private int maxHealthPoint;
    public int MaxHealthPoint => maxHealthPoint;
    [SerializeField] private int attackPoint;
    public int AttackPoint => attackPoint;
    [SerializeField] private int defencePoint;
    public int DefencePoint => defencePoint;
    [SerializeField] private int agilityPoint;
    public int AgilityPoint => agilityPoint;
    
    
}
