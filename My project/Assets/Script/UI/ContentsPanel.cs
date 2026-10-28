using System;
using System.Collections.Generic;
using UnityEngine;

public enum ContentsType
{
    Mail,
    Codex,
    Dungeon,
    C1,
    C2,
    C3,
    C4,
    C5,
    C6,
    C7
}

public class ContentsPanel : BasePanel
{
    [Header("Buttons")]
    [SerializeField] private List<ContentIconButton> _buttons = new List<ContentIconButton>();

    private Action<ContentsType> _onContentsClicked;

    protected override void OnEnable()
    {
        base.OnEnable();

        foreach (ContentIconButton iconButton in _buttons) {
            ContentsType type = iconButton.Type;
            AddListener(iconButton.Button, () => OnButtonClicked(type));
        }
    }

    public void AddClickListener(Action<ContentsType> listener)
    {
        _onContentsClicked += listener;
    }

    public void RemoveClickListener(Action<ContentsType> listener)
    {
        _onContentsClicked -= listener;
    }

    private void OnButtonClicked(ContentsType type)
    {
        _onContentsClicked?.Invoke(type);
        Close();
    }
}
