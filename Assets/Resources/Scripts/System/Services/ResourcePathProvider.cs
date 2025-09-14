using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AutoRegister(typeof(IPathProvider), priority: 0)]
public class ResourcePathProvider : IPathProvider
{
    private const string BGM_PATH = "Sounds/Bgm/";
    private const string EFFECT_PATH = "Sounds/Effect/";
    private const string SPRITE_ATLAS_PATH = "Arts/Item/ItemSpriteAtlas";

    public string GetSaveDirPath(int slotId) => Path.Combine(Application.persistentDataPath, $"Slot_{slotId:D2}");

    public string GetSavePath(int slotId)
    {
        string dirPath = GetSaveDirPath(slotId);

        return Path.Combine(dirPath, $"PlayerData{slotId:D2}.json");
    }

    public string GetBgmSoundPath(Define.SceneType sceneType)
    {
        if (sceneType == Define.SceneType.Unknown)
        {
            return string.Empty;
        }

        return string.Concat(BGM_PATH, sceneType.ToString());
    }

    public string GetStageBgmSoundPath(Define.Stage stage)
    {
        if (stage == Define.Stage.None)
        {
            return string.Empty;
        }

        return string.Concat(BGM_PATH, "Stage", (int)stage);
    }

    public string GetEffectSoundPath(Define.EffectSoundType effectSoundType)
    {
        string soundName = Enum.GetName(typeof(Define.EffectSoundType), effectSoundType);

        return string.Concat(EFFECT_PATH, soundName);
    }

    public string GetPrefabPath(string prefabName) => $"Prefabs/{prefabName}";

    public string GetUIPath(string uiName) => $"UI/{uiName}";

    public string GetSpriteAtlasPath() => SPRITE_ATLAS_PATH;

    public string GetStagePath(Define.Stage stage)
    {
        if (stage == Define.Stage.None)
        {
            return string.Empty;
        }

        return $"Stages/{stage.ToString()}";
    }
}
