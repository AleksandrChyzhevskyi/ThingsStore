using _Development.Scripts.ThingsStore.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatItemView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _textStat;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _positiveEffectColor;
    [SerializeField] private Color _negativeEffectColor;

    public void SetParameters(Sprite icon, string nameStat, TypeEffectColor typeEffectColor)
    {
        _icon.sprite = icon;
        SetColor(typeEffectColor);
        _textStat.text = nameStat;
    }

    private void SetColor(TypeEffectColor typeEffectColor)
    {
        switch (typeEffectColor)
        {
            case TypeEffectColor.Default:
                _textStat.color = _defaultColor;
                break;
            case TypeEffectColor.Positive:
                _textStat.color = _positiveEffectColor;
                break;
            case TypeEffectColor.Negative:
                _textStat.color = _negativeEffectColor;
                break;
        }
    }
}