using System;

[Flags]
public enum ParcelItem
{
    None = 0,
    First = 1,
    Second = 2,
    Both = First | Second
}

public readonly struct MailProtocol
{
    public readonly MailSlotType Type;
    public readonly string Content;
    public readonly ParcelItem ItemType;

    public MailProtocol(MailSlotType type, string content, ParcelItem itemType = ParcelItem.None)
    {
        Type = type;
        Content = content;
        ItemType = itemType;
    }
}
