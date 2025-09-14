using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataService
{
    Data.Game.StageDependency GetStageDependency(Define.Stage stage);
    IReadOnlyDictionary<Define.Stage, Data.Game.StageDependency> GetAllStageDependencies();
    Data.Game.StageData GetStageData(Define.Stage stage);
    Data.Game.RequireExp GetRequireExp(int level);
    IReadOnlyDictionary<int, Data.Game.Item> GetAllItems();
    Data.Game.Item GetItem(int itemCode);
    Sprite GetItemSprite(int itemCode);
    Data.Game.Enemy GetEnemy(Define.EnemyType enemyType);

}
