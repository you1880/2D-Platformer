using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserDataService
{
    int PlayerType { get; }
    int ActivityPoint { get; }
    int MaxActivityPoint { get; }
    int Gold { get; }
    int Level { get; }
    int Exp { get; }
    int Attack { get; }
    int MaxHP { get; }
    int HP { get; }
    List<Data.User.ClearStage> ClearStages { get; }
    event Action<int, int> OnActivityChanged;
    event Action<int> OnGoldChanged;
    event Action<int> OnHpChanged;
    bool ConsumeActivityPoint(int amount);
    bool AddActivityPoint(int amount);
    void AddGold(int amount);
    bool TrySpendGold(int amount);
    void ReduceHp(int amount);
    bool HealHp(int amount);
    void AddExp(int amount);
}
