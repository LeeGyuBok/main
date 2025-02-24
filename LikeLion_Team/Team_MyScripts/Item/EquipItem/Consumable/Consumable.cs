using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour, IConsumable
{
    public EquipItem_Consumable ConsumableData { get; private set; }
    
    public void SettingConsumableData(EquipItem_Consumable consumable)
    {
        ConsumableData = consumable;
        data = ConsumableData.data;
    }

    public ConsumableStatus_SO data { get; private set; }
    
    public void Consume()
    {
        if (ConsumableData is Item_SO item)
        {
            if (item.data.ItemCode.Equals(EnumItemCode.EnergyDrink.ToString()))
            {
                Debug.Log(data.RestoreDegree + "restore stamina");
            }
            else if (item.data.ItemCode.Equals(EnumItemCode.HealthRestoreSyringe.ToString()))
            {
                Debug.Log(data.RestoreDegree + "restore health power");
            }
            else if (item.data.ItemCode.Equals(EnumItemCode.InfectionRestoreSyringe.ToString()))
            {
                Debug.Log(data.RestoreDegree + "restore infection");
            }
        }
     
    }
}
