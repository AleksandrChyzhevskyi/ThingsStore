using System;
using _Development.Scripts.ThingsStore.Enum;
using BLINK.RPGBuilder.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    public event Action<RPGItem, ItemView> ClickedBuyButton;

    public NegativeStatsView NegativeStats;

    [SerializeField] private BuyButtonView _buyButton;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameItom;

    public RPGItem Item { get; private set; }

    private void OnEnable() =>
        _buyButton.Clicked += OnClickedBuyButton;

    private void OnDisable() =>
        _buyButton.Clicked -= OnClickedBuyButton;

    public void Initialize(int iDItem)
    {
        Item = GameDatabase.Instance.GetItems()[iDItem];
        _icon.sprite = Item.entryIcon;
        _nameItom.text = Item.entryDisplayName;

        SetBehaviourBuyButton(StateBuyButton.Active);
    }

    public void SetBehaviourBuyButton(StateBuyButton stat)
    {
        switch (stat)
        {
            case StateBuyButton.Own:
                _buyButton.Clicked -= OnClickedBuyButton;
                _buyButton.SetStateOwn();
                break;
            case StateBuyButton.Active:
                _buyButton.SetActiveButton(
                    $"<sprite name={GameDatabase.Instance.GetCurrencies()[Item.sellCurrencyID].entryName}> {Item.sellPrice}");
                break;
            case StateBuyButton.Inactive:
                _buyButton.Clicked -= OnClickedBuyButton;
                _buyButton.gameObject.SetActive(false);
                break;
        }
    }

    private void OnClickedBuyButton() =>
        ClickedBuyButton?.Invoke(Item, this);
}