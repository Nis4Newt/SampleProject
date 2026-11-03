using System;

public class NotifyKey
{
    public ContentsType Contents { get; }
    public int Flag { get; }

    private NotifyKey(ContentsType contents, int flag)
    {
        Contents = contents;
        Flag = flag;
    }

    public static NotifyKey Of<TFlag>(ContentsType contents, TFlag flag) where TFlag : Enum
        => new NotifyKey(contents, Convert.ToInt32(flag));
}

public interface INotifySource
{
    public event Action<NotifyKey, bool> Changed;
}
