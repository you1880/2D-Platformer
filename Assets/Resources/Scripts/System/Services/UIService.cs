using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AutoRegister(typeof(IUIService), priority: 10)]
public class UIService : IUIService
{
    private Dictionary<string, UI_Base> _sceneUIDict = new Dictionary<string, UI_Base>();
    private readonly IResourceService _resourceService;
    private readonly IPathProvider _pathProvider;

    public UIService(IResourceService resourceService, IPathProvider pathProvider)
    {
        _resourceService = resourceService;
        _pathProvider = pathProvider;
    }

    private GameObject _uiRoot;
    public GameObject Root
    {
        get
        {
            if (_uiRoot == null)
            {
                GameObject root = GameObject.Find("@UIRoot");

                if (root == null)
                {
                    root = new GameObject { name = "@UIRoot" };

                    Canvas canvas = root.GetOrAddComponent<Canvas>();
                    canvas.sortingOrder = 5;
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = Camera.main;
                }

                _uiRoot = root;
            }

            return _uiRoot;
        }
    }
    public T ShowUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        UI_Base ui = null;
        if (_sceneUIDict.TryGetValue(name, out ui))
        {
            if (ui == null)
            {
                _sceneUIDict.Remove(name);
            }
            else
            {
                if (!ui.gameObject.activeSelf)
                {
                    ui.gameObject.SetActive(true);
                    ui.Refresh();
                }
                
                ui.Show();

                return ui as T;
            }
        }

        string path = _pathProvider.GetUIPath(name);
        GameObject obj = _resourceService.Instantiate(path, Root.transform);
        if (obj == null)
        {
            return null;
        }

        ui = obj.GetOrAddComponent<T>();
        obj.transform.SetParent(Root.transform, false);
        ui.Show();
        _sceneUIDict[name] = ui;

        return ui as T;
    }

    public UI_MessageBox ShowMessageBox(MessageID messageID, System.Action onCompleted = null, bool isConfirmMode = false)
    {
        UI_MessageBox messageBox = ShowUI<UI_MessageBox>();
        if (messageBox == null)
        {
            return null;
        }

        string message = MessageTable.GetMessage(messageID);
        messageBox.SetMessageBox(message, onCompleted, isConfirmMode);
        
        return messageBox;
    }

    public void CloseUI(GameObject ui)
    {
        if (ui == null)
        {
            return;
        }

        ui.SetActive(false);
    }

    public void CloseMessageBox(GameObject ui)
    {
        if (ui == null)
        {
            return;
        }

        _resourceService.Destroy(ui);
        ui = null;
    }

    public void ClearUIDictionary()
    {
        _sceneUIDict.Clear();
    }
}
