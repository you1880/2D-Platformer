using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}
