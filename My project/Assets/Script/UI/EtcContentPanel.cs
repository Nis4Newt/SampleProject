using TMPro;
using UnityEngine;

public class EtcContentPanel : BasePanel
{
    [Header("Text")]
    [SerializeField] private TMP_Text _typeText;

    public void Open(ContentsType type)
    {
        _typeText.text = type.ToString();
        Open();
    }
}
