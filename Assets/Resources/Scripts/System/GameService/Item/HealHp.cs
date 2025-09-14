using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemEffects/HealHp")]
public class HealHp : ItemEffectSO
{
    public override bool TryApplyEffect(IUserDataService userDataService, int amount = 1)
    {
        return userDataService.HealHp(amount);
    }
}
