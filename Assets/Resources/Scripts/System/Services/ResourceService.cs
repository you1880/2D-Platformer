using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AutoRegister(typeof(IResourceService), priority: 5)]
public class ResourceService : IResourceService
{
    private IPathProvider _pathProvider;
    //private IDataService _dataService;
    private Dictionary<string, Sprite> _itemSpriteDict = new Dictionary<string, Sprite>();
    private UnityEngine.U2D.SpriteAtlas _itemSpriteAtlas;

    public ResourceService(IPathProvider pathProvider)
    {
        //_dataService = dataService;
        _pathProvider = pathProvider;

        InitItemSpriteDict();
    }

    public T Load<T>(string path) where T : Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"Invalid Path : {path}");
            return null;
        }

        return Resources.Load<T>(path);
    }

    public Sprite LoadItemSprite(string spriteName)
    {
        if (_itemSpriteDict.TryGetValue(spriteName, out Sprite sprite))
        {
            return sprite;
        }
        
        return null;
    }

    public GameObject Instantiate(string name, Transform parent = null)
    {
        string path = _pathProvider.GetPrefabPath(name);
        GameObject original = Load<GameObject>(path);

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject obj = UnityEngine.Object.Instantiate(original, parent);
        obj.name = original.name;

        return obj;
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.Log($"Prefab is null");
            return null;
        }

        GameObject obj = UnityEngine.Object.Instantiate(prefab, parent);
        obj.name = prefab.name;

        return obj;
    }

    public void Destroy(GameObject obj, float time = 0)
    {
        if (obj == null)
        {
            return;
        }

        UnityEngine.Object.Destroy(obj, time);
    }

    private void InitItemSpriteDict()
    {
        _itemSpriteAtlas = Load<UnityEngine.U2D.SpriteAtlas>(_pathProvider.GetSpriteAtlasPath());
        if (_itemSpriteAtlas == null)
        {
            return;
        }

        _itemSpriteDict.Clear();

        Sprite[] sprites = new Sprite[_itemSpriteAtlas.spriteCount];
        _itemSpriteAtlas.GetSprites(sprites);

        foreach (Sprite sprite in sprites)
        {
            string spriteName = sprite.name.Replace("(Clone)", "");
            _itemSpriteDict[spriteName] = sprite;
        }
    }
}
