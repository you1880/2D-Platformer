using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemEffects/HealAp")]
public class HealActivityPoint : ItemEffectSO
{
    public override bool TryApplyEffect(IUserDataService userDataService, int amount = 1)
    {
        return userDataService.AddActivityPoint(amount);
    }
}
