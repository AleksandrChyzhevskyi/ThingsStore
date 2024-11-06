using BLINK.RPGBuilder.LogicMono;
using UnityEngine;

namespace _Development.Scripts.ThingsStore.Infrastructure
{
    public class ItemShopActivator : MonoBehaviour
    {
        private PanelThingsStoreView _panelThingsStoreView;

        private void Start() => 
            _panelThingsStoreView = RPGBuilderEssentials.Instance.LauncherThingsStore.PanelThingsStore;

        public void Show() => 
            _panelThingsStoreView.gameObject.SetActive(true);
    }
}