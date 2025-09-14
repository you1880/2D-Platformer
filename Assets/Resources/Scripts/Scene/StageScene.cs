using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScene : BaseScene
{
    private IPathProvider _pathProvider;
    private IStageService _stageService;
    private ISceneService _sceneService;
    private GameObject _player;
    private BoxCollider2D _areaBound;
    private bool _stageLoaded = false;
    private float _timer = 0.0f;
    
    public float GetTimer()
    {
        int minutes = (int)(_timer / 60);
        int seconds = (int)(_timer % 60);

        return _timer;
    }

    protected override void EnsureAdditionalService()
    {
        if (_pathProvider == null)
            _pathProvider = ServiceLocator.GetService<IPathProvider>();
        if (_stageService == null)
            _stageService = ServiceLocator.GetService<IStageService>();
        if (_sceneService == null)
            _sceneService = ServiceLocator.GetService<ISceneService>();
    }

    public override void Clear() { }

    public override void Init()
    {
        SceneType = Define.SceneType.Stage;

        _stageLoaded = true;
        InstantiateStage();
        CameraSetting();

        if (!_stageLoaded)
            _sceneService.LoadScene(Define.SceneType.Lobby);

        _soundService.PlayStageBGM(_stageService.CurrentStage);
    }

    private void InstantiateStage()
    {
        Define.Stage stage = _stageService.CurrentStage;
        if (stage == Define.Stage.None)
        {
            _stageLoaded = false;
            return;
        }

        string path = _pathProvider.GetStagePath(stage);
        GameObject stagePrefab = _resourceService.Instantiate(path);

        if (stagePrefab == null)
        {
            Debug.LogError($"Stage Prefab is null. Path: {path}");
            _stageLoaded = false;
            return;
        }

        Stage stageComp = stagePrefab.GetComponent<Stage>();
        if (stageComp == null)
        {
            _stageLoaded = false;
            return;
        }

        _areaBound = stageComp.AreaBound;
        InstantiatePlayer(stageComp.CharacterSpawnPosition);
    }

    private void InstantiatePlayer(Vector3 spawnPosition)
    {
        if (_player != null)
        {
            _resourceService.Destroy(_player);
            _player = null;
        }

        Data.User.Userdata currentData = _saveService.CurrentData;
        if (currentData == null)
        {
            _stageLoaded = false;
            return;
        }

        string path = $"Player/Player{currentData.playerType}";
        _player = _resourceService.Instantiate(path);
        if (_player == null)
        {
            _stageLoaded = false;
            return;
        }

        _player.transform.position = spawnPosition;
    }

    private void CameraSetting()
    {
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (_player == null || cameraController == null)
        {
            _stageLoaded = false;
            return;
        }

        cameraController.InitailizeCamera(_player, _areaBound);
    }

    private void Update()
    {
        if (_stageLoaded)
            _timer += Time.deltaTime;
    }
}
