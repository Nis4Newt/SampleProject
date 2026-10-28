using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CodexTabType
{
    Number,
    Korean,
    English
}

public class CodexPanel : BasePanel
{
    [Header("Tabs")]
    [SerializeField] private TabButton[] _tabButtons;

    [Header("Slots")]
    [SerializeField] private List<CodexSlot> _slots = new List<CodexSlot>();

    private static readonly string[] NumberTexts =
        { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
    private static readonly string[] KoreanTexts =
        { "가", "나", "다", "라", "마", "바", "사", "아", "자", "차", "카", "타" };
    private static readonly string[] EnglishTexts =
        { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };

    private CodexTabType _currentTab = CodexTabType.Number;

    protected override void OnEnable()
    {
        base.OnEnable();

        // _tabButtons 인덱스는 CodexTabType 값과 같은 순서 (Number, Korean, English)
        for (int i = 0; i < _tabButtons.Length; i++) {
            CodexTabType tab = (CodexTabType)i;
            AddListener(_tabButtons[i], () => SetTab(tab));
        }

        Refresh();
    }

    private void SetTab(CodexTabType tab)
    {
        _currentTab = tab;
        Refresh();
    }

    private void Refresh()
    {
        string[] texts = GetTexts(_currentTab);

        for (int i = 0; i < _slots.Count; i++) {
            bool used = i < texts.Length;
            _slots[i].gameObject.SetActive(used);

            if (used) {
                _slots[i].SetText(texts[i]);
            }
        }
    }

    private static string[] GetTexts(CodexTabType tab)
    {
        switch (tab) {
            case CodexTabType.Korean: return KoreanTexts;
            case CodexTabType.English: return EnglishTexts;
            default: return NumberTexts;
        }
    }
}
