using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailboxPanel : BasePanel
{
    [Header("Tabs")]
    [SerializeField] private TabButton _mailButton;
    [SerializeField] private TabButton _parcelButton;

    [Header("List")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content;
    [SerializeField] private MailSlot _mailSlotPrefab;

    private readonly List<MailSlot> _slots = new List<MailSlot>();
    private readonly List<MailProtocol> _mails = new List<MailProtocol>();
    private readonly List<MailProtocol> _parcels = new List<MailProtocol>();

    private MailSlotType _currentTab = MailSlotType.Mail;

    private void OnDestroy()
    {
        _slots.ForEach(slot => slot.Clicked -= OnSlotClicked);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        AddListener(_mailButton, ShowMails);
        AddListener(_parcelButton, ShowParcels);

        Refresh();
    }

    public void OnMailProtocol(MailProtocol protocol)
    {
        switch (protocol.Type) {
            case MailSlotType.Mail:
                AddMail(protocol.Content);
                break;
            case MailSlotType.Parcel:
                AddParcel(protocol.Content, protocol.ItemType);
                break;
        }
    }

    public void AddMail(string mailContent)
    {
        _mails.Add(new MailProtocol(MailSlotType.Mail, mailContent));
        RefreshIfShowing(MailSlotType.Mail);
    }

    public void AddParcel(string parcelContent, ParcelItem itemType)
    {
        if (!IsValidParcelItem(itemType)) {
            Debug.LogWarning($"MailboxPanel.AddParcel: 잘못된 택배 아이템({itemType})이라 무시합니다.");
            return;
        }

        _parcels.Add(new MailProtocol(MailSlotType.Parcel, parcelContent, itemType));
        RefreshIfShowing(MailSlotType.Parcel);
    }

    public void ShowMails()
    {
        _currentTab = MailSlotType.Mail;
        Refresh();
        ResetScroll();
    }

    public void ShowParcels()
    {
        _currentTab = MailSlotType.Parcel;
        Refresh();
        ResetScroll();
    }

    private static bool IsValidParcelItem(ParcelItem itemType)
    {
        return itemType != ParcelItem.None && (itemType & ~ParcelItem.Both) == 0;
    }

    private void RefreshIfShowing(MailSlotType type)
    {
        if (isActiveAndEnabled && _currentTab == type) {
            Refresh();
        }
    }

    private void Refresh()
    {
        List<MailProtocol> items = _currentTab == MailSlotType.Mail ? _mails : _parcels;

        EnsureSlotCount(items.Count);

        for (int i = 0; i < _slots.Count; i++) {
            MailSlot slot = _slots[i];
            bool used = i < items.Count;
            slot.gameObject.SetActive(used);

            if (!used) {
                continue;
            }

            MailProtocol item = items[i];
            if (item.Type == MailSlotType.Mail) {
                slot.SetMail(item.Content);
            } else {
                slot.SetParcel(item.Content, item.ItemType);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
    }

    private void EnsureSlotCount(int count)
    {
        while (_slots.Count < count) {
            MailSlot slot = Instantiate(_mailSlotPrefab, _content);
            slot.Clicked += OnSlotClicked;
            _slots.Add(slot);
        }
    }

    private void OnSlotClicked(MailSlot slot)
    {
        List<MailProtocol> items = _currentTab == MailSlotType.Mail ? _mails : _parcels;
        int index = _slots.IndexOf(slot);

        if (index < 0 || index >= items.Count) {
            return;
        }

        items.RemoveAt(index);
        Refresh();
    }

    private void ResetScroll()
    {
        if (_scrollRect != null) {
            _scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
