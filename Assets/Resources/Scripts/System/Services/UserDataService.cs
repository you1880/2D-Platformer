using System;
using System.Collections;
using System.Collections.Generic;
using Data.User;
using UnityEngine;

[AutoRegister(typeof(IUserDataService), priority: 15)]
public class UserDataService : IUserDataService
{
    private readonly IDataService _dataService;
    private readonly ISaveService _saveService;
    private Userdata userData => _saveService.CurrentData;
    public UserDataService(IDataService dataService, ISaveService saveService)
    {
        _dataService = dataService;
        _saveService = saveService;
    }

    public int PlayerType
        => userData?.playerType ?? 0;
        
    public int ActivityPoint
        => userData?.activityPoint ?? 0;

    public int MaxActivityPoint
        => userData?.maxActivityPoint ?? 0;

    public int Gold
        => userData?.gold ?? 0;

    public int Level
        => userData?.level ?? 0;

    public int Exp
        => userData?.exp ?? 0;

    public int Attack
        => userData?.attack ?? 1;

    public int MaxHP
        => userData?.maxHp ?? 0;

    public int HP
        => userData?.hp ?? 0;

    public List<ClearStage> ClearStages
        => userData?.clearStages ?? new List<ClearStage>();

    public event Action<int, int> OnActivityChanged;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnHpChanged;

    public bool AddActivityPoint(int amount)
    {
        if (userData == null)
            return false;

        if (userData.activityPoint + amount > userData.maxActivityPoint)
            return false;

        userData.activityPoint += amount;
        OnActivityChanged?.Invoke(userData.activityPoint, userData.maxActivityPoint);

        return true;
    }

    public void AddGold(int amount)
    {
        if (userData == null)
            return;

        userData.gold += amount;
        OnGoldChanged?.Invoke(userData.gold);
    }

    public bool TrySpendGold(int amount)
    {
        if (userData == null)
            return false;

        if (userData.gold - amount < 0)
            return false;

        userData.gold -= amount;
        OnGoldChanged?.Invoke(userData.gold);

        return true;
    }

    public bool ConsumeActivityPoint(int amount)
    {
        if (userData == null)
            return false;

        if (userData.activityPoint - amount < 0)
            return false;

        userData.activityPoint -= amount;
        OnActivityChanged?.Invoke(userData.activityPoint, userData.maxActivityPoint);

        return true;
    }

    public void ReduceHp(int amount)
    {
        if (userData == null)
            return;

        userData.hp = Math.Max(userData.hp - amount, 0);
        OnHpChanged?.Invoke(userData.hp);
    }

    public bool HealHp(int amount)
    {
        if (userData == null)
            return false;

        if( userData.hp >= userData.maxHp)
            return false;
            
        userData.hp = Math.Min(userData.hp + amount, userData.maxHp);
        OnHpChanged?.Invoke(userData.hp);

        return true;
    }

    public void AddExp(int amount)
    {
        if (userData == null || amount <= 0)
            return;

        while (amount > 0)
        {
            Data.Game.RequireExp requireExp = _dataService.GetRequireExp(userData.level);
            if (requireExp == null)
                break;

            int curExp = Exp;
            int needExp = requireExp.exp;
            int remainingExpForLevel = needExp - curExp;
            
            if (amount >= remainingExpForLevel)
            {
                amount -= remainingExpForLevel;
                userData.level += 1;
                userData.exp = 0;
            }
            else
            {
                userData.exp += amount;
                amount = 0;
            }
        }
    }
}
