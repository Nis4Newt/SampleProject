using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPanel : BasePanel
{
    [Header("Tabs (항상 표시)")]
    [SerializeField] private List<TabButton> _tabButtons = new List<TabButton>();

    [Header("Banner Window (배너는 탭끼리 재활용)")]
    [SerializeField] private GameObject _bannerWindow;
    [SerializeField] private List<TabButton> _banners = new List<TabButton>();

    [Header("Detail Window (사이드탭 + 설명창)")]
    [SerializeField] private GameObject _detailWindow;
    [SerializeField] private List<TabButton> _sideTabs = new List<TabButton>();
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private GameObject _dim;

    private int _userLevel = 1;
    private int _currentTab;
    private int _currentBanner;
    private int _currentSide;

    protected override void OnEnable()
    {
        base.OnEnable();

        for (int i = 0; i < _tabButtons.Count; i++) {
            int tabIndex = i;
            AddListener(_tabButtons[i], () => SelectTab(tabIndex));
        }

        for (int i = 0; i < _banners.Count; i++) {
            int bannerIndex = i;
            AddListener(_banners[i], () => OpenDetail(_currentTab, bannerIndex));
        }

        for (int i = 0; i < _sideTabs.Count; i++) {
            int sideIndex = i;
            AddListener(_sideTabs[i], () => SelectSideTab(sideIndex));
        }

        SelectTab(0);
    }

    public void SetUserLevel(int level)
    {
        _userLevel = level;

        if (_detailWindow != null && _detailWindow.activeInHierarchy) {
            RefreshDim();
        }
    }

    private void SelectTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= _tabButtons.Count) {
            return;
        }

        _currentTab = tabIndex;
        RefreshBanners(tabIndex);

        if (GetBannerCount(tabIndex) <= 1) {
            OpenDetail(tabIndex, 0);
            return;
        }

        SetWindow(showBanner: true);
    }

    private void OpenDetail(int tabIndex, int bannerIndex)
    {
        _currentTab = tabIndex;
        _currentBanner = bannerIndex;

        SetWindow(showBanner: false);
        RefreshSideTabs();
        SelectSideTab(0);
    }

    private void SelectSideTab(int sideIndex)
    {
        _currentSide = sideIndex;

        if (_descriptionText != null) {
            _descriptionText.text = GetDescription(_currentTab, _currentBanner, sideIndex);
        }

        RefreshDim();
    }

    private void RefreshBanners(int tabIndex)
    {
        TempData.TryGetValue(tabIndex, out var tabData);

        for (int i = 0; i < _banners.Count; i++) {
            bool used = tabData != null && tabData.ContainsKey(i);
            _banners[i].gameObject.SetActive(used);

            if (used) {
                _banners[i].SetText(tabData[i].name);
            }
        }
    }

    private void RefreshSideTabs()
    {
        int[] values = TryGetData(_currentTab, _currentBanner, out var data) ? data.values : null;

        for (int i = 0; i < _sideTabs.Count; i++) {
            bool used = values != null && i < values.Length;
            _sideTabs[i].gameObject.SetActive(used);

            if (used) {
                _sideTabs[i].SetText(values[i].ToString());
            }
        }
    }

    private void RefreshDim()
    {
        if (_dim == null) {
            return;
        }

        _dim.SetActive(TryGetValue(_currentTab, _currentBanner, _currentSide, out int value) && value > _userLevel);
    }

    private void SetWindow(bool showBanner)
    {
        if (_bannerWindow != null) {
            _bannerWindow.SetActive(showBanner);
        }
        if (_detailWindow != null) {
            _detailWindow.SetActive(!showBanner);
        }
    }

    private static readonly Dictionary<int, Dictionary<int, (string name, int[] values)>> TempData =
        new Dictionary<int, Dictionary<int, (string, int[])>>
        {
            { 0, new Dictionary<int, (string, int[])>
                {
                    { 0, ("불", new[] { 3, 10, 17, 24, 31 }) },
                    { 1, ("물", new[] { 4, 11, 18, 25, 32 }) },
                    { 2, ("풀", new[] { 5, 12, 19, 26, 33 }) },
                } },
            { 1, new Dictionary<int, (string, int[])>
                {
                    { 0, ("균열", new[] { 6, 13, 20, 27, 34 }) },
                    { 1, ("대피", new[] { 7, 14, 21, 28, 35 }) },
                    { 2, ("표류기", new[] { 8, 15, 22, 29, 36 }) },
                } },
            { 2, new Dictionary<int, (string, int[])>
                {
                    { 0, ("보스", new[] { 9, 16, 23, 30, 37 }) },
                } },
        };

    private static int GetBannerCount(int tab)
    {
        return TempData.TryGetValue(tab, out var tabData) ? tabData.Count : 0;
    }

    private static bool TryGetData(int tab, int banner, out (string name, int[] values) data)
    {
        data = default;
        return TempData.TryGetValue(tab, out var tabData) && tabData.TryGetValue(banner, out data);
    }

    private static bool TryGetValue(int tab, int banner, int side, out int value)
    {
        value = 0;

        if (!TryGetData(tab, banner, out var data) || side < 0 || side >= data.values.Length) {
            return false;
        }

        value = data.values[side];
        return true;
    }

    private static string GetDescription(int tab, int banner, int side)
    {
        if (!TryGetData(tab, banner, out var data) || !TryGetValue(tab, banner, side, out int value)) {
            return string.Empty;
        }

        return $"{data.name} - {value}";
    }
}
