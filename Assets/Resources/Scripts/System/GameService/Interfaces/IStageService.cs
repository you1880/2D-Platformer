using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStageService
{
    Define.Stage CurrentStage { get; }
    string GetStageClearTime(Define.Stage stage);
    bool IsStageClear(Define.Stage stage);
    bool CanEnterStage(Define.Stage stage);
    void EnterStage(Define.Stage stage);
    bool SkipStage(Define.Stage stage);
    void ClearStage(Define.Stage stage, float clearTime);
    void FailStage(Define.Stage stage);
}
