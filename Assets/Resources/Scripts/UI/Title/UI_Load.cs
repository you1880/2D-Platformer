using System;
using System.Collections;
using System.Collections.Generic;
using Data.User;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadSlot
{
    private const string SLOT_NO_TEXT = "SaveNoText";
    private const string LAST_PLAY_TEXT = "LastPlayText";
    private const string TRASH_BUTTON = "TrashBinButton";
    public TextMeshProUGUI SaveNoText;
    public TextMeshProUGUI LastPlayText;
    public Button TrashButton;
    public Userdata Userdata;
    public int SlotId;

    public LoadSlot(TextMeshProUGUI saveNoText, TextMeshProUGUI lastPlayText, Button trashButton)
    {
        SaveNoText = saveNoText;
        LastPlayText = lastPlayText;
        TrashButton = trashButton;
    }

    public static LoadSlot CreateLoadSlot(GameObject slot)
    {
        TextMeshProUGUI saveNoText = Util.FindChild<TextMeshProUGUI>(slot, SLOT_NO_TEXT, true);
        TextMeshProUGUI lastPlayText = Util.FindChild<TextMeshProUGUI>(slot, LAST_PLAY_TEXT, true);
        Button trashButton = Util.FindChild<Button>(slot, TRASH_BUTTON, true);

        return new LoadSlot(saveNoText, lastPlayText, trashButton);
    }

    public void SetSlotData(int slotId, Userdata userData)
    {
        if (SaveNoText != null)
        {
            SaveNoText.text = $"Save No. {slotId}";
        }

        if (LastPlayText != null && userData != null)
        {
            LastPlayText.text = userData.lastSavedTime;
            Userdata = userData;
        }

        SlotId = slotId;
    }
}

public class UI_Load : UI_Base
{
    private ISaveService _gameSaveService;
    private IResourceService _resourceService;
    private ISceneService _sceneService;
    private IUIService _uiService;

    private enum GameObjects
    {
        Content,
    }

    private enum Texts
    {
        NoDataText
    }

    private enum Buttons
    {
        ExitButton
    }

    private Dictionary<int, (GameObject slotObject, LoadSlot loadSlot)> _loadSlots = new Dictionary<int, (GameObject, LoadSlot)>();
    [SerializeField] private GameObject _loadSlotPrefab;
    private GameObject _content;
    private GameObject _noDataText;
    private RectTransform _contentRect;
    private int _selectedSlot = INVALID_SLOT_NUMBER;
    private const string LOAD_SLOT_PATH = "Etc/LoadSlot";
    private const string PREFIX = "LoadSlot";
    private const int INVALID_SLOT_NUMBER = -1;

    public override void EnsureService()
    {
        _gameSaveService = ServiceLocator.GetService<ISaveService>();
        _resourceService = ServiceLocator.GetService<IResourceService>();
        _sceneService = ServiceLocator.GetService<ISceneService>();
        _uiService = ServiceLocator.GetService<IUIService>();
    }

    public override void Init()
    {
        if (_uiAnimatior == null)
        {
            _uiAnimatior = gameObject.GetOrAddComponent<UI_Animation>();
        }

        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        InstantiateLoadSlots();
    }

    public override void Refresh()
    {
        InstantiateLoadSlots();
    }

    private int GetSlotId(string slotName, string prefix = PREFIX)
    {
        if (slotName.StartsWith(prefix))
        {
            string numberString = slotName.Substring(prefix.Length);

            if (int.TryParse(numberString, out int slotNumber))
            {
                return slotNumber;
            }
        }

        return INVALID_SLOT_NUMBER;
    }

    private void OnLoadSlotClicked(PointerEventData data)
    {
        int slotId = GetSlotId(data.pointerClick.name);
        if (slotId == INVALID_SLOT_NUMBER)
        {
            return;
        }

        if (!_loadSlots.TryGetValue(slotId, out (GameObject slotObj, LoadSlot loadSlot) val))
        {
            return;
        }

        _gameSaveService.SetCurrentData(val.loadSlot.Userdata, slotId);
        _sceneService.LoadScene(Define.SceneType.Lobby);
    }

    private void OnTrashButtonClicked(PointerEventData data)
    {
        int slotId = GetSlotId(data.pointerClick.transform.parent.name);
        if (slotId == INVALID_SLOT_NUMBER)
        {
            return;
        }

        _selectedSlot = slotId;
        _uiService.ShowMessageBox(MessageID.DeleteSaveData, DeleteSlot);
    }

    private void DeleteSlot()
    {
        if (!_loadSlots.TryGetValue(_selectedSlot, out (GameObject, LoadSlot) val))
        {
            return;
        }

        if (_gameSaveService.Delete(_selectedSlot))
        {
            _resourceService.Destroy(val.Item1);
            val.Item2 = null;
            _loadSlots.Remove(_selectedSlot);
        }

        if(_loadSlots.Count == 0)
        {
            _noDataText.SetActive(true);
        }
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        _uiService.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _content = GetObject((int)GameObjects.Content);
        _contentRect = _content.GetOrAddComponent<RectTransform>();
        _noDataText = GetText((int)Texts.NoDataText).gameObject;
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    private void InstantiateLoadSlots()
    {
        if (_content == null || _contentRect == null)
        {
            return;
        }

        ClearSlots();   
        int slotCount = 0;

        for (int i = 0; i < 100; i++)
        {
            if (_gameSaveService.IsSlotUsed(i))
            {
                GameObject slot =  _resourceService.Instantiate(_loadSlotPrefab, _content.transform);
                
                if (slot == null) continue;

                LoadSlot loadSlot = LoadSlot.CreateLoadSlot(slot);
                SetLoadSlot(slot, loadSlot, i);
                _loadSlots[i] = (slot, loadSlot);
                slotCount++;
            }
        }

        if (slotCount > 0)
        {
            _noDataText.SetActive(false);
            if (slotCount > 5)
            {
                _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, 107 * slotCount);
            }
        }
    }

    private void SetLoadSlot(GameObject slotObject, LoadSlot loadSlot, int slotId)
    {
        if (loadSlot == null)
        {
            return;
        }

        slotObject.name = string.Concat(PREFIX, slotId.ToString());
        slotObject.BindEvent(OnLoadSlotClicked);
        loadSlot.SetSlotData(slotId, _gameSaveService.Load(slotId));
        loadSlot.TrashButton.gameObject.BindEvent(OnTrashButtonClicked);
    }

    private void ClearSlots()
    {
        foreach (Transform slot in _content.transform)
        {
            _resourceService.Destroy(slot.gameObject);
        }

        _loadSlots.Clear();
    }
}
