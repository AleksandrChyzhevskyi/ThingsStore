using System;
using System.Collections.Generic;
using _Development.Scripts.ThingsStore.Enum;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.Data;
using BLINK.RPGBuilder.Managers;
using Object = UnityEngine.Object;

namespace _Development.Scripts.ThingsStore.Model
{
    public class PanelThingsModel : IPanelThingsModel
    {
        public event Action<StateBuyButton> InventoryEmpty;

        private readonly List<RPGItem> _items;
        private readonly PanelThingsView _panelThings;
        private readonly ItemView _itemView;
        private readonly StatItemView _statItemView;

        private Dictionary<ItemView, IItemMadel> _models;

        public PanelThingsModel(PanelThingsView panelThings, ItemView itemView, StatItemView statItemView,
            List<RPGItem> items)
        {
            _panelThings = panelThings;
            _itemView = itemView;
            _statItemView = statItemView;
            _items = items;
        }

        public void CreateItemViewInPanel()
        {
            _models = new Dictionary<ItemView, IItemMadel>();

            foreach (RPGItem rpgItem in _items)
            {
                ItemView itemView = Object.Instantiate(_itemView, _panelThings.ThingsContent);
                itemView.Initialize(rpgItem.ID);

                IItemMadel itemMadel = new ItemMadel(_statItemView, rpgItem, itemView);
                itemMadel.Create();

                _models.Add(itemView, itemMadel);
            }
        }

        public void OnEnabledPanelThingsView()
        {
            Subscribe();

            if (CheckInventory() == false)
            {
                SetBuyButtonState(StateBuyButton.Inactive);
                return;
            }

            SetBuyButtonState(StateBuyButton.Default);
        }

        public void OnDisabledPanelThingsView() => 
            Unsubscribe();

        private StateBuyButton CheckItemInInventory(RPGItem rpgItem)
        {
            foreach (EconomyData.EquippedArmor playerEntityEquippedArmor in GameState.playerEntity.equippedArmors)
            {
                RPGItem item = playerEntityEquippedArmor.item;

                if (item == null)
                    continue;

                if (item.ID == rpgItem.ID)
                    return StateBuyButton.Own;
            }

            foreach (CharacterEntries.InventorySlotEntry inventorySlotEntry in Character.Instance.CharacterData
                         .Inventory.baseSlots)
            {
                if (inventorySlotEntry.itemID == -1)
                    continue;

                RPGItem item = GameDatabase.Instance.GetItems()[inventorySlotEntry.itemID];

                if (item == null)
                    continue;

                if (item.ID == rpgItem.ID)
                    return StateBuyButton.Own;
            }

            return StateBuyButton.Active;
        }

        private void OnClickedBuyButton(RPGItem item, ItemView itemView)
        {
            if (TryBuyItem(item) == false)
                return;
            
            RPGBuilderUtilities.HandleItemLooting(item.ID,
                RPGBuilderUtilities.HandleNewItemDATA(item.ID,
                    CharacterEntries.ItemEntryState.InWorld), 1, false,
                true);

            if (CheckInventory() == false)
            {
                SetBuyButtonState(StateBuyButton.Inactive);
                return;
            }

            itemView.SetBehaviourBuyButton(StateBuyButton.Own);
        }

        private bool TryBuyItem(RPGItem rpgItem)
        {
            RPGCurrency currency = GameDatabase.Instance.GetCurrencies()[rpgItem.sellCurrencyID];

            int currencyAmount =
                Character.Instance.getCurrencyAmount(currency) - rpgItem.sellPrice;

            if (currencyAmount < 0)
                return false;

            EconomyUtilities.setCurrencyAmount(currency, currencyAmount);
            GeneralEvents.Instance.OnPlayerCurrencyChanged(currency);
            return true;
        }

        private bool CheckInventory() => 
            Character.Instance.CharacterData.Inventory.baseSlots.Count > GetCountItemsInInventory();

        private int GetCountItemsInInventory()
        {
            List<RPGItem> count = new List<RPGItem>();

            foreach (var inventorySlotEntry in Character.Instance.CharacterData.Inventory.baseSlots)
            {
                if (inventorySlotEntry.itemID == -1)
                    continue;

                RPGItem item = GameDatabase.Instance.GetItems()[inventorySlotEntry.itemID];

                if (item == null)
                    continue;

                count.Add(item);
            }

            return count.Count + 1;
        }

        private void SetBuyButtonState(StateBuyButton state)
        {
            InventoryEmpty?.Invoke(state);

            foreach (ItemView itemView in _models.Keys)
                itemView.SetBehaviourBuyButton(state == StateBuyButton.Inactive
                    ? StateBuyButton.Inactive
                    : CheckItemInInventory(itemView.Item));
        }

        private void Subscribe()
        {
            foreach (KeyValuePair<ItemView, IItemMadel> keyValuePair in _models)
            {
                keyValuePair.Value.CompareItemParametersWithInventory();
                keyValuePair.Key.ClickedBuyButton += OnClickedBuyButton;
            }
        }

        private void Unsubscribe()
        {
            foreach (KeyValuePair<ItemView, IItemMadel> keyValuePair in _models)
            {
                keyValuePair.Value.ResetStatsInItem();
                keyValuePair.Key.ClickedBuyButton -= OnClickedBuyButton;
            }
        }
    }
}