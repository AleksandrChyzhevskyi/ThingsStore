using System.Collections.Generic;
using System.Linq;
using _Development.Scripts.Boot;
using _Development.Scripts.ThingsStore.Data;
using _Development.Scripts.ThingsStore.Enum;
using _Development.Scripts.ThingsStore.Model;
using UnityEngine;

public class LauncherThingsStore : MonoBehaviour
{
    public PanelThingsStoreView PanelThingsStore;
    public PanelThingsView PanelThings;
    public ButtonTypeThingView ButtonTypeThing;
    public ItemView Item;
    public StatItemView StatItem;

    private Dictionary<PanelThingsView, IPanelThingsModel> _panelThingsModels;
    private List<ButtonTypeThingView> _buttons;
    private PanelThingsView _currentPanel;

    public void LaunchThingsStore()
    {
        _panelThingsModels = new Dictionary<PanelThingsView, IPanelThingsModel>();
        _buttons = new List<ButtonTypeThingView>();

        CreateAllItemsButton();

        foreach (TypeContentThings things in Game.instance.GetThingsStoreData().FillingStore)
        {
            ButtonTypeThingView buttonTypeThingView =
                CreateViewElements(out var panelThings, out var panelThingsModel, things.Type, things.Items);

            panelThings.gameObject.SetActive(false);

            _panelThingsModels.Add(panelThings, panelThingsModel);
            _buttons.Add(buttonTypeThingView);
        }

        Subscribe();
    }

    private void Subscribe()
    {
        foreach (ButtonTypeThingView buttonTypeThingView in _buttons)
            buttonTypeThingView.ClickedButton += OnClickedButtonThings;

        foreach (KeyValuePair<PanelThingsView, IPanelThingsModel> panelThingsModel in _panelThingsModels)
        {
            panelThingsModel.Key.EnabledPanelThingsView += panelThingsModel.Value.OnEnabledPanelThingsView;
            panelThingsModel.Key.DisabledPanelThingsView += panelThingsModel.Value.OnDisabledPanelThingsView;
            panelThingsModel.Value.InventoryEmpty += panelThingsModel.Key.OnInventoryEmpty;
        }
    }

    public void Unsubscribe()
    {
        foreach (ButtonTypeThingView buttonTypeThingView in _buttons)
            buttonTypeThingView.ClickedButton -= OnClickedButtonThings;

        foreach (KeyValuePair<PanelThingsView, IPanelThingsModel> panelThingsModel in _panelThingsModels)
        {
            panelThingsModel.Key.EnabledPanelThingsView -= panelThingsModel.Value.OnEnabledPanelThingsView;
            panelThingsModel.Key.DisabledPanelThingsView -= panelThingsModel.Value.OnDisabledPanelThingsView;
            panelThingsModel.Value.InventoryEmpty -= panelThingsModel.Key.OnInventoryEmpty;
        }
    }

    private void CreateAllItemsButton()
    {
        List<RPGItem> items = new List<RPGItem>();

        foreach (TypeContentThings things in Game.instance.GetThingsStoreData().FillingStore)
        {
            foreach (var item in things.Items.Where(item => items.Contains(item) == false))
                items.Add(item);
        }

        ButtonTypeThingView buttonTypeThingView =
            CreateViewElements(out var panelThings, out var panelThingsModel, TypeThings.AllItems, items);

        _currentPanel = panelThings;

        _panelThingsModels.Add(panelThings, panelThingsModel);
        _buttons.Add(buttonTypeThingView);
    }

    private ButtonTypeThingView CreateViewElements(out PanelThingsView panelThings,
        out IPanelThingsModel panelThingsModel,
        TypeThings type, List<RPGItem> items)
    {
        ButtonTypeThingView buttonTypeThingView =
            CreateButtonTypeThingView(ButtonTypeThing, PanelThingsStore.ButtonThingsContent,
                Game.instance.GetThingsStoreData().Icon, type);

        panelThings = CreatePanelThingsView(PanelThings, PanelThingsStore.ThingsContent, type);
        panelThingsModel = CreatePanelThingsModel(panelThings, Item, StatItem, items);
        return buttonTypeThingView;
    }

    private PanelThingsView CreatePanelThingsView(PanelThingsView panel, Transform thingsContent, TypeThings type)
    {
        PanelThingsView panelThings = Instantiate(panel, thingsContent);
        panelThings.Initialize(type);
        return panelThings;
    }

    private IPanelThingsModel CreatePanelThingsModel(PanelThingsView panelThings, ItemView itemView,
        StatItemView statItem, List<RPGItem> items)
    {
        IPanelThingsModel panelThingsModel = new PanelThingsModel(panelThings, itemView, statItem, items);
        panelThingsModel.CreateItemViewInPanel();
        return panelThingsModel;
    }

    private ButtonTypeThingView CreateButtonTypeThingView(ButtonTypeThingView buttonTypeThing,
        Transform buttonThingsContent, Sprite icon, TypeThings type)
    {
        ButtonTypeThingView buttonTypeThingView =
            Instantiate(buttonTypeThing, buttonThingsContent);
        buttonTypeThingView.Initialize(icon, type);
        return buttonTypeThingView;
    }

    private void OnClickedButtonThings(TypeThings things)
    {
        _currentPanel.gameObject.SetActive(false);
        _currentPanel = _panelThingsModels.Keys.FirstOrDefault(panelThingsView => panelThingsView.Type == things);
        _currentPanel.gameObject.SetActive(true);
    }
}