using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    public Define.SceneType SceneType = Define.SceneType.Unknown;
    protected IUIService _uiService;
    protected ISoundService _soundService;
    protected IResourceService _resourceService;
    protected ISaveService _saveService;

    public abstract void Init();
    public abstract void Clear();

    protected void EnsureBasicService()
    {
        if (_uiService == null)
            _uiService = ServiceLocator.GetService<IUIService>();
        if (_soundService == null)
            _soundService = ServiceLocator.GetService<ISoundService>();
        if (_resourceService == null)
            _resourceService = ServiceLocator.GetService<IResourceService>();
        if (_saveService == null)
            _saveService = ServiceLocator.GetService<ISaveService>();
    }

    protected virtual void EnsureAdditionalService() {}

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ServiceLocator.IsInitialized);

        EnsureBasicService();
        EnsureAdditionalService();
        Init();
    }

    private void OnDestroy()
    {
        Clear();
    }
}
