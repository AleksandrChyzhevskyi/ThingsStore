using System;
using _Development.Scripts.ThingsStore.Enum;

namespace _Development.Scripts.ThingsStore.Model
{
    public interface IPanelThingsModel
    {
        event Action<StateBuyButton> InventoryEmpty;
        void CreateItemViewInPanel();
        void OnEnabledPanelThingsView();
        void OnDisabledPanelThingsView();
    }
}