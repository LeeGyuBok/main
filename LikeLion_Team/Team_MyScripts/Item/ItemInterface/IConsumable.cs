using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConsumable
{
    public ConsumableStatus_SO data { get; }
    public void Consume();
}
