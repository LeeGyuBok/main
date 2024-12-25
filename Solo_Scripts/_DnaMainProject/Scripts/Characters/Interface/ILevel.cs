using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevel
{
    //레벨
    public int Level{ get; }
    //현재경험치
    public int CurrentExperiencePoints { get; }
    //요구경험치
    public int RequireExperiencePoints { get; }
    //레벨업당 요구 경험치
    public Dictionary<int, int> LevelUpExperience { get; }
    //경험치 셋팅 함수
    public void SetExperience(int experience);
    //레벨업 함수
    public void LevelUp();
    //스크립터블 오브젝트로부터 딕셔너리가져오고 초기값 세팅하기
    public void SetDictionaryAndInitialize();

}
