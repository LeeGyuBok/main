using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMapLocationPointer : IObserver, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject myPanel { get; }
    public string currentScene { get; }
}
