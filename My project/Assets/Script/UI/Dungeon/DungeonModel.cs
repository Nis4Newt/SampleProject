using System;
using System.Collections.Generic;
using System.Linq;

public class DungeonProgress
{
    public int ConfirmedLevel = 1;
}

public class DungeonModel : INotifySource
{
    public enum DungeonFlag
    {
        NewDungeon
    }

    private readonly Dictionary<DungeonType, Dictionary<int, DungeonProgress>> _dungeonStates = new();

    public int UserLevel { get; private set; } = 1;

    public event Action<NotifyKey, bool> Changed;

    public DungeonModel()
    {
        foreach (DungeonType type in Enum.GetValues(typeof(DungeonType))) {
            _dungeonStates[type] = new Dictionary<int, DungeonProgress>();
        }

        foreach (var tab in DungeonData) {
            foreach (var banner in tab.Value) {
                _dungeonStates[tab.Key][banner.Key] = new DungeonProgress();
            }
        }
    }

    #region [ Get NewDungeon List ]

    public List<int> GetNewDungeons(DungeonType type, int bannerKey)
    {
        return GetNewDungeonsByLevel(type, bannerKey).ToList();
    }

    public List<int> GetBannersWithNewDungeon(DungeonType type)
    {
        return _dungeonStates[type].Keys
            .Where(bannerKey => GetNewDungeonsByLevel(type, bannerKey).Any())
            .ToList();
    }

    public List<DungeonType> GetTabsWithNewDungeon()
    {
        return _dungeonStates.Keys
            .Where(type => GetBannersWithNewDungeon(type).Count > 0)
            .ToList();
    }

    private IEnumerable<int> GetNewDungeonsByLevel(DungeonType type, int bannerKey)
    {
        int confirmedLevel = _dungeonStates[type][bannerKey].ConfirmedLevel;
        return DungeonData[type][bannerKey].values
            .Where(level => level > confirmedLevel && level <= UserLevel);
    }

    #endregion

    #region [ Confirm Dungeons ]

    public void ConfirmAllDungeons(DungeonType type, int bannerKey)
    {
        _dungeonStates[type][bannerKey].ConfirmedLevel = UserLevel;
        NotifyChanged();
    }

    public void ConfirmOneDungeons(DungeonType type, int bannerKey, int level)
    {
        var progress = _dungeonStates[type][bannerKey];
        progress.ConfirmedLevel = Math.Max(progress.ConfirmedLevel, level);
        NotifyChanged();
    }

    #endregion

    #region [ Dungeon Protocol ]

    public void ChangeUserLevel(int level)
    {
        UserLevel = level;
        NotifyChanged();
    }

    #endregion

    private void NotifyChanged()
    {
        var key = NotifyKey.Of(ContentsType.Dungeon, DungeonFlag.NewDungeon);
        Changed?.Invoke(key, GetTabsWithNewDungeon().Count > 0);
    }

    #region [ Dungeon Data ]

    public static readonly Dictionary<DungeonType, Dictionary<int, (string name, int[] values)>> DungeonData =
        new Dictionary<DungeonType, Dictionary<int, (string name, int[] values)>>()
        {
            // 캠페인
            { DungeonType.Normal, new Dictionary<int, (string, int[])>
                {
                    { 0, ("불", new[] { 3, 10, 17, 24, 31 }) },
                    { 1, ("물", new[] { 4, 11, 18, 25, 32 }) },
                    { 2, ("풀", new[] { 5, 12, 19, 26, 33 }) },
                } },
            // 협동전
            { DungeonType.Elite, new Dictionary<int, (string, int[])>
                {
                    { 0, ("균열", new[] { 6, 13, 20, 27, 34 }) },
                    { 1, ("대피", new[] { 7, 14, 21, 28, 35 }) },
                    { 2, ("표류기", new[] { 8, 15, 22, 29, 36 }) },
                } },
            // 보스
            { DungeonType.Boss, new Dictionary<int, (string, int[])>
                {
                    { 0, ("보스", new[] { 9, 16, 23, 30, 37 }) },
                } },
        };

    public static bool TryGetData(DungeonType type, int banner, out (string name, int[] values) data)
    {
        data = default;
        return DungeonData.TryGetValue(type, out var tabData) && tabData.TryGetValue(banner, out data);
    }

    public static int GetBannerCount(DungeonType type)
    {
        return DungeonData.TryGetValue(type, out var tabData) ? tabData.Count : 0;
    }

    public static bool TryGetValue(DungeonType type, int banner, int side, out int value)
    {
        value = 0;

        if (!TryGetData(type, banner, out var data) || side < 0 || side >= data.values.Length) {
            return false;
        }

        value = data.values[side];
        return true;
    }

    public static string GetBannerName(DungeonType type, int banner)
    {
        return TryGetData(type, banner, out var data) ? data.name : string.Empty;
    }

    public static int GetLevel(DungeonType type, int banner, int side)
    {
        return TryGetValue(type, banner, side, out int value) ? value : 0;
    }

    #endregion
}
