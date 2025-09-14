using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathProvider
{
    string GetSaveDirPath(int slotId);
    string GetSavePath(int slotId);
    string GetBgmSoundPath(Define.SceneType sceneType);
    string GetStageBgmSoundPath(Define.Stage stage);
    string GetEffectSoundPath(Define.EffectSoundType effectSoundType);
    string GetPrefabPath(string prefabName);
    string GetUIPath(string uiName);
    string GetSpriteAtlasPath();
    string GetStagePath(Define.Stage stage);
}
