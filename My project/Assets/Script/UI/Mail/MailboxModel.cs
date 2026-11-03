using System;
using System.Collections.Generic;
using UnityEngine;

public class MailboxModel : INotifySource
{
    public enum MailFlag
    {
        HasMail,
        HasParcel
    }

    private readonly List<MailProtocol> _mails = new List<MailProtocol>();
    private readonly List<MailProtocol> _parcels = new List<MailProtocol>();

    public event Action<NotifyKey, bool> Changed;

    #region [ Read Access ]

    public List<MailProtocol> GetNewItems(MailSlotType slotType)
    {
        return slotType == MailSlotType.Mail ? _mails : _parcels;
    }

    public bool HasNewFlag(MailSlotType slotType)
    {
        return GetNewItems(slotType).Count > 0;
    }

    #endregion

    #region [ Control Items ]

    public void AddMail(string mailContent)
    {
        _mails.Add(new MailProtocol(MailSlotType.Mail, mailContent));
        NotifyChanged(MailFlag.HasMail, true);
    }

    public void AddParcel(string parcelContent, ParcelItem itemType)
    {
        if (!IsValidParcelItem(itemType)) {
            Debug.LogWarning($"MailboxModel.AddParcel: 잘못된 택배 아이템({itemType})이라 무시합니다.");
            return;
        }

        _parcels.Add(new MailProtocol(MailSlotType.Parcel, parcelContent, itemType));
        NotifyChanged(MailFlag.HasParcel, true);
    }

    public void RemoveItem(MailSlotType slotType, int index)
    {
        var items = GetNewItems(slotType);
        if (index < 0 || index >= items.Count) {
            return;
        }

        items.RemoveAt(index);
        if (items.Count == 0) {
            var flag = slotType == MailSlotType.Mail ? MailFlag.HasMail : MailFlag.HasParcel;
            NotifyChanged(flag, false);
        }
    }

    #endregion

    #region [ Mail Protocol ]

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

    #endregion

    #region [ Helpers ]

    private void NotifyChanged(MailFlag flag, bool isOn)
    {
        Changed?.Invoke(NotifyKey.Of(ContentsType.Mail, flag), isOn);
    }

    private static bool IsValidParcelItem(ParcelItem itemType)
    {
        return itemType != ParcelItem.None && (itemType & ~ParcelItem.Both) == 0;
    }

    #endregion
}
