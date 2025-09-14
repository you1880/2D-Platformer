using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceService
{
    T Load<T>(string path) where T : UnityEngine.Object;
    Sprite LoadItemSprite(string spriteName);
    GameObject Instantiate(string path, Transform parent = null);
    GameObject Instantiate(GameObject prefab, Transform parent = null);
    void Destroy(GameObject obj, float time = 0.0f);    

}
