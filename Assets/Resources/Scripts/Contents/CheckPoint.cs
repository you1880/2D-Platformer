using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private enum CheckPointType
    {
        None, Start, End, Temp
    }
    private ISoundService _soundService;
    private ISceneService _sceneService;    
    private IStageService _stageService;
    [SerializeField] private CheckPointType _checkPointType = CheckPointType.None;
    private StageScene _stageScene;
    private bool _checked = false;

    private void Start()
    {
        _soundService = ServiceLocator.GetService<ISoundService>();
        _sceneService = ServiceLocator.GetService<ISceneService>();
        _stageService = ServiceLocator.GetService<IStageService>();

        _stageScene = _sceneService.CurrentScene as StageScene;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_checkPointType == CheckPointType.None || _checkPointType == CheckPointType.Start)
        {
            return;
        }

        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (_checked)
        {
            _sceneService.LoadScene(Define.SceneType.Lobby);
            return;
        }

        _soundService.PlayEffect(Define.EffectSoundType.CheckPointEnter);
        
        switch(_checkPointType)
        {
            case CheckPointType.End:
                float clearTime = 0.0f;

                if (_stageScene != null)
                {
                    clearTime = _stageScene.GetTimer();
                }
                
                _checked = true;
                _stageService.ClearStage(_stageService.CurrentStage, clearTime);
                PlayerController controller = collision.GetComponent<PlayerController>();

                if (controller != null)
                {
                    controller.enabled = false;
                }

                break;
            case CheckPointType.Temp:
                Debug.Log("Temp CheckPoint");
                break;
        }
    }

}
