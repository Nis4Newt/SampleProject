using System;
using System.Collections.Generic;
using System.Linq;

public static class NotifyManager
{
    public static event Action<bool> OnMasterNotify;
    public static event Action<ContentsType, bool> OnContentsIconNotify;

    private static readonly Dictionary<ContentsType, int> _contentsNotifyFlags = new();

    static NotifyManager()
    {
        foreach (ContentsType type in Enum.GetValues(typeof(ContentsType))) {
            _contentsNotifyFlags[type] = 0;
        }
    }

    public static void Register(INotifySource notifySource)
    {
        notifySource.Changed += SetNotifyFlag;
    }

    public static void Unregister(INotifySource notifySource)
    {
        notifySource.Changed -= SetNotifyFlag;
    }

    #region [ Notify Flag Management ]

    private static void SetNotifyFlag(NotifyKey key, bool isOn)
    {
        int flag = 1 << key.Flag;

        if (isOn) {
            _contentsNotifyFlags[key.Contents] |= flag;
        } else {
            _contentsNotifyFlags[key.Contents] &= ~flag;
        }

        RaiseMasterNotify();
        RaiseContentsIconNotify(key.Contents, _contentsNotifyFlags[key.Contents] != 0);
    }

    private static void RaiseMasterNotify()
    {
        bool hasAnyNotify = _contentsNotifyFlags.Values.Any(flag => flag != 0);
        OnMasterNotify?.Invoke(hasAnyNotify);
    }

    private static void RaiseContentsIconNotify(ContentsType type, bool isOn)
    {
        OnContentsIconNotify?.Invoke(type, isOn);
    }

    #endregion
}
