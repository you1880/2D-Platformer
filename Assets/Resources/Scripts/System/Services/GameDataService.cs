using System.Collections;
using System.Collections.Generic;
using Data.Game;
using UnityEngine;

[AutoRegister(typeof(IDataService), priority: 10)]
public class GameDataService : IDataService
{
    private readonly IResourceService _resourceService;
    private GameDataSO _gameDataSO;
    private GameDataRegistry<Define.Stage, Data.Game.StageDependency> _stageDependencyData = new GameDataRegistry<Define.Stage, StageDependency>();
    private GameDataRegistry<Define.Stage, Data.Game.StageData> _stageData = new GameDataRegistry<Define.Stage, StageData>();
    private GameDataRegistry<int, Data.Game.RequireExp> _requireExpData = new GameDataRegistry<int, RequireExp>();
    private GameDataRegistry<int, Data.Game.Item> _itemData = new GameDataRegistry<int, Data.Game.Item>();
    private GameDataRegistry<Define.EnemyType, Data.Game.Enemy> _enemyData = new GameDataRegistry<Define.EnemyType, Data.Game.Enemy>();

    public GameDataService(IResourceService resourceService)
    {
        _resourceService = resourceService;
        LoadAll();
    }

    /// <summary>
    /// Get 함수들은 데이터 조회용 함수 호출부에서 null 체크를 하세요
    /// </summary>

    public StageDependency GetStageDependency(Define.Stage stage)
        => _stageDependencyData.TryGet(stage, out StageDependency stageDependency) ? stageDependency : null;

    public IReadOnlyDictionary<Define.Stage, StageDependency> GetAllStageDependencies()
        => _stageDependencyData.Dict;

    public StageData GetStageData(Define.Stage stage)
        => _stageData.TryGet(stage, out StageData stageData) ? stageData : null;

    public RequireExp GetRequireExp(int level)
        => _requireExpData.TryGet(level, out RequireExp requireExp) ? requireExp : null;

    public IReadOnlyDictionary<int, Item> GetAllItems()
        => _itemData.Dict;
    
    public Item GetItem(int itemCode)
        => _itemData.TryGet(itemCode, out Item item) ? item : null;

    public Sprite GetItemSprite(int itemCode)
    {
        Item item = GetItem(itemCode);
        if (item == null)
        {
            return null;
        }

        string spriteName = item.itemName;
        return _resourceService.LoadItemSprite(spriteName);
    }

    public Data.Game.Enemy GetEnemy(Define.EnemyType enemyType)
        => _enemyData.TryGet(enemyType, out Data.Game.Enemy enemy) ? enemy : null;
        
    private void LoadAll()
    {
        _gameDataSO = _resourceService.Load<GameDataSO>("GameDataSO");

        try
        {
            _stageDependencyData.LoadData(_gameDataSO.stageDependencies, stage => stage.stage);
            _stageData.LoadData(_gameDataSO.stageData, stage => stage.stage);
            _requireExpData.LoadData(_gameDataSO.requireExp, req => req.level);
            _itemData.LoadData(_gameDataSO.itemData, item => item.itemCode);
            _enemyData.LoadData(_gameDataSO.enemyData, enemy => enemy.enemyType);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
