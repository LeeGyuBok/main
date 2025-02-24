using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatMachine
{
    private Chimera _playerChimera;
    private Chimera _opponentChimera;
    
    private Chimera _attacker;
    private Chimera _defender;
    
    private List<IChimeraState> _playerChimeraStates;
    private List<IChimeraState> _opponentChimeraStates;
    
    private Dictionary<Chimera, List<IChimeraState>> _combatChimerasStatesDictionary;
    private Dictionary<Chimera, int> _combatChimerasInstinctPointsDictionary;
    
    ITestManager _testManager;

    private int _attackerCoefficient;
    private int _defenderCoefficient;

    private bool _endCombat = false;
    private bool _switchAttacker = false;

    private int _encounterCount = 0;
    
    

    public CombatMachine(Chimera player, Chimera opponent, ITestManager testManager)
    {
        _playerChimera = player;
        _opponentChimera = opponent;
        _testManager = testManager;
        
        _playerChimeraStates= new List<IChimeraState>();
        _opponentChimeraStates = new List<IChimeraState>();
        
        _combatChimerasStatesDictionary = new Dictionary<Chimera, List<IChimeraState>>()
        {
            {_playerChimera, _playerChimeraStates},
            {_opponentChimera, _opponentChimeraStates}
        };
        _combatChimerasInstinctPointsDictionary = new Dictionary<Chimera, int>()
        {
            { _playerChimera, 20 },
            { _opponentChimera, 20 }
        };
    }

    /// <summary>
    /// officialTest Only
    /// </summary>
    /// <param name="player"></param>
    /// <param name="opponent"></param>
    public void RestartCombat(Chimera player, Chimera opponent)
    {
        _endCombat = false;
        _encounterCount = 0;
        _playerChimera = player;
        _opponentChimera = opponent;

        _playerChimeraStates ??= new List<IChimeraState>();
        _playerChimeraStates.Clear();
        _opponentChimeraStates ??= new List<IChimeraState>();
        _opponentChimeraStates.Clear();

        _combatChimerasStatesDictionary ??= new Dictionary<Chimera, List<IChimeraState>>();
        _combatChimerasStatesDictionary.Clear();
        _combatChimerasStatesDictionary = new Dictionary<Chimera, List<IChimeraState>>()
        {
            {_playerChimera, _playerChimeraStates},
            {_opponentChimera, _opponentChimeraStates}
        };
        _combatChimerasInstinctPointsDictionary ??= new Dictionary<Chimera, int>();
        _combatChimerasInstinctPointsDictionary.Clear();
        _combatChimerasInstinctPointsDictionary = new Dictionary<Chimera, int>()
        {
            { _playerChimera, 20 },
            { _opponentChimera, 20 }
        };
    }

    /// <summary>
    /// first is player's, second is researcher's
    /// </summary>
    /// <returns></returns>
    public (List<IChimeraState>, List<IChimeraState>) GetChimeraStatesChain()
    {
        Chaining();
        for (int i = 0; i < _playerChimeraStates.Count; i++)
        {
            switch (_testManager)
            {
                case OfficialTestManager:
                    OfficialTestUiManager.Instance.AddTestLog($"{i}. {_playerChimeraStates[i].StateName}");   
                    break;
                case NonOfficialTestManager:
                    NonOfficialTestUiManager.Instance.AddTestLog($"{i}. {_playerChimeraStates[i].StateName}");  
                    break;
            }
        }

        /*for (int i = 0; i < _opponentChimeraStates.Count; i++)
        {
            Debug.Log($"{i}. {_opponentChimeraStates[i].GetType()} / Cost: {_combatChimerasInstinctPointsDictionary[_opponentChimera]}");
        }*/
        
        return (_playerChimeraStates, _opponentChimeraStates);
    }
    
    private void Chaining()
    {
        //선공권 결정 후 공격시도
        SetFirstStates(); //<- 리스트의 최초 스테이트를 채움.(공격, 취약 또는 공격, 회피. 회피시 공 수 교대)
        while (!_endCombat)
        {
            Encounter(_attacker, _defender);
            _encounterCount++;
            /*Debug.Log($"{_encounterCount}:  P: {_playerChimeraStates[^1].GetType()}/{_combatChimerasInstinctPointsDictionary[_playerChimera]}/{_playerChimera.CurrentHealthPoint}   " +
                      $"O: {_opponentChimeraStates[^1].GetType()}/{_combatChimerasInstinctPointsDictionary[_opponentChimera]}/{_opponentChimera.CurrentHealthPoint}");*/
            if (_endCombat)
            {
                break;
            }
            if (_switchAttacker)
            {
                _switchAttacker = false;
            }

            
            
            //avoid while infinity loop
            if (_encounterCount > 50)
            {
                if (_playerChimera.CurrentHealthPoint >= _opponentChimera.CurrentHealthPoint)
                {
                    _testManager.WinnerIs(_playerChimera);    
                }
                else
                {
                    _testManager.WinnerIs(_opponentChimera);   
                }
                _endCombat = true;
            }
        }
    }

    //시작 시 선공 키메라 정하기. Agility 비교하여 같으면 반반뽑기
    private void SetFirstStates()
    {
        if (_playerChimera.AgilityPoint > _opponentChimera.AgilityPoint)
        {
            _attacker = _playerChimera;
            _defender = _opponentChimera;
            _combatChimerasStatesDictionary[_attacker].Add(_attacker.StandingState);
            _combatChimerasStatesDictionary[_defender].Add(_defender.StandingState);
        }
        else if (_playerChimera.AgilityPoint < _opponentChimera.AgilityPoint)
        {
            _attacker = _opponentChimera;
            _defender = _playerChimera;
            _combatChimerasStatesDictionary[_attacker].Add(_attacker.StandingState);
            _combatChimerasStatesDictionary[_defender].Add(_defender.StandingState);
        }
        else
        {
           int rand = UnityEngine.Random.Range(0, 2);
           switch (rand)
           {
               case 0:
                   _attacker = _playerChimera;
                   _defender = _opponentChimera;
                   _combatChimerasStatesDictionary[_attacker].Add(_attacker.StandingState);
                   _combatChimerasStatesDictionary[_defender].Add(_defender.StandingState);
                   break;
               case 1:
                   _attacker = _opponentChimera;
                   _defender = _playerChimera;
                   _combatChimerasStatesDictionary[_attacker].Add(_attacker.StandingState);
                   _combatChimerasStatesDictionary[_defender].Add(_defender.StandingState);
                   break;
           }
        }
    }

    /// <summary>
    /// basicAttack
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    private void Encounter(Chimera attacker, Chimera defender)
    {
        //AttackerSkillState, CanDodge, defenderState
        (IChimeraState, IAttackSkill, IChimeraState) attackerStateInfo = AttackerUseSkill(); 
        
        _combatChimerasStatesDictionary[attacker].Add(attackerStateInfo.Item1);

        //공격 스킬을 사용했다면
        if (attackerStateInfo.Item2 != null)
        {
            //사용된 공격스킬이 회피할 수 없다면
            if (!attackerStateInfo.Item2.IsDodgeAble())
            {
                //공격 스킬이 방어 키메라의 상태를 강제하는 경우
                if (attackerStateInfo.Item3 != null)
                {
                    if (_combatChimerasStatesDictionary[defender][^1] is GroggyState)
                    {
                        defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient * 2f);    
                        if (CheckEndCombat(attacker, defender)) return;

                        _combatChimerasInstinctPointsDictionary[_attacker] += 10;
                        _combatChimerasInstinctPointsDictionary[_defender] += 10;
                    }
                    else
                    {
                        defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient);    
                        if (CheckEndCombat(attacker, defender)) return;

                        _combatChimerasInstinctPointsDictionary[_attacker] += 10;
                        _combatChimerasInstinctPointsDictionary[_defender] += 10;
                    }
                    _combatChimerasStatesDictionary[defender].Add(attackerStateInfo.Item3);
                    return;
                }
                (IChimeraState, IDefendSkill) defenderSkill = DefenderUseSkill();
                _combatChimerasStatesDictionary[defender].Add(defenderSkill.Item1);    
                if (defenderSkill.Item2 != null)
                {
                    defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient * defenderSkill.Item2.DefendCoefficient);    
                }
                else
                {
                    defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient);
                }
                if (CheckEndCombat(attacker, defender)) return;

                _combatChimerasInstinctPointsDictionary[_attacker] += 10;
                _combatChimerasInstinctPointsDictionary[_defender] += 10;
                return;
            }
            
            //만약 디펜더의 직전 상태가 그로기상태라면
            if (_combatChimerasStatesDictionary[defender][^1] is GroggyState)
            {
                defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient * 2f);
                if (CheckEndCombat(attacker, defender)) return;

                _combatChimerasInstinctPointsDictionary[_attacker] += 10;
                _combatChimerasInstinctPointsDictionary[_defender] += 10;
                _combatChimerasStatesDictionary[defender].Add(attackerStateInfo.Item3);
                return;
            }
            
            //사용된 공격스킬이 회피할 수 있다면
     
            
            //밸런싱이 필요해요
            _attackerCoefficient = UnityEngine.Random.Range(1, (int)(attacker.AgilityPoint/10)); 
            _defenderCoefficient = UnityEngine.Random.Range(1, (int)(defender.AgilityPoint/10));
            
            //회피 여부
            //회피에 실패함.
            if (_attackerCoefficient > _defenderCoefficient)
            {
                //공격 스킬이 방어 키메라의 상태를 강제하는 경우
                if (attackerStateInfo.Item3 != null)
                {
                    if (_combatChimerasStatesDictionary[defender][^1] is GroggyState)
                    {
                        defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient * 2f);
                        if (CheckEndCombat(attacker, defender)) return;

                        _combatChimerasInstinctPointsDictionary[_attacker] += 10;
                        _combatChimerasInstinctPointsDictionary[_defender] += 10;
                    }
                    else
                    {
                        defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient);
                        if (CheckEndCombat(attacker, defender)) return;

                        _combatChimerasInstinctPointsDictionary[_attacker] += 10;
                        _combatChimerasInstinctPointsDictionary[_defender] += 10;
                    }

                    _combatChimerasStatesDictionary[defender].Add(attackerStateInfo.Item3);
                    return;
                }

                //방어 스킬 사용
                (IChimeraState, IDefendSkill) defenderSkill = DefenderUseSkill();
                _combatChimerasStatesDictionary[defender].Add(defenderSkill.Item1);    
                if (defenderSkill.Item2 != null)
                {
                    defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient * defenderSkill.Item2.DefendCoefficient);    
                }
                else
                {
                    defender.TakeDamage(attacker.AttackPoint * attackerStateInfo.Item2.DamageCoefficient);
                }
                if (CheckEndCombat(attacker, defender)) return;

                _combatChimerasInstinctPointsDictionary[_attacker] += 10;
                _combatChimerasInstinctPointsDictionary[_defender] += 10;
                return;
            }
           
            _combatChimerasStatesDictionary[defender].Add(defender.DodgeState);
            SwitchAttacker();
            _endCombat = false;
            return;
        }
        //공격 스킬이 사용되지 않았다면 == 기본공격이라면
        
        //밸런싱이 필요해요
        _attackerCoefficient = UnityEngine.Random.Range(1, (int)(attacker.AgilityPoint/10)); 
        _defenderCoefficient = UnityEngine.Random.Range(1, (int)(defender.AgilityPoint/10));
        
        //회피 여부
        if (_attackerCoefficient > _defenderCoefficient)//회피에 실패함.
        {
            //공격 스킬이 방어 키메라의 상태를 강제하지 않는 경우
            //방어 스킬 사용
            (IChimeraState, IDefendSkill) defenderSkill = DefenderUseSkill();
            _combatChimerasStatesDictionary[defender].Add(defenderSkill.Item1);
            if (defenderSkill.Item2 != null)
            {
                defender.TakeDamage(attacker.AttackPoint * defenderSkill.Item2.DefendCoefficient);    
            }
            else
            {
                defender.TakeDamage(attacker.AttackPoint);
            }
            
            
            if (CheckEndCombat(attacker, defender)) return;

            _combatChimerasInstinctPointsDictionary[_attacker] += 10;
            _combatChimerasInstinctPointsDictionary[_defender] += 10;
        }
        else
        {
            _combatChimerasStatesDictionary[defender].Add(defender.DodgeState);
            SwitchAttacker();
        }
        _endCombat = false;
    }

    private bool CheckEndCombat(Chimera attacker, Chimera defender)
    {
        if (defender.CurrentHealthPoint <= 0)
        {
            _combatChimerasStatesDictionary[defender].Add(defender.DeathState);
            _testManager.WinnerIs(attacker);
            _endCombat = true;
            return true;
        }

        return false;
    }

    private void SwitchAttacker()
    {
        _switchAttacker = true;
        //회피시 공수 교대
        if (_combatChimerasStatesDictionary[_defender][^1] is DodgeState)
        {
            (_attacker, _defender) = (_defender, _attacker);
        }
    }

    /// <summary>
    ///  </summary>
    /// <returns>AttackerSkillState, bool is DodgeAble, DefenderState by AttackerSkill</returns>
    private (IChimeraState, IAttackSkill, IChimeraState) AttackerUseSkill()
    {
        //attacker의 모든 스킬 중
        List<IDnaSkill> skills = new List<IDnaSkill>();
        skills.AddRange(_attacker.SubSkills);
        skills.Add(_attacker.MainSkill);
        
        //공격 스킬들 찾고
        List<IAttackSkill> attackSkills = new List<IAttackSkill>();
        foreach (var skill in skills)
        {
            if (skill is IAttackSkill iAttackSkill)
            {
                attackSkills.Add(iAttackSkill);
            }
        }

        //공격 스킬이 없어?
        if (attackSkills.Count == 0)
        {
            return (_attacker.BasicAttackState, null, null);
        }
        
        //공격 스킬들 중 현재 자원량으로 사용 가능한 스킬들을 찾고
        List<IAttackSkill> currentUseAbleAttackSkills = new List<IAttackSkill>();
        foreach (var skill in attackSkills)
        {
            if (skill.NeedInstinctPoint <= _combatChimerasInstinctPointsDictionary[_attacker])
            {
                currentUseAbleAttackSkills.Add(skill);
            }
        }
        
        //자원량으로 사용 가능한 스킬이 없어?
        if (currentUseAbleAttackSkills.Count == 0)
        {
            return (_attacker.BasicAttackState, null, null);
        }

        //자원량을 충족하는 스킬들 중 적의 특정 마지막 상태를 요구하는 스킬들을 찾는다.
        List<IAttackSkill> stateCheckedSkills = new List<IAttackSkill>();
        foreach (var skill in currentUseAbleAttackSkills)
        {
            if (skill.IsOpponentOnState(_combatChimerasStatesDictionary[_defender][^1]))
            {
                stateCheckedSkills.Add(skill);
            }
        }

        switch (stateCheckedSkills.Count)
        {
            //상태 요구를 충족하는 스킬이 없어?
            case 0:
                return (_attacker.BasicAttackState, null, null);
            case 1://있으면서 하나야?
                _combatChimerasInstinctPointsDictionary[_attacker] -= stateCheckedSkills[0].NeedInstinctPoint;
                return (_attacker.SkillStates[stateCheckedSkills[0]], null, null);
            //여러개면 다음으로
        }
        
        IAttackSkill attackSkill = null;
        
        //공격스킬이고, 현재 자원량으로 사용이 가능하고, 특정 상태를 요구하는 스킬들 중 혹시 메인스킬이 있어?
        foreach (IAttackSkill skill in stateCheckedSkills)
        {
            if (skill is DnaMainSkill)
            {
                _combatChimerasInstinctPointsDictionary[_attacker] -= skill.NeedInstinctPoint;
                return (_attacker.SkillStates[skill], skill, skill.AdditionalEffectToOpponentChimera(_defender));
            }
        }
        
        //공격스킬이고, 현재 자원량으로 사용 가능하고, 적의 마지막 상태가 일치하는 스킬들이면서 메인스킬이 아닌 것들 중에
        switch (stateCheckedSkills.Count)
        {
            //그런 스킬이 없다면, 그냥 공격 상태
            case 0:
                break;
            case 1:
                attackSkill = stateCheckedSkills[0];
                break;
            case >= 2:
                foreach (var skill in stateCheckedSkills)
                {
                    //처음엔 바로 할당
                    if (attackSkill == null)
                    {
                        attackSkill = skill;
                        continue;
                    }
                    //할당된 스킬보다 할당되지 않은 스킬의 요구 포인트가 더 높다면
                    if (skill.NeedInstinctPoint > attackSkill.NeedInstinctPoint)
                    {
                        attackSkill = skill;
                    }
                }
                break;
        }

        if (attackSkill == null)
        {
            return (_attacker.BasicAttackState, null, null);
        }

        _combatChimerasInstinctPointsDictionary[_attacker] -= attackSkill.NeedInstinctPoint;
        return (_attacker.SkillStates[attackSkill], attackSkill, attackSkill.AdditionalEffectToOpponentChimera(_defender));
    }

    /// <summary>
    ///  </summary>
    /// <returns>Defender(MainSkill)State, DefenderSkill</returns>
    private (IChimeraState, IDefendSkill) DefenderUseSkill()
    {
        //defender의 모든 스킬 중
        List<IDnaSkill> skills = new List<IDnaSkill>();
        skills.AddRange(_defender.SubSkills);
        skills.Add(_defender.MainSkill);
        
        //방어 스킬들 찾고
        List<IDefendSkill> defendSkills = new List<IDefendSkill>();
        foreach (var skill in skills)
        {
            if (skill is IDefendSkill)
            {
                defendSkills.Add(skill as IDefendSkill);
            }
        }
        
        //방어 스킬들 중 현재 자원량으로 사용 가능한 스킬들을 찾고
        List<IDefendSkill> currentUseAbleDefendSkills = new List<IDefendSkill>();
        foreach (var skill in defendSkills)
        {
            if (skill.NeedInstinctPoint <= _combatChimerasInstinctPointsDictionary[_defender])
            {
                currentUseAbleDefendSkills.Add(skill);
            }
        }

        //자원량을 충족하는 스킬들 중 적의 특정 마지막 상태를 요구하는 스킬들을 찾는다.
        List<IDefendSkill> stateCheckedSkills = new List<IDefendSkill>();
        foreach (var skill in currentUseAbleDefendSkills)
        {
            if (skill.IsOpponentOnState(_combatChimerasStatesDictionary[_attacker][^1]))
            {
                stateCheckedSkills.Add(skill);
            }
        }
        
        switch (stateCheckedSkills.Count)
        {
            //상태 요구를 충족하는 스킬이 없어?
            case 0:
                return (_defender.VulnerableState, null);
            case 1://있으면서 하나야?
                _combatChimerasInstinctPointsDictionary[_defender] -= stateCheckedSkills[0].NeedInstinctPoint;
                return (_defender.SkillStates[stateCheckedSkills[0].AdditionalEffectToMySelf(_defender)], stateCheckedSkills[0]);
            //여러개면 다음으로
        }
        
        
        
        
        
        IDefendSkill defendSkill = null;
        
        //방어형 메인스킬이 있으면 메인스킬을 사용한다.
        foreach (IDefendSkill skill in stateCheckedSkills)
        {
            if (skill is DnaMainSkill)
            {
                return (_defender.SkillStates[skill.AdditionalEffectToMySelf(_defender)], skill);
            }
        }
        
        switch (stateCheckedSkills.Count)
        {
            case 0:
                break;
            case 1:
                defendSkill = stateCheckedSkills[0];
                break;
            case >= 2:
                //상태 요구를 충족하는 스킬이 여러개면 그중 가장 비싼 값을 요구하는 스킬을 사용한다.
                foreach (var skill in stateCheckedSkills)
                {
                    //처음엔 바로 할당
                    if (defendSkill == null)
                    {
                        defendSkill = skill;
                        continue;
                    }
                    //할당된 스킬보다 할당되지 않은 스킬의 요구 포인트가 더 높다면
                    if (skill.NeedInstinctPoint > defendSkill.NeedInstinctPoint)
                    {
                        defendSkill = skill;
                    }
                }
                break;
        }

        if (defendSkill == null)
        {
            return (_defender.VulnerableState, null);
        }
        
        return (_defender.SkillStates[defendSkill.AdditionalEffectToMySelf(_defender)], defendSkill);
    }
    
    /*/// <summary>
   /// attackerSkill
   /// </summary>
   /// <param name="attacker"></param>
   /// <param name="attackerState"></param>
   /// <param name="defender"></param>
   private void IsVulnerableBySkillAttack(Chimera attacker,IChimeraState attackerState, Chimera defender)
   {
       _combatChimerasStatesDictionary[attacker].Add(attackerState);
       //밸런싱이 필요해요
       _attackerCoefficient = UnityEngine.Random.Range(1, (int)attacker.AgilityPoint);
       _defenderCoefficient = UnityEngine.Random.Range(1, (int)defender.AgilityPoint);

       //회피 여부
       if (_attackerCoefficient > _defenderCoefficient)
       {
           _combatChimerasStatesDictionary[defender].Add(DefenderUseSkill());
           defender.TakeDamage(attacker.AttackPoint);
           if (defender.CurrentHealthPoint <= 0)
           {
               _combatChimerasStatesDictionary[defender].Add(defender.DeathState);
               _testManager.WinnerIs(attacker);
               _endCombat = true;
               return;
           }
       }
       else
       {
           _combatChimerasStatesDictionary[defender].Add(defender.DodgeState);
           SwitchAttacker();
       }
       _endCombat = false;
   }*/
}
