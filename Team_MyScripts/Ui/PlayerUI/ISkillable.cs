using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillable
{
    public Liminex liminex { get; }
    public void UseSkill(int keycode);
}
