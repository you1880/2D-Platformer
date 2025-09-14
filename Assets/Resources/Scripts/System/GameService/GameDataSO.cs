using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "ScriptableObjects/GameDataSO", order = 1)]
public class GameDataSO : ScriptableObject
{
    public List<Data.Game.StageDependency> stageDependencies;
    public List<Data.Game.StageData> stageData;
    public List<Data.Game.RequireExp> requireExp;
    public List<Data.Game.Item> itemData;
    public Data.Game.Shop shopData;
    public List<Data.Game.Enemy> enemyData;

    public void LoadFromGameData(Data.Game.GameData gameData)
    {
        stageDependencies = gameData.stageDependencies;
        stageData = gameData.stageData;
        requireExp = gameData.requireExp;
        itemData = gameData.itemData;
        shopData = gameData.shopData;
        enemyData = gameData.enemyData;
    }
}
