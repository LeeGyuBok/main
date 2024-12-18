using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem_Consumable : Item_SO, IConsumable
{
    public EquipItem_Consumable(ItemData_SO data_SO, ConsumableStatus_SO consumableStatusSo) : base(data_SO)
    {
        data = consumableStatusSo;
    }

    public ConsumableStatus_SO data { get; private set; }
    
    public void Consume()
    {
        if (this is Item_SO item)
        {
            if (int.TryParse(item.data.ItemCode, out int itemCode))
            {
                switch (itemCode)
                {
                    case (int)EnumItemCode.EnergyDrink:
                        UiManager.Instance.Player.RestoreStamina((int)data.RestoreDegree);
                        break;
                    case (int)EnumItemCode.HealthRestoreSyringe:
                        UiManager.Instance.Player.RestoreHealthPower((int)data.RestoreDegree);
                        break;
                    case (int)EnumItemCode.InfectionRestoreSyringe:
                        UiManager.Instance.Player.RestoreInfection((int)data.RestoreDegree);
                        break;
                    default:
                        Debug.Log($"{item.data.ItemName}: {data.RestoreDegree}");
                        break;
                }    
            }

            
            
        }
        
    }
}
