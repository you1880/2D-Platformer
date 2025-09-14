using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ItemEffectSO : ScriptableObject
{
    public abstract bool TryApplyEffect(IUserDataService reqireService, int amount = 1);
}
