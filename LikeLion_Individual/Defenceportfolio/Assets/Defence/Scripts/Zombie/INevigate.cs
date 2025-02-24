using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INevigate {

    //좀비들이 움직일 방향을 갖고있는 부모오브젝트
    GameObject DirectionFlag_Parent { get; set; }
    //자식오브젝트를 담을 배열(방향표시하는 깃발들)
    GameObject[] DirectionFlag_Children { get; set; }
    //좀비 바라보고있는 방향
    Vector3 CurrentDirectionVector3  {get; set; }
    //바라볼 다음 방향
    Vector3 NextDirectionVector3  {get; set; }
    //방향전환 했어요 변수
    bool Turn { get; set; }
}
