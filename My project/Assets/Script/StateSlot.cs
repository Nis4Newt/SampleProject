using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SlotState
{
    Locked,
    New,
    Confirmed
}

public class StateSlot : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _noti;

    public void SetText(string value)
    {
        _text.text = value;
    }

    public void SetState(SlotState state)
    {
        switch (state) {
            case SlotState.Locked: _image.color = Color.gray; break;
            case SlotState.New: _image.color = Color.white; break;
            case SlotState.Confirmed: _image.color = Color.yellow; break;
        }

        SetNotify(state == SlotState.New);
    }

    public void AddListener(Action listener)
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => listener?.Invoke());
    }

    public void SetNotify(bool isOn)
    {
        _noti.SetActive(isOn);
    }
}
