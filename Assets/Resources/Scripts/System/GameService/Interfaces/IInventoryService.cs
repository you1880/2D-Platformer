using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryService
{
    //TMP itemCode -> slotId
    bool TryUseConsumable(Data.Game.Item item);
}
