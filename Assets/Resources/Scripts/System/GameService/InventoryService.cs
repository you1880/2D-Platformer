using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AutoRegister(typeof(IInventoryService), priority: 30)]
public class InventoryService : IInventoryService
{
    private readonly IUserDataService _userDataService;

    public InventoryService(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    public bool TryUseConsumable(Data.Game.Item item)
    {
        return item.itemEffect?.TryApplyEffect(_userDataService) == true;
    }
}
