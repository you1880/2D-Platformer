using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataRegistry<TId, TDef>
{
    private Dictionary<TId, TDef> _dataDIct = new Dictionary<TId, TDef>();
    public IReadOnlyDictionary<TId, TDef> Dict => _dataDIct;

    public void LoadData(IEnumerable<TDef> defs, Func<TDef, TId> keySelector)
    {
        _dataDIct.Clear();

        foreach (var def in defs)
        {
            var key = keySelector(def);
            _dataDIct[key] = def;
        }
    }

    public bool TryGet(TId id, out TDef def)
        => _dataDIct.TryGetValue(id, out def);
}
