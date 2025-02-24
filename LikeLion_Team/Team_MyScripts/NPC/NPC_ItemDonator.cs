using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Itemcontributor : MonoBehaviour
{
    [SerializeField] private List<ItemData_SO> rangeWeaponMaterialItems;
    [SerializeField] private List<ItemData_SO> meleeWeaponMaterialItems;

    public List<ItemData_SO> RangeWeaponMaterialItems
    {
        get => rangeWeaponMaterialItems;
        private set => rangeWeaponMaterialItems = value;
    }

    public List<ItemData_SO> MeleeWeaponMaterialItems
    {
        get => meleeWeaponMaterialItems;
        private set => meleeWeaponMaterialItems = value;
    }

    private void Awake()
    {
        RangeWeaponMaterialItems = rangeWeaponMaterialItems;
        MeleeWeaponMaterialItems = meleeWeaponMaterialItems;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < rangeWeaponMaterialItems.Count; i++)
        {
            Debug.Log(rangeWeaponMaterialItems[i].ItemName);
        }

        for (int i = 0; i < meleeWeaponMaterialItems.Count; i++)
        {
            Debug.Log(meleeWeaponMaterialItems[i].name);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
