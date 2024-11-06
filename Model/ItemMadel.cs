using System.Collections.Generic;
using System.Linq;
using _Development.Pool;
using _Development.Scripts.Boot;
using _Development.Scripts.ThingsStore.Enum;
using BLINK.RPGBuilder.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Development.Scripts.ThingsStore.Model
{
    public class ItemMadel : IItemMadel
    {
        private readonly IPoolMono<StatItemView> _statItemViewPool;
        private readonly StatItemView _statItemView;
        private readonly RPGItem _item;
        private readonly ItemView _itemView;

        private Dictionary<RPGItem.ITEM_STATS, float> _statsGains;
        private Dictionary<RPGItem.ITEM_STATS, float> _statsLoose;

        public ItemMadel(StatItemView statItemView, RPGItem item, ItemView itemView)
        {
            _statItemView = statItemView;
            _item = item;
            _itemView = itemView;

            _statItemViewPool =
                new PoolMono<StatItemView>(_statItemView, 5, true,
                    _itemView.NegativeStats.DifferenceParametersOfThings);
        }

        public void Create()
        {
            foreach (RPGItem.ITEM_STATS stat in _item.stats)
            {
                StatItemView statItemView =
                    Object.Instantiate(_statItemView, _itemView.NegativeStats.ItemParametersContent);
                statItemView.SetParameters(GetIcon(stat.statID),
                    SetTextInParameter(true, stat),
                    TypeEffectColor.Default);
            }
        }

        public void CompareItemParametersWithInventory()
        {
            if (_item.ArmorSlot == null)
                return;

            _statsGains = new Dictionary<RPGItem.ITEM_STATS, float>();
            _statsLoose = new Dictionary<RPGItem.ITEM_STATS, float>();

            foreach (EconomyData.EquippedArmor playerEntityEquippedArmor in GameState.playerEntity.equippedArmors)
            {
                if (playerEntityEquippedArmor.ArmorSlot.entryName != _item.ArmorSlot.entryName)
                    continue;

                RPGItem rpgItem = playerEntityEquippedArmor.item;

                if (rpgItem == null)
                    continue;

                foreach (RPGItem.ITEM_STATS rpgItemStat in rpgItem.stats)
                {
                    RPGItem.ITEM_STATS defaultStats =
                        _item.stats.FirstOrDefault(stats => stats.statID == rpgItemStat.statID);

                    if (defaultStats != null)
                    {
                        float result = defaultStats.amount - rpgItemStat.amount;

                        switch (result)
                        {
                            case < 0:
                                _statsLoose.TryAdd(rpgItemStat, result);
                                break;
                            case >= 0:
                                _statsGains.TryAdd(defaultStats, result);
                                break;
                        }
                    }
                    else
                        _statsLoose.TryAdd(rpgItemStat, rpgItemStat.amount);
                }
            }

            foreach (RPGItem.ITEM_STATS itemStats in _item.stats)
            {
                if (_statsGains.ContainsKey(itemStats))
                {
                    if (_statsGains[itemStats] == 0)
                        _statsGains.Remove(itemStats);

                    continue;
                }

                _statsGains.TryAdd(itemStats, itemStats.amount);
            }

            CreateStatView(_statsGains, true);

            if (_statsLoose.Any() == false && _statsGains.Any() == false)
            {
                _itemView.NegativeStats.SetBehaviourDifferenceParametersOfThings(false);
                return;
            }

            _itemView.NegativeStats.SetBehaviourDifferenceParametersOfThings(true);
            CreateStatView(_statsLoose, false);
        }

        public void ResetStatsInItem()
        {
            foreach (StatItemView statItemView in _statItemViewPool.GetAllListElements())
                statItemView.gameObject.SetActive(false);
        }

        private void CreateStatView(IEnumerable<KeyValuePair<RPGItem.ITEM_STATS, float>> stats, bool isPositive)
        {
            foreach (KeyValuePair<RPGItem.ITEM_STATS, float> keyValuePair in stats)
            {
                if (keyValuePair.Value == 0)
                    continue;

                StatItemView statItemView = _statItemViewPool.GetFreeElement();
                statItemView.SetParameters(GetIcon(keyValuePair.Key.statID),
                    SetTextInParameter(isPositive, keyValuePair.Key),
                    isPositive ? TypeEffectColor.Positive : TypeEffectColor.Negative);
            }
        }

        private string SetTextInParameter(bool isPositive, RPGItem.ITEM_STATS stat)
        {
            string percent = "%";
            string notPercent = " ";
            string positive = "+";
            string notPositive = "-";

            return $"{(isPositive ? positive : notPositive)}" +
                   $"{stat.amount.ToString()}" +
                   $"{(stat.isPercent ? percent : notPercent)}";
        }

        private Sprite GetIcon(int ID) =>
            (from upgradeData in Game.instance.GetStaticData().UpgradesDate
                where upgradeData.ID == ID
                select upgradeData.IconUpgrade).FirstOrDefault();
    }
}