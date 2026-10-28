using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    [Header("Close")]
    [SerializeField] private Button _closeButton;

    private readonly List<(Button button, UnityAction action)> _listeners = new List<(Button, UnityAction)>();

    protected virtual void OnEnable()
    {
        AddListener(_closeButton, Close);
    }

    protected virtual void OnDisable()
    {
        foreach (var (button, action) in _listeners) {
            if (button != null) {
                button.onClick.RemoveListener(action);
            }
        }
        _listeners.Clear();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    protected void AddListener(Button button, UnityAction action)
    {
        if (button == null) {
            return;
        }

        button.onClick.AddListener(action);
        _listeners.Add((button, action));
    }
}
