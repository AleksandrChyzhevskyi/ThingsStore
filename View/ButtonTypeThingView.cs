using System;
using _Development.Scripts.ThingsStore.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTypeThingView : MonoBehaviour
{
    public event Action<TypeThings> ClickedButton; 

    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameButton;
    [SerializeField] private Button _button;

    private TypeThings Things { get; set; }

    private void OnEnable() => 
        _button.onClick.AddListener(OnClickedButton);

    private void OnDisable() => 
        _button.onClick.RemoveListener(OnClickedButton);

    public void Initialize(Sprite icon, TypeThings type)
    {
        _icon.sprite = icon;
        _nameButton.text = type.ToString();
        Things = type;
    }

    private void OnClickedButton() => 
        ClickedButton?.Invoke(Things);
}
