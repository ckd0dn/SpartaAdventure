using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] itemSlots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private PlayerController playerController;
    private PlayerCondition playerCondition;

    ItemData selectedItem;
    int selectedItemIndex;

    int curEquipIndex;

    // Start is called before the first frame update
    void Start()
    {
        playerController = CharacterManager.Instance.Player.controller;
        playerCondition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        playerController.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false);
        itemSlots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            itemSlots[i].index = i;
            itemSlots[i].inventory = this;
        }
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        inventoryWindow.SetActive(!IsOpen());
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        if(data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if(slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if(emptySlot != null)
        {
            emptySlot.itemData = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemData != null)
            {
                itemSlots[i].Set();
            }
            else
            {
                itemSlots[i].Clear();
            }
        } 
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].itemData == data && itemSlots[i].quantity < data.maxStackAmount)
            {
                return itemSlots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemData == null)
            {
                return itemSlots[i];
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (itemSlots[index].itemData == null) return;

        selectedItem = itemSlots[index].itemData;   
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;  

        for(int i = 0;i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !itemSlots[index].equipped);
        unEquipButton.SetActive(selectedItem.type == ItemType.Equipable && itemSlots[index].equipped);
        dropButton.SetActive(true);

    }

    public void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            for(int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch(selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        playerCondition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        playerCondition.Eat(selectedItem.consumables[i].value);
                        break;
                }
            }
        }
        RemoveSelctedItem();
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelctedItem();
    }

    void RemoveSelctedItem()
    {
        itemSlots[selectedItemIndex].quantity--;

        if (itemSlots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            itemSlots[selectedItemIndex].itemData = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow(); 
        }

        UpdateUI();
    }

    public void OnEquipButton()
    {
        if (itemSlots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        itemSlots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equipment.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        itemSlots[selectedItemIndex].equipped = false;
        CharacterManager.Instance.Player.equipment.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void UnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }

}
