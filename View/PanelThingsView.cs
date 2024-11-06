using System;
using _Development.Scripts.ThingsStore.Enum;
using DTT.Utils.Extensions;
using TMPro;
using UnityEngine;

public class PanelThingsView : MonoBehaviour
{
    public event Action EnabledPanelThingsView;
    public event Action DisabledPanelThingsView;

    public Transform ThingsContent;
    public Color DefaultColor;
    public Color ErrorColor;

    [SerializeField] private TMP_Text _namePanel;
    
    public TypeThings Type {get; private set; }

    private void OnEnable() => 
        EnabledPanelThingsView?.Invoke();

    private void OnDisable() => 
        DisabledPanelThingsView?.Invoke();

    public void Initialize(TypeThings type) => 
        Type = type;

    public void OnInventoryEmpty(StateBuyButton stateBuyButton)
    {
        _namePanel.color = stateBuyButton != StateBuyButton.Inactive ? DefaultColor : ErrorColor;
        _namePanel.text = stateBuyButton != StateBuyButton.Inactive ? Type.ToString() : "The inventory is full";
    }
}