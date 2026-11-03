using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CodexModel : INotifySource
{
    public enum CodexFlag
    {
        NewNumber,
        NewKorean,
        NewEnglish
    }

    private readonly Dictionary<CodexTabType, Dictionary<string, SlotState>> _codexStates = new();

    public event Action<NotifyKey, bool> Changed;

    public CodexModel()
    {
        foreach (CodexTabType tab in Enum.GetValues(typeof(CodexTabType))) {
            _codexStates[tab] = GetTexts(tab).ToDictionary(text => text, _ => SlotState.Locked);
        }
    }

    #region [ Read Access ]

    public Dictionary<string, SlotState> GetElements(CodexTabType codexType)
    {
        return _codexStates.TryGetValue(codexType, out var elements)
            ? elements
            : new Dictionary<string, SlotState>();
    }

    public bool HasNewElements(CodexTabType codexType)
    {
        return _codexStates.TryGetValue(codexType, out var elements)
            && elements.Values.Any(state => state == SlotState.New);
    }

    #endregion

    #region [ Control Element State ]

    public void SetElementState(CodexTabType codexType, string element, SlotState state)
    {
        if (!IsValidElement(codexType, element)) {
            Debug.LogWarning($"CodexModel.SetElementState: 도감에 없는 값({codexType}, '{element}')이라 무시합니다.");
            return;
        }

        _codexStates[codexType][element] = state;
        NotifyChanged(codexType);
    }

    public void ConfirmAllElementsState(CodexTabType codexType)
    {
        if (!_codexStates.TryGetValue(codexType, out var elements)) {
            Debug.LogWarning($"CodexModel.ConfirmAllElementsState: 도감에 없는 타입({codexType})이라 무시합니다.");
            return;
        }

        foreach (var element in elements.Keys.ToList()) {
            if (elements[element] == SlotState.New) {
                elements[element] = SlotState.Confirmed;
            }
        }

        NotifyChanged(codexType);
    }

    #endregion

    #region [ Codex Protocol ]

    public void OnCodexProtocol(CodexTabType tab, string element)
    {
        if (!IsValidElement(tab, element)) {
            Debug.LogWarning($"CodexModel.OnCodexProtocol: 도감에 없는 값({tab}, '{element}')이라 무시합니다.");
            return;
        }

        _codexStates[tab][element] = SlotState.New;
        NotifyChanged(tab);
    }

    #endregion

    #region [ Helpers ]

    private void NotifyChanged(CodexTabType codexType)
    {
        var key = NotifyKey.Of(ContentsType.Codex, ConvertTypeToFlag(codexType));
        Changed?.Invoke(key, HasNewElements(codexType));
    }

    private static CodexFlag ConvertTypeToFlag(CodexTabType tab)
    {
        switch (tab) {
            case CodexTabType.Korean: return CodexFlag.NewKorean;
            case CodexTabType.English: return CodexFlag.NewEnglish;
            default: return CodexFlag.NewNumber;
        }
    }

    private bool IsValidElement(CodexTabType codexType, string element)
    {
        return _codexStates.TryGetValue(codexType, out var elements) && elements.ContainsKey(element);
    }

    #endregion

    #region [ Codex Data ]

    private static readonly string[] NumberTexts =
        { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
    private static readonly string[] KoreanTexts =
        { "가", "나", "다", "라", "마", "바", "사", "아", "자", "차", "카", "타" };
    private static readonly string[] EnglishTexts =
        { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };

    public static string[] GetTexts(CodexTabType tab)
    {
        switch (tab) {
            case CodexTabType.Number: return NumberTexts;
            case CodexTabType.Korean: return KoreanTexts;
            case CodexTabType.English: return EnglishTexts;
            default: return Array.Empty<string>();
        }
    }

    #endregion
}
