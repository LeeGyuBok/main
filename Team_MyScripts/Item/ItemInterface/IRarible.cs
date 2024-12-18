using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRarible
{
    public EnumItemRarity Rarity { get; }
    
    public EnumItemRarity MaxRarity { get; }

    public EnumItemRarity MinRarity { get; }
    
    public int coefficient { get; }
}
