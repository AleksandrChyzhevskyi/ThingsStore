using System;
using _Development.Scripts.ThingsStore.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyButtonView : MonoBehaviour
{
    public event Action Clicked;

    [SerializeField] private TMP_Text _textButton;
    [SerializeField] private Sprite _own;
    [SerializeField] private Sprite _default;
    [SerializeField] private Button _button;

    private StateBuyButton _type = StateBuyButton.Default;

    private void OnEnable()
    {
        if (_type == StateBuyButton.Active)
            _button.onClick.AddListener(OnClicked);
    }

    private void OnDisable() => 
        _button.onClick.RemoveListener(OnClicked);


    public void SetStateOwn()
    {
        _type = StateBuyButton.Own;
        _button.image.sprite = _own;
        _textButton.text = "Own";
    }

    public void SetActiveButton(string text)
    {
        _type = StateBuyButton.Active;
        _button.image.sprite = _default;
        _textButton.text = text;
    }

    private void OnClicked() =>
        Clicked?.Invoke();
}