using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundService
{
    float BgmVolume { get; }
    float EffectVolume { get; }

    void SetBgmVolume(float volume);
    void SetEffectVolume(float volume);
    void Play(AudioClip audioClip, Define.SoundType soundType = Define.SoundType.Effect, float pitch = 1.0f);
    void PlayBGM(Define.SceneType sceneType);
    void PlayStageBGM(Define.Stage stage);
    void PlayEffect(Define.EffectSoundType effectSoundType);
}
