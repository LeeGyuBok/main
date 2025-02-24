using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActiveTime
{
    Start,Middle,End
}

public class SkillSubOption
{
    public int TestInt { get; private set; }
    public ActiveTime ActiveTime { get; private set; }

    public void Activate(string text)
    {
        //로직
        Debug.Log($"{text}: Activate");
    }

    public SkillSubOption(int testInt)
    {
        TestInt = testInt;
    }
}
