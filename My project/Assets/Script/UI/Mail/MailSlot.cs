using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MailSlotType
{
    Mail,
    Parcel
}

public class MailSlot : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button _button;

    [Header("Display")]
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private GameObject _firstItem;
    [SerializeField] private GameObject _secondItem;

    public event Action<MailSlot> Clicked;

    private void OnEnable()
    {
        _button.onClick.AddListener(NotifyClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(NotifyClicked);
    }

    private void NotifyClicked()
    {
        Clicked?.Invoke(this);
    }

    public void SetMail(string content)
    {
        SetParcel(content, ParcelItem.None);
    }

    public void SetParcel(string content, ParcelItem itemType)
    {
        _contentText.text = content;
        _firstItem.SetActive((itemType & ParcelItem.First) != 0);
        _secondItem.SetActive((itemType & ParcelItem.Second) != 0);
    }
}
