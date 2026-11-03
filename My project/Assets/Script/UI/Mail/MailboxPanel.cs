using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailboxPanel : ModelPanel<MailboxModel>
{
    [Header("Tabs")]
    [SerializeField] private TabButton _mailButton;
    [SerializeField] private TabButton _parcelButton;

    [Header("List")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content;
    [SerializeField] private MailSlot _mailSlotPrefab;

    private readonly List<MailSlot> _slots = new List<MailSlot>();

    private MailSlotType _currentTab = MailSlotType.Mail;

    protected override void OnBound()
    {
        Model.Changed += Refresh;
    }

    protected override void OnUnbound()
    {
        Model.Changed -= Refresh;

        _slots.ForEach(slot => slot.Clicked -= OnSlotClicked);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        AddListener(_mailButton, ShowMails);
        AddListener(_parcelButton, ShowParcels);

        RefreshSlots();
        RefreshFlagNotify();
    }

    public void ShowMails()
    {
        _currentTab = MailSlotType.Mail;
        RefreshSlots();
        ResetScroll();
    }

    public void ShowParcels()
    {
        _currentTab = MailSlotType.Parcel;
        RefreshSlots();
        ResetScroll();
    }

    private void Refresh(NotifyKey key, bool isOn)
    {
        if (key.Contents != ContentsType.Mail) {
            return;
        }

        RefreshSlots();
        RefreshFlagNotify();
    }

    private void RefreshSlots()
    {
        List<MailProtocol> items = Model.GetNewItems(_currentTab);

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

    private void RefreshFlagNotify()
    {
        _mailButton.SetNotify(Model.HasNewFlag(MailSlotType.Mail));
        _parcelButton.SetNotify(Model.HasNewFlag(MailSlotType.Parcel));
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
        List<MailProtocol> items = Model.GetNewItems(_currentTab);
        int index = _slots.IndexOf(slot);

        if (index < 0 || index >= items.Count) {
            return;
        }

        Model.RemoveItem(_currentTab, index);
        RefreshSlots();
    }

    private void ResetScroll()
    {
        if (_scrollRect != null) {
            _scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
