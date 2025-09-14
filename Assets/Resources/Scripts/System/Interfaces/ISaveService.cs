using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISaveService
{
    Data.User.Userdata CurrentData { get; }
    bool CheckValidSlotIndex(int slotId);
    bool IsSlotUsed(int slotId);
    bool Save(int slotId, Data.User.Userdata userData);
    bool Save();
    Data.User.Userdata Load(int slotId);
    Data.User.Userdata CreateNewData(int characterType);
    bool Delete(int slotId);
    void SetCurrentData(Data.User.Userdata userData, int slotId);
}
