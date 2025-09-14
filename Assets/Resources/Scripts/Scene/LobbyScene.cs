using System.Collections;
using System.Collections.Generic;
using Data.User;
using UnityEngine;

public class LobbyScene : BaseScene
{
    [SerializeField] private Vector3 _CharacterSpawnPosition;
    private GameObject _player;

    public override void Clear() { }

    public override void Init()
    {
        SceneType = Define.SceneType.Lobby;
        _soundService.PlayBGM(SceneType);

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (_player != null)
        {
            _resourceService.Destroy(_player);
            _player = null;
        }

        Userdata currentData = _saveService.CurrentData;
        if (currentData == null)
        {
            return;
        }

        string path = $"Player/Player{currentData.playerType}";
        _player = _resourceService.Instantiate(path);
        if (_player != null)
        {
            _player.transform.position = _CharacterSpawnPosition;
        }
    }
}
