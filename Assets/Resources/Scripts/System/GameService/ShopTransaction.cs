using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTransaction
{
    //추후 인벤토리에 넣는 로직으로 변경
    public static bool TryPurchase(IInventoryService inventoryService, IUserDataService userDataService, Data.Game.Item item)
    {
        int purchaseCost = item.itemPrice;
        if (!userDataService.TrySpendGold(purchaseCost))
        {
            return false;
        }

        if (!inventoryService.TryUseConsumable(item))
        {
            userDataService.AddGold(purchaseCost);
            return false;
        }

        return true;
    }
}
