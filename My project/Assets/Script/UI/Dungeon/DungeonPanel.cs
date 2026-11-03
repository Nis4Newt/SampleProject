using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DungeonType
{
    Normal,
    Elite,
    Boss
}

public class DungeonPanel : ModelPanel<DungeonModel>
{
    [Header("Tabs (항상 표시)")]
    [SerializeField] private List<TabButton> _tabButtons = new List<TabButton>();

    [Header("Banner Window (배너는 탭끼리 재활용)")]
    [SerializeField] private GameObject _bannerWindow;
    [SerializeField] private List<TabButton> _banners = new List<TabButton>();

    [Header("Detail Window (사이드탭 + 설명창)")]
    [SerializeField] private GameObject _detailWindow;
    [SerializeField] private List<StateSlot> _sideTabs = new List<StateSlot>();
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private GameObject _dim;

    private DungeonType _currentTab = DungeonType.Normal;
    private int _currentBanner;
    private int _currentSide;
    private bool _isDetailView = false;

    protected override void OnBound()
    {
        Model.Changed += Refresh;
    }

    protected override void OnUnbound()
    {
        Model.Changed -= Refresh;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        for (DungeonType type = 0; type <= DungeonType.Boss; type++) {
            DungeonType tabType = type;
            AddListener(_tabButtons[(int)type], () => SelectTab(tabType));
        }

        for (int i = 0; i < _banners.Count; i++) {
            int bannerIndex = i;
            AddListener(_banners[i], () => SelectBanner(_currentTab, bannerIndex));
        }

        for (int i = 0; i < _sideTabs.Count; i++) {
            int sideIndex = i;
            _sideTabs[i].AddListener(() => OnSideTabClicked(sideIndex));
        }

        SelectTab(DungeonType.Normal);
    }

    private void SelectTab(DungeonType type)
    {
        if (_isDetailView) {
            Model.ConfirmAllDungeons(_currentTab, _currentBanner);
        }

        _currentTab = type;
        RefreshBanners(type);

        if (DungeonModel.GetBannerCount(type) <= 1) {
            SelectBanner(type, 0);
            return;
        }

        SetWindow(showBanner: true);
    }

    private void SelectBanner(DungeonType type, int bannerIndex)
    {
        _currentTab = type;
        _currentBanner = bannerIndex;

        SetWindow(showBanner: false);
        ShowSideTab(0);
    }

    private void OnSideTabClicked(int sideIndex)
    {
        int dungeonLevel = DungeonModel.GetLevel(_currentTab, _currentBanner, sideIndex);
        Model.ConfirmOneDungeons(_currentTab, _currentBanner, dungeonLevel);

        ShowSideTab(sideIndex);
    }

    private void ShowSideTab(int sideIndex)
    {
        _currentSide = sideIndex;

        if (_descriptionText != null) {
            int dungeonLevel = DungeonModel.GetLevel(_currentTab, _currentBanner, sideIndex);
            _descriptionText.text = $"{DungeonModel.GetBannerName(_currentTab, _currentBanner)}-{dungeonLevel}";
        }

        RefreshSideTabs();
        RefreshDim();
    }

    private void RefreshTab()
    {
        var newTabs = Model.GetTabsWithNewDungeon();
        foreach (DungeonType type in Enum.GetValues(typeof(DungeonType))) {
            _tabButtons[(int)type].SetNotify(newTabs.Contains(type));
        }
    }

    private void RefreshBanners(DungeonType type)
    {
        DungeonModel.DungeonData.TryGetValue(type, out var tabData);
        var newBanners = Model.GetBannersWithNewDungeon(type);

        for (int i = 0; i < _banners.Count; i++) {
            bool used = tabData != null && tabData.ContainsKey(i);
            _banners[i].gameObject.SetActive(used);

            if (used) {
                _banners[i].SetText(tabData[i].name);
                _banners[i].SetNotify(newBanners.Contains(i));
            }
        }

        RefreshTab();
    }

    private void RefreshSideTabs()
    {
        int[] values = DungeonModel.TryGetData(_currentTab, _currentBanner, out var data) ? data.values : null;
        var newDungeons = Model.GetNewDungeons(_currentTab, _currentBanner);

        for (int i = 0; i < _sideTabs.Count; i++) {
            bool used = values != null && i < values.Length;
            _sideTabs[i].gameObject.SetActive(used);

            if (!used) {
                continue;
            }

            _sideTabs[i].SetText(values[i].ToString());
            _sideTabs[i].SetState(GetSideTabState(values[i], newDungeons));
        }

        RefreshTab();
    }

    private SlotState GetSideTabState(int dungeonLevel, List<int> newDungeons)
    {
        if (newDungeons.Contains(dungeonLevel)) {
            return SlotState.New;
        }

        return dungeonLevel <= Model.UserLevel ? SlotState.Confirmed : SlotState.Locked;
    }

    private void RefreshDim()
    {
        if (_dim == null) {
            return;
        }

        bool hasValue = DungeonModel.TryGetValue(_currentTab, _currentBanner, _currentSide, out int value);
        _dim.SetActive(hasValue && value > Model.UserLevel);
    }

    private void SetWindow(bool showBanner)
    {
        if (_bannerWindow != null) {
            _bannerWindow.SetActive(showBanner);
        }
        if (_detailWindow != null) {
            _detailWindow.SetActive(!showBanner);
        }

        _isDetailView = !showBanner;
    }

    private void Refresh(NotifyKey key, bool isOn)
    {
        if (key.Contents != ContentsType.Dungeon) {
            return;
        }

        RefreshTab();
        RefreshBanners(_currentTab);
        if (_detailWindow != null && _detailWindow.activeInHierarchy) {
            RefreshDim();
        }
    }
}
