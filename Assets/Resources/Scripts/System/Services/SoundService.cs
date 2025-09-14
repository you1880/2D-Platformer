using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AutoRegister(typeof(ISoundService), priority: 10)]
public class SoundService : ISoundService
{
    public float BgmVolume => _bgmVolume;
    public float EffectVolume => _effectVolume;
    private IPathProvider _pathProvider;
    private IResourceService _resourceService;
    private AudioSource[] _audioSources = new AudioSource[(int)Define.SoundType.Count];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private Dictionary<Define.EffectSoundType, AudioClip> _effectAudioClips = new Dictionary<Define.EffectSoundType, AudioClip>();
    private float _bgmVolume = 1.0f;
    private float _effectVolume = 1.0f;

    public SoundService(IPathProvider pathProvider, IResourceService resourceService)
    {
        _pathProvider = pathProvider;
        _resourceService = resourceService;

        Init();
    }

    public void SetBgmVolume(float volume)
    {
        AudioSource audioSource = _audioSources[(int)Define.SoundType.Bgm];
        if (audioSource == null)
        {
            return;
        }

        _bgmVolume = Mathf.Clamp01(volume / 100.0f);
        audioSource.volume = _bgmVolume;
    }

    public void SetEffectVolume(float volume)
    {
        _effectVolume = Mathf.Clamp01(volume / 100.0f);
    }

    public void Play(string path, Define.SoundType soundType = Define.SoundType.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, soundType);

        Play(audioClip, soundType, pitch);
    }

    public void Play(AudioClip audioClip, Define.SoundType soundType = Define.SoundType.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
        {
            return;
        }

        if (soundType == Define.SoundType.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.SoundType.Bgm];

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.SoundType.Effect];
            audioSource.pitch = pitch;
            audioSource.volume = _effectVolume;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void PlayBGM(Define.SceneType sceneType)
    {
        if (sceneType == Define.SceneType.Unknown)
        {
            return;
        }

        string path = _pathProvider.GetBgmSoundPath(sceneType);
        Play(path, Define.SoundType.Bgm);
    }

    public void PlayStageBGM(Define.Stage stage)
    {
        if (stage == Define.Stage.None)
        {
            return;
        }
        
        string path = _pathProvider.GetStageBgmSoundPath(stage);
        Play(path, Define.SoundType.Bgm);
    }

    public void PlayEffect(Define.EffectSoundType effectSoundType)
    {
        if (!_effectAudioClips.TryGetValue(effectSoundType, out AudioClip audioClip))
        {
            string path = _pathProvider.GetEffectSoundPath(effectSoundType);
            audioClip = _resourceService.Load<AudioClip>(path);

            if (audioClip == null)
            {
                return;
            }

            _effectAudioClips.Add(effectSoundType, audioClip);
        }

        Play(audioClip, Define.SoundType.Effect);
    }

    private void Init()
    {
        GameObject root = GameObject.Find("@SoundRoot");

        if (root == null)
        {
            root = new GameObject { name = "@SoundRoot" };
            UnityEngine.Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject obj = new GameObject { name = soundNames[i] };
                _audioSources[i] = obj.AddComponent<AudioSource>();
                obj.transform.parent = root.transform;
            }

            _audioSources[(int)Define.SoundType.Bgm].loop = true;
        }
    }
    
    private AudioClip GetOrAddAudioClip(string path, Define.SoundType soundType = Define.SoundType.Effect)
    {
        AudioClip audioClip = null;

        if (soundType == Define.SoundType.Bgm)
        {
            audioClip = _resourceService.Load<AudioClip>(path);
        }
        else
        {
            if (!_audioClips.TryGetValue(path, out audioClip))
            {
                audioClip = _resourceService.Load<AudioClip>(path);
                if (audioClip != null)
                {
                    _audioClips.Add(path, audioClip);
                }
            }
        }

        if(audioClip == null)
        {
            Debug.Log($"AudioClip Missing : {path}");
        }

        return audioClip;
    }
}
