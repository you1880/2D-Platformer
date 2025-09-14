using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneService
{
    BaseScene CurrentScene { get; }
    Define.SceneType CurrentSceneType { get; }
    void LoadScene(Define.SceneType sceneType);
}
