using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

[AutoRegister(typeof(IStageService), priority: 30)]
public class StageService : IStageService
{
    private IReadOnlyDictionary<Define.Stage, Data.Game.StageDependency> _stageDependencies;
    private readonly IDataService _dataService;
    private readonly ISaveService _saveService;
    private readonly ISceneService _sceneService;
    private readonly IUIService _uiService;
    private readonly IUserDataService _userDataService;
    private Define.Stage _currentStage = Define.Stage.None;
    public Define.Stage CurrentStage { get { return _currentStage; } }

    public StageService(IDataService dataService, ISaveService saveService, ISceneService sceneService, IUIService uiService, IUserDataService userDataService)
    {
        _dataService = dataService;
        _saveService = saveService;
        _sceneService = sceneService;
        _userDataService = userDataService;
        _uiService = uiService;

        _stageDependencies = _dataService.GetAllStageDependencies();
    }

    public string GetStageClearTime(Define.Stage stage)
    {
        if (stage == Define.Stage.None || !IsStageClear(stage))
        {
            return "";
        }

        foreach (var clearStage in _userDataService.ClearStages)
        {
            if (clearStage.stage == stage)
            {
                return $"{clearStage.clearMin:D2}:{clearStage.clearSec:D2}";
            }
        }

        return "";
    }

    public bool IsStageClear(Define.Stage stage)
    {
        if (stage == Define.Stage.None)
        {
            return false;
        }

        foreach (var clearStage in _userDataService.ClearStages)
        {
            if (clearStage.stage == stage)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanEnterStage(Define.Stage stage)
    {
        if (stage == Define.Stage.None)
        {
            return false;
        }

        List<Data.User.ClearStage> cleardata = _userDataService.ClearStages;
        HashSet<Define.Stage> clearStages = new HashSet<Define.Stage>();

        if (cleardata != null)
        {
            foreach (var clearStage in cleardata)
            {
                clearStages.Add(clearStage.stage);
            }
        }

        Data.Game.StageData stageData = _dataService.GetStageData(stage);
        if (stageData == null)
        {
            return false;
        }

        if (_userDataService.ActivityPoint < stageData.requireActivityPoint)
        {
            return false;
        }

        List<Define.Stage> requireStages = _stageDependencies.TryGetValue(stage, out var data) ? data.requireStages : new List<Define.Stage>();

        foreach (Define.Stage dependency in requireStages)
        {
            if (!clearStages.Contains(dependency))
            {
                return false;
            }
        }

        return true;
    }

    public void EnterStage(Define.Stage stage)
    {
        if (!CanEnterStage(stage))
        {
            return;
        }

        _userDataService.ConsumeActivityPoint(_dataService.GetStageData(stage).requireActivityPoint);
        _currentStage = stage;
        _sceneService.LoadScene(Define.SceneType.Stage);
    }

    public bool SkipStage(Define.Stage stage)
    {
        if (!IsStageClear(stage) || !CanEnterStage(stage))
        {
            return false;
        }

        Data.Game.StageData stageData = _dataService.GetStageData(stage);
        if (stageData == null)
        {
            return false;
        }

        int requireActivity = stageData.requireActivityPoint;
        int minSkipGold = stageData.minSkipGold;
        int maxSkipGold = stageData.maxSkipGold;

        if (!_userDataService.ConsumeActivityPoint(requireActivity))
        {
            return false;
        }

        _userDataService.AddGold(Random.Range(minSkipGold, maxSkipGold + 1));
        return true;
    }

    public void ClearStage(Define.Stage stage, float timer)
    {
        if (stage == Define.Stage.None)
        {
            return;
        }

        int secs = Mathf.FloorToInt(timer);
        int clearMin = secs / 60;
        int clearSec = secs % 60;
        bool flag = false;

        foreach (var clearStage in _userDataService.ClearStages)
        {
            if (clearStage.stage == stage)
            {
                flag = true;
                if (clearStage.clearMin > clearMin || (clearStage.clearMin == clearMin && clearStage.clearSec > clearSec))
                {
                    clearStage.clearMin = clearMin;
                    clearStage.clearSec = clearSec;
                }
                break;
            }
        }

        if (!flag)
        {
            _userDataService.ClearStages.Add(new Data.User.ClearStage(stage, clearMin, clearSec));
        }

        _uiService.ShowUI<UI_StageClear>();
        AddClearReward(stage);
        _saveService.Save();
    }

    public void FailStage(Define.Stage stage)
    {
        _uiService.ShowMessageBox(MessageID.StageFail,
            () =>
            {
                _sceneService.LoadScene(Define.SceneType.Lobby);
                _userDataService.HealHp(_userDataService.MaxHP / 2);
            }, true);
        _currentStage = Define.Stage.None;
    }

    private void AddClearReward(Define.Stage stage)
    {
        if (stage == Define.Stage.None)
        {
            return;
        }

        Data.Game.StageData stageData = _dataService.GetStageData(stage);
        if (stageData == null)
        {
            return;
        }

        int gold = stageData.clearGold;
        int exp = stageData.clearExp;

        _userDataService.AddGold(gold);
        _userDataService.AddExp(exp);
    }
}
