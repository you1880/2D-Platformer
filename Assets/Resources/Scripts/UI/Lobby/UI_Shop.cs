using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot
{
    private const string ITEM_IMAGE_NAME = "ItemImage";
    private const string ITEM_NAME_TEXT_NAME = "ItemText";
    private const string ITEM_PRICE_TEXT_NAME = "ItemPrice";
    private Image _itemImage;
    private TextMeshProUGUI _itemNameText;
    private TextMeshProUGUI _itemPriceText;
    private Data.Game.Item _slotItem;

    public ShopSlot(Image itemImage, TextMeshProUGUI itemNameText, TextMeshProUGUI itemPriceText)
    {
        _itemImage = itemImage;
        _itemNameText = itemNameText;
        _itemPriceText = itemPriceText;
    }

    public static ShopSlot CreateShopSlot(GameObject slotObject)
    {
        Image itemImage = Util.FindChild<Image>(slotObject, ITEM_IMAGE_NAME);
        TextMeshProUGUI itemNameText = Util.FindChild<TextMeshProUGUI>(slotObject, ITEM_NAME_TEXT_NAME);
        TextMeshProUGUI itemPriceText = Util.FindChild<TextMeshProUGUI>(slotObject, ITEM_PRICE_TEXT_NAME);

        return new ShopSlot(itemImage, itemNameText, itemPriceText);
    }

    public void SetSlotItem(Data.Game.Item item, Sprite itemSprite)
    {
        if (item == null)
        {
            return;
        }

        _slotItem = item;

        _itemImage.sprite = itemSprite;
        _itemImage.color = Color.white;

        _itemNameText.text = item.itemName;
        _itemPriceText.text = $"{item.itemPrice:N0}";
    }
    
    public Data.Game.Item GetSlotItem() => _slotItem;
}

public class UI_Shop : UI_Base
{
    private IDataService _dataService;
    private IInventoryService _inventoryService;
    private IResourceService _resourceService;
    private IUIService _uiService;
    private IUserDataService _userDataService;


    private enum GameObjects
    {
        Consumable,
        ConsumableContents,
        Equipment,
        EquipmentContents
    }

    private enum Texts
    {
        ConsumableButtonText,
        EquipmentButtonText,
        PlayerGoldText
    }

    private enum Buttons
    {
        ConsumableButton,
        EquipmentButton,
        ExitButton
    }

    // Key : itemCode
    private Dictionary<int, ShopSlot> _shopSlots = new Dictionary<int, ShopSlot>();
    private enum ShopCategory { None, Consumable, Equipment }
    private Color _selectedColor = Color.white;
    private Color _unselectedColor = Color.gray;
    [SerializeField] private GameObject _shopSlot;
    private GameObject _consumablePanel;
    private GameObject _consumableContents;
    private GameObject _equipmentPanel;
    private GameObject _equipmentContents;
    private TextMeshProUGUI _consumableButtonText;
    private TextMeshProUGUI _equipmentButtonText;
    private TextMeshProUGUI _playerGoldText;
    private ShopCategory _currentCategory = ShopCategory.None; 
    private const string SHOP_SLOT_PREFIX = "ShopSlot";
    private const int INVALID_SLOT_NUMBER = -1;

    public override void EnsureService()
    {
        _dataService = ServiceLocator.GetService<IDataService>();
        _inventoryService = ServiceLocator.GetService<IInventoryService>();
        _resourceService = ServiceLocator.GetService<IResourceService>();
        _uiService = ServiceLocator.GetService<IUIService>();
        _userDataService = ServiceLocator.GetService<IUserDataService>();
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        BuildShopSlots();
        OnCategoryButtonClicked(null, ShopCategory.Consumable);
        PlayerGoldTextUpdate(_userDataService.Gold);
        BindEvent();
    }

    public override void Refresh()
    {
        BindEvent();
        PlayerGoldTextUpdate(_userDataService.Gold);
    }

    private int GetSlotId(string slotName, string prefix = SHOP_SLOT_PREFIX)
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

    private void PlayerGoldTextUpdate(int gold)
    {
        if (_playerGoldText == null)
        {
            return;
        }

        _playerGoldText.text = $"{gold:N0}";
    }

    private void OnShopSlotClicked(PointerEventData data)
    {
        int slotId = GetSlotId(data.pointerClick.name);
        if (!_shopSlots.TryGetValue(slotId, out ShopSlot slot))
        {
            return;
        }

        Data.Game.Item item = slot.GetSlotItem();
        if (item == null)
        {
            return;
        }

        //TMP
        //InventoryService의 TryUseItem을 임시로 바로 사용
        //추후 InventoryContainer로 이동, Inventory내에서 사용
        if (ShopTransaction.TryPurchase(_inventoryService, _userDataService, item) == false)
        {
            return;
        }

        _soundService.PlayEffect(Define.EffectSoundType.UI_Shop);
    }

    private void OnCategoryButtonClicked(PointerEventData data, ShopCategory category)
    {
        if (_currentCategory == category)
        {
            return;
        }

        if (_consumablePanel == null || _equipmentPanel == null || _consumableButtonText == null || _equipmentButtonText == null)
        {
            return;
        }

        _currentCategory = category;

        _consumablePanel.SetActive(_currentCategory == ShopCategory.Consumable);
        _equipmentPanel.SetActive(_currentCategory == ShopCategory.Equipment);
        _consumableButtonText.color = _currentCategory == ShopCategory.Consumable ? _selectedColor : _unselectedColor;
        _equipmentButtonText.color = _currentCategory == ShopCategory.Equipment ? _selectedColor : _unselectedColor;
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
        _consumablePanel = GetObject((int)GameObjects.Consumable);
        _consumableContents = GetObject((int)GameObjects.ConsumableContents);
        _equipmentPanel = GetObject((int)GameObjects.Equipment);
        _equipmentContents = GetObject((int)GameObjects.EquipmentContents);
        _consumableButtonText = GetText((int)Texts.ConsumableButtonText);
        _equipmentButtonText = GetText((int)Texts.EquipmentButtonText);
        _playerGoldText = GetText((int)Texts.PlayerGoldText);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ConsumableButton).gameObject.BindEvent((data) => OnCategoryButtonClicked(data, ShopCategory.Consumable));
        GetButton((int)Buttons.EquipmentButton).gameObject.BindEvent((data) => OnCategoryButtonClicked(data, ShopCategory.Equipment));
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    private void BuildShopSlots()
    {
        IReadOnlyDictionary<int, Data.Game.Item> items = _dataService.GetAllItems();

        foreach (var item in items)
        {
            if (item.Value.itemType == Define.ItemType.Consumable)
                CreateShopSlot(item.Value, _consumableContents.transform);
            else if (item.Value.itemType == Define.ItemType.Equipment)
                CreateShopSlot(item.Value, _equipmentContents.transform);
        }
    }

    private void CreateShopSlot(Data.Game.Item item, Transform transform)
    {
        if (item == null || transform == null || _shopSlot == null)
        {
            return;
        }

        GameObject slotObject = _resourceService.Instantiate(_shopSlot, transform);
        ShopSlot shopSlot = ShopSlot.CreateShopSlot(slotObject);

        if (shopSlot != null)
        {
            Sprite itemSprite = _dataService.GetItemSprite(item.itemCode);
            shopSlot.SetSlotItem(item, itemSprite);

            slotObject.name = $"{SHOP_SLOT_PREFIX}{item.itemCode}";
            slotObject.BindEvent(OnShopSlotClicked);
            _shopSlots[item.itemCode] = shopSlot;
        }
    }

    private void BindEvent()
    {
        _userDataService.OnGoldChanged += PlayerGoldTextUpdate;
    }

    private void UnbindEvent()
    {
        _userDataService.OnGoldChanged -= PlayerGoldTextUpdate;
    }

    private void OnDisable()
    {
        UnbindEvent();
    }
}
