using System.Collections.Generic;
using UnityEngine;

public enum CodexTabType
{
    Number,
    Korean,
    English
}

public class CodexPanel : ModelPanel<CodexModel>
{
    [SerializeField] private TabButton[] _tabButtons;
    [SerializeField] private List<StateSlot> _slots = new List<StateSlot>();

    private CodexTabType _currentTab = CodexTabType.Number;

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

        for (int i = 0; i < _tabButtons.Length; i++) {
            var tab = (CodexTabType)i;
            AddListener(_tabButtons[i], () => SetTab(tab));
        }

        RefreshSlots();
        RefreshFlagNotify();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Model.ConfirmAllElementsState(_currentTab);
    }

    private void SetTab(CodexTabType tab)
    {
        if (_currentTab == tab) {
            return;
        }

        Model.ConfirmAllElementsState(_currentTab);
        _currentTab = tab;
        RefreshSlots();
    }

    private void Refresh(NotifyKey key, bool isOn)
    {
        if (key.Contents != ContentsType.Codex) {
            return;
        }

        RefreshFlagNotify();
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        var elementEnumerator = Model.GetElements(_currentTab).GetEnumerator();

        foreach (StateSlot slot in _slots) {
            bool hasElement = elementEnumerator.MoveNext();
            slot.gameObject.SetActive(hasElement);

            if (!hasElement) {
                continue;
            }

            var (element, state) = elementEnumerator.Current;
            slot.SetText(element);
            slot.SetState(state);
            slot.AddListener(() => OnSlotClicked(element, state));
        }
    }

    private void OnSlotClicked(string element, SlotState state)
    {
        if (state == SlotState.New) {
            Model.SetElementState(_currentTab, element, SlotState.Confirmed);
        }
    }

    private void RefreshFlagNotify()
    {
        for (int i = 0; i < _tabButtons.Length; i++) {
            _tabButtons[i].SetNotify(Model.HasNewElements((CodexTabType)i));
        }
    }
}
