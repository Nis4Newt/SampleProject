using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ExpansionButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Sprites")]
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;

    [Header("State")]
    [SerializeField] private bool _isOn;
    [SerializeField] private GameObject _noti;

    private Image _image;
    private Action<bool> _onClicked;

    public bool IsOn => _isOn;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _noti.SetActive(false);
        Refresh();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isOn = !_isOn;
        Refresh();
        _onClicked?.Invoke(_isOn);
    }

    public void SetOn(bool value)
    {
        _isOn = value;
        Refresh();
    }

    public void AddClickListener(Action<bool> listener)
    {
        _onClicked += listener;
    }

    public void RemoveClickListener(Action<bool> listener)
    {
        _onClicked -= listener;
    }

    public void SetNotify(bool isOn)
    {
        _noti.SetActive(isOn);
    }

    private void Refresh()
    {
        if (_image != null) {
            _image.sprite = _isOn ? _onSprite : _offSprite;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_image == null) {
            _image = GetComponent<Image>();
        }
        Refresh();
    }
#endif
}
