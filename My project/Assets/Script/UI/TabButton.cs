using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : Button
{
    [Header("Visual")]
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _noti;

    private bool _onNotify = false;

    protected override void Awake()
    {
        base.Awake();

        if (Application.isPlaying) {
            _noti.SetActive(_onNotify);
        }
    }

    public void SetText(string value)
    {
        _text.text = value;
    }

    public void SetNotify(bool isOn)
    {
        _onNotify = isOn;
        _noti.SetActive(_onNotify);
    }
}
