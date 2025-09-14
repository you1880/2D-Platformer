using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IBaseState
{
    private PlayerStateMachine _playerStateMachine;

    public PlayerIdleState(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
    }

    public void EnterState()
    {
    }
    
    public void ExitState() { }
    public void UpdateState()
    { 
        
    }
}
